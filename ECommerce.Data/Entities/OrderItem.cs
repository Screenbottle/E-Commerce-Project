namespace ECommerce.Data.Entities;

public class OrderItem
{
    public int OrderItemId { get; set; }
    public int ProductId { get; set; }
    public int OrderId { get; set; }
    public int Quantity { get; set; }
    public decimal TotalPrice { get; set; }
    // Additional order item properties
 
    public Product? Product { get; set; }
    public Order? Order { get; set; }
}