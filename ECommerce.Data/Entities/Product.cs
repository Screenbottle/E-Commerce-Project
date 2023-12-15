namespace ECommerce.Data.Entities;
public class Product
{
    public int ProductId { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    // Additional product properties
 
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public Product(string name) {
        Name = name;
    }
}