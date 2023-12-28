namespace ECommerce.API.Endpoints;

public class OrderItemEndpoint : IEndPoint
{
    // public void Register(WebApplication app) => app.Register<OrderItem, OrderItemPostDTO, OrderItemPutDTO, OrderItemGetDTO>();
      private readonly IMapper _mapper;

      public OrderItemEndpoint( IMapper mapper)
    {
        _mapper = mapper;
    }


    public void Register (WebApplication app) 
    {
        app.MapGet($"/api/OrderItems/" + "{id}", HttpSingleAsync<OrderItem, OrderItemGetDTO>);
        app.MapGet($"/api/OrderItems", HttpGetAsync<OrderItem, OrderItemGetDTO>);
        app.MapPut($"/api/OrderItems/" + "{id}", HttpPutAsync);
        app.MapDelete($"/api/OrderItems/" + "{id}", HttpDeleteAsync);
        
    }

     public static async Task<IResult> HttpSingleAsync<TEntity, TDto>(DbService db, int id) where TEntity : class, IEntity where TDto : class
        {
            var result = await db.SingleAsync<TEntity, TDto>(id);
            if (result is null) return Results.NotFound();
            return Results.Ok(result);
        }

         public static async Task<IResult> HttpGetAsync<TEntity, TDto>(DbService db) where TEntity : class where TDto : class => 
        Results.Ok(await db.GetAsync<TEntity, TDto>());

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

    public async Task<IResult> HttpPutAsync(DbService db, OrderItemPutDTO dto)
{

        var orderItem = ConvertToEntity<OrderItem, OrderItemPutDTO>(dto);

        if (orderItem is OrderItem oItem)
        {
            var existingOrderItem = db.db.OrderItems.Find(oItem.Id);
            var product = db.db.Products.Find(oItem.ProductId);

            if (product != null && existingOrderItem?.ProductId == orderItem.ProductId)
            {
                if (existingOrderItem.Quantity > oItem.Quantity)
                {
                    var decreaseAmount = existingOrderItem.Quantity - oItem.Quantity;

                    product.InventoryQuantity += decreaseAmount;
                    existingOrderItem.Quantity -= decreaseAmount;
                

                    if (product.InventoryQuantity > 0)
                    {
                        product.InStock = true;
                    }

                }
                else if (existingOrderItem.Quantity < oItem.Quantity)
                {
                    var increaseAmount = oItem.Quantity - existingOrderItem.Quantity;
                    
                    product.InventoryQuantity -= increaseAmount;
                    existingOrderItem.Quantity += increaseAmount;
                
                    if (product.InventoryQuantity == 0)
                    {
                        product.InStock = false;
                    }
                }
                
                
                existingOrderItem.TotalPrice = product.Price * existingOrderItem.Quantity; 
                await db.db.SaveChangesAsync();
            }
            else
            {
                return Results.Problem($"Error: Product with Id {oItem.ProductId} does not exist in the database. ");
            }
        
        }
        return Results.NoContent();
    }


               


    public OrderItem? ConvertToEntity<TEntity, TDto>(TDto dto) where TEntity : class, IEntity where TDto : class
    {
        
        var entity = _mapper.Map<TEntity>(dto);

            if (typeof(TEntity) == typeof(OrderItem))
        {
             var orderItem = entity as OrderItem;
            if (orderItem != null)
            {
               
                return orderItem;
            }
        }
        return null;
    }
}