namespace ECommerce.Data.Entities;
public class Product : IEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
    [Precision(18, 2)]
    public decimal Price { get; set; }
    public int InventoryQuantity { get; set; } // Represents the available quantity of products.
    public bool InStock { get; set; }
 
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public Product(string name, decimal price, int inventoryQuantity) {
        Name = name;
        Price = price;
        InventoryQuantity = inventoryQuantity;
    }
}