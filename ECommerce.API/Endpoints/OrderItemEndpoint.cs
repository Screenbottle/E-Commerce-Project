namespace ECommerce.API.Endpoints;

public class OrderItemEndpoint : IEndPoint
{
    public void Register(WebApplication app) => app.Register<OrderItem, OrderItemPostDTO, OrderItemPutDTO, OrderItemGetDTO>();
}