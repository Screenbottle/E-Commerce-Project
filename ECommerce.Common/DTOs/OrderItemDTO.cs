namespace ECommerce.Common.DTOs;



public class OrderItemPostDTO 
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal TotalPrice { get; set; }

}

public class OrderItemPutDTO : OrderItemPostDTO
{
   public int Id { get; set; } 
}

public class OrderItemGetDTO : OrderItemPutDTO 
{
    public int OrderId { get; set; }
    public ProductGetDTO Product { get; set; }
}
