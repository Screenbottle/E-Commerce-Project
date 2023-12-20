namespace ECommerce.Data.Entities;

public class OrderItem : IEntity
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public Product Product { get; set; }
    public int OrderId { get; set; }
    public Order Order { get; set; }
    public int Quantity { get; set; }
    [Precision(18, 2)]
    public decimal TotalPrice { get; set; }
 
}