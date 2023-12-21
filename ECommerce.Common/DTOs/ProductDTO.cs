namespace ECommerce.Common.DTOs;

public class ProductPostDTO
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; } 
    public int InventoryQuantity { get; set; }
    public bool InStock { get; set; }
}

public class ProductPutDTO : ProductPostDTO 
{
    public int Id { get; set; }
}

public class ProductGetDTO : ProductPutDTO
{
    public List<OrderItemGetDTO> OrderItems { get; set; } = new List<OrderItemGetDTO>();
}

