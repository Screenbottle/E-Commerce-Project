

namespace ECommerce.API.Endpoints;

public class OrderEndpoint : IEndPoint
{
      
      private readonly IMapper _mapper;

      public OrderEndpoint( IMapper mapper)
    {
        _mapper = mapper;
    }


    public void Register (WebApplication app) 
    {
        app.MapGet($"/api/Orders/" + "{id}", HttpSingleAsync<Order, OrderGetDTO>);
        app.MapGet($"/api/Orders", HttpGetAsync<Order, OrderGetDTO>);
        app.MapPost($"api/Order", HttpPostAsync);
        app.MapDelete($"/api/Orders/" + "{id}", HttpDeleteAsync);
        app.MapPut($"/api/Orders/" + "{id}", HttpPutAsync);
        

       
    }

    //  public static void Register<TEntity, TPostDto, TPutDto, TGetDto>(this WebApplication app)
    // where TEntity : class, IEntity where TPostDto : class where TPutDto : class where TGetDto : class
    // {
    //     var node = typeof(TEntity).Name.ToLower();
    //     app.MapGet($"/api/{node}s/" + "{id}", HttpSingleAsync<TEntity, TGetDto>);
    //     app.MapGet($"/api/{node}s", HttpGetAsync<TEntity, TGetDto>);
    //     app.MapPost($"/api/{node}", HttpPostAsync<TEntity, TPostDto>);
    //     app.MapPut($"/api/{node}s/" + "{id}", HttpPutAsync<TEntity, TPutDto>);
    //     app.MapDelete($"/api/{node}s/" + "{id}", HttpDeleteAsync<TEntity>);
    // }
    

    public static async Task<IResult> HttpSingleAsync<TEntity, TDto>(DbService db, int id) where TEntity : class, IEntity where TDto : class
        {
            var result = await db.SingleAsync<TEntity, TDto>(id);
            if (result is null) return Results.NotFound();
            return Results.Ok(result);
        }
    public static async Task<IResult> HttpGetAsync<TEntity, TDto>(DbService db) where TEntity : class where TDto : class => 
        Results.Ok(await db.GetAsync<TEntity, TDto>());

    public async Task<IResult> HttpPostAsync(DbService db, OrderPostDTO dto)
    {
        var order = ConvertToEntity<Order, OrderPostDTO>(dto);

        if (order is Order newOrder) {
        
        foreach (var oItem in newOrder.OrderItems)
        {
            var product = db.db.Products.Find(oItem.ProductId);
            var quantity = oItem.Quantity;

            if (product != null) {
                var inventoryOuantity = db.db.Products.First(p => p.Id == product.Id).InventoryQuantity;
                var inStock = db.db.Products.First(p => p.Id == product.Id).InStock;

                if(quantity <= inventoryOuantity && inStock) {
                    // check success
                    oItem.TotalPrice = product.Price * quantity; 
                }
                else {
                    return Results.Problem($"Error: Not enough of {product.Name} in stock");
                }  
            } else 
            {
                return Results.Problem($"Error: Product with Id {oItem.ProductId} does not exist in the database. "); 
            }
        }

        // if the previous foreach is completed without problems - decrease Product.inventoryQuantity

        foreach (var oItem in newOrder.OrderItems) 
        {   
            var product = db.db.Products.Find(oItem.ProductId);
            var quantity = oItem.Quantity;
            if(product != null) {
                product.InventoryQuantity -= quantity;

                if(product.InventoryQuantity == 0) {
                    product.InStock = false;
                }
            }
        }

        // use context
        db.db.Orders.Add(newOrder);
        await db.db.SaveChangesAsync();
            
        }
        
        return Results.Created();
    }

    public async Task<IResult> HttpPutAsync(DbService db, OrderPutDTO dto) {
        var order = ConvertToEntity<Order, OrderPutDTO>(dto);

        if (order is Order newOrder) {

            var oldOrder = db.db.Orders.Find(newOrder.Id);

            if(oldOrder != null)
            {   
                var oldOrderItems = oldOrder.OrderItems;
                var newOrderItems = newOrder.OrderItems;
                
                // if a customer will be able to remove items from their order and change the number of items at the same time, there needs to be another check
                // compare the quantity of each product, update the stock quantity if they differ, return an error message if the new quantity of products in the order is larger than what's in stock
                if(newOrderItems.Count == oldOrderItems.Count) {
                    for(int i = 0; i < newOrderItems.Count; i++) {

                        var newOrderItem = newOrderItems.ElementAt(i);
                        var oldOrderItem = oldOrderItems.ElementAt(i);

                        int difference = OrderItemQuantityDiff(newOrderItem, oldOrderItem);

                        if(difference < 0) {
                            var product = db.db.Products.Find(newOrderItem.ProductId);
                            if(product != null) {
                                newOrderItem.TotalPrice = product.Price * newOrderItem.Quantity; 
                                product.InventoryQuantity -= difference;

                                if(product.InventoryQuantity > 0) {
                                    product.InStock = true;
                                }
                            }
                        }
                        else if(difference > 0) {
                            var product = db.db.Products.Find(newOrderItem.ProductId);
                            if(product != null) {
                                newOrderItem.TotalPrice = product.Price * newOrderItem.Quantity; 

                                if(product.InventoryQuantity - newOrderItem.Quantity < 0) {
                                    return Results.Problem($"Error: Not enough of {product.Name} in stock");
                                }

                                product.InventoryQuantity -= difference;

                                if(product.InventoryQuantity == 0) {
                                    product.InStock = false;
                                }

                            }
                        }
                    }
                }
            }

            db.db.Orders.Update(newOrder);
            await db.db.SaveChangesAsync();
        }

        return Results.Created();
    }

    static int OrderItemQuantityDiff(OrderItem newItem, OrderItem oldItem) {
        return newItem.Quantity - oldItem.Quantity;
      }

    public async Task<IResult> HttpDeleteAsync(DbService db, int id)
    {
        try
        {
            if(!await db.DeleteAsync<Order>(id)) return Results.NotFound();

            if (await db.SaveChangesAsync()) return Results.NoContent();
        }
        catch
        {
        }

        return Results.BadRequest($"Couldn't delete the Order entity.");

    }

    public Order? ConvertToEntity<TEntity, TDto>(TDto dto) where TEntity : class, IEntity where TDto : class
    {
        
        Console.WriteLine($"ConvertToEntity körssss");
        var entity = _mapper.Map<TEntity>(dto);

            if (typeof(TEntity) == typeof(Order))
        {
            Console.WriteLine("type är order");
            
             var order = entity as Order;
            if (order != null)
            {
                Console.WriteLine($"{order.CustomerId}");
                return order;
            }
        }
        //Console.WriteLine($"Order: {entity}");
        return null;
    }

}