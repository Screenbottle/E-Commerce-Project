namespace ECommerce.Common.DTOs;

public class CustomerPostDTO 
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}

public class CustomerPutDTO : CustomerPostDTO
{
    public int Id { get; set; }
}

public class CustomerGetDTO : CustomerPutDTO
{
    public ICollection<OrderGetDTO> Orders { get; set; } = new List<OrderGetDTO>();
}
