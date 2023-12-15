namespace ECommerce.Data.Entities;

public class Customer
{
    public int CustomerId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    // Additional customer properties
 
    public ICollection<Order> Orders { get; set; } = new List<Order>();

    public Customer(string firstName, string lastName) {
        FirstName = firstName;
        LastName = lastName;
    }
}