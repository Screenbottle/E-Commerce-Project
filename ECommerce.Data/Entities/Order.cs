namespace ECommerce.Data.Entities;

public class Order : IEntity
{
    public int Id { get; set; }
    public int CustomerId { get; set; } 
    public DateTime OrderDate { get; set; }
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}