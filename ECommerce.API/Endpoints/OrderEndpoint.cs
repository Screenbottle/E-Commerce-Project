

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
        //app.MapPut($"/api/Orders/" + "{id}", HttpPutAsync);
        app.MapDelete($"/api/Orders/" + "{id}", HttpDeleteAsync);
        
    }
    

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
        
        var entity = _mapper.Map<TEntity>(dto);

            if (typeof(TEntity) == typeof(Order))
        {
             var order = entity as Order;
            if (order != null)
            {

                return order;
            }
        }
        return null;
    }

}