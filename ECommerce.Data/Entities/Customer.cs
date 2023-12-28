namespace ECommerce.Data.Entities;

public class Customer : IEntity
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }

    public string Street {get; set;}
    public string PostCode {get; set;}
    public string City {get; set;}
    public string? Apartment {get; set;}
 
    public ICollection<Order>? Orders { get; set; } = new List<Order>();

    public Customer(string firstName, string lastName, string street, string postcode, string city, string? apartment) {
        FirstName = firstName;
        LastName = lastName;
        Street = street;
        PostCode = postcode;
        City = city;
        Apartment = apartment;
    }
}