

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
        
        app.MapPost($"api/Orderrrr", HttpPostAsync) ;
    }

    public async Task<IResult> HttpPostAsync(DbService db, OrderPostDTO dto)
    {
        var order = ConvertToEntity<Order, OrderPostDTO>(dto);

        if (order is Order newOrder) {
        
        foreach (var oItem in newOrder.OrderItems)
        {
            var product = db.db.Products.Find(oItem.ProductId);
            var quantity = oItem.Quantity;
            
            // TODO implement null check if the object Product is null
            var inventoryOuantity = db.db.Products.First(p => p.Id == product.Id).InventoryQuantity;

            if(quantity <= inventoryOuantity) {
                // check success
            }
            else {
                return Results.Problem($"Error: Not enough of {product.Name} in stock");
            }
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