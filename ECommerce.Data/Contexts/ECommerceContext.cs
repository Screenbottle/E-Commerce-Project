namespace ECommerce.Data.Contexts;

using ECommerce.Data.Entities;
using Microsoft.EntityFrameworkCore;

public class ECommerceDbContext : DbContext
{
    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Customer> Customers { get; set; }
 
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure relationships, constraints, etc.
        modelBuilder.Entity<OrderItem>()
            .HasKey(oi => new { oi.OrderItemId, oi.OrderId, oi.ProductId });
 
        modelBuilder.Entity<OrderItem>()
            .HasOne(oi => oi.Product)
            .WithMany(p => p.OrderItems)
            .HasForeignKey(oi => oi.ProductId);
 
        modelBuilder.Entity<OrderItem>()
            .HasOne(oi => oi.Order)
            .WithMany(o => o.OrderItems)
            .HasForeignKey(oi => oi.OrderId);
 
        // Add additional configurations as needed
    }
}