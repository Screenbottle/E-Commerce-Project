namespace ECommerce.API.Endpoints;

public class ProductEndpoint : IEndPoint
{
    public void Register(WebApplication app) => app.Register<Product, ProductPostDTO, ProductPutDTO, ProductGetDTO>();
}