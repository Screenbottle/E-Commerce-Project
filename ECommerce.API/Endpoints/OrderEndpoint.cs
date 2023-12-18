namespace ECommerce.API.Endpoints;

public class OrderEndpoint : IEndPoint
{
    public void Register(WebApplication app) => app.Register<Order, OrderPostDTO, OrderPutDTO, OrderGetDTO>();
}