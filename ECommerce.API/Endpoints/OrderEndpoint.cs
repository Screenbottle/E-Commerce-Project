

using ECommerce.Common.DTOs;

namespace ECommerce.API.Endpoints;

public class OrderEndpoint : IEndPoint
{
      
      private readonly IMapper _mapper;

      public OrderEndpoint( IMapper mapper)
    {
        _mapper = mapper;
    }
   // public void Register(WebApplication app) => app.Register<Order, OrderPostDTO, OrderPutDTO, OrderGetDTO>();

    public void Register (WebApplication app) 
    {
        //app.MapGet($"/api/Orderss/" + "{id}", HttpSingleAsync<TEntity, TGetDto>);
        app.MapGet($"/api/Orders", HttpGetAsync<Order, OrderGetDTO>);
        app.MapPost($"api/Order", HttpPostAsync);
        

       
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

                if(quantity <= inventoryOuantity) {
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

            product.InventoryQuantity -= quantity;
        }

        // use context
        db.db.Orders.Add(newOrder);
        await db.db.SaveChangesAsync();
            
        }
        
        return Results.Created();
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