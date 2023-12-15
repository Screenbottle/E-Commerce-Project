namespace ECommerce.Data.Entities;

public class Order
{
    public int OrderId { get; set; }
    public DateTime OrderDate { get; set; }
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    // Additional order properties
}