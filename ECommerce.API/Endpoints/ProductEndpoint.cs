namespace ECommerce.API.Endpoints;

public class ProductEndpoint : IEndPoint
{
    public void Register(WebApplication app) => app.Register<Product, ProductPostDTO, ProductPutDTO, ProductGetDTO>();
    
}


// add order - orderitem quantity får ej va högre än product.inventoryOuantity
// när order items skapas, ska product.inventory minskas?
