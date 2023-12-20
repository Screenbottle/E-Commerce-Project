using System.Dynamic;

namespace ECommerce.Common.DTOs;

public class OrderPostDTO 
{
    public int CustomerId { get; set; }
    public DateTime OrderDate { get; set; }
    public ICollection<OrderItemPostDTO> OrderItems { get; set; } = new List<OrderItemPostDTO>();
}

public class OrderPutDTO : OrderPostDTO 
{
    public int Id { get; set; }
}

public class OrderGetDTO : OrderPutDTO 
{}