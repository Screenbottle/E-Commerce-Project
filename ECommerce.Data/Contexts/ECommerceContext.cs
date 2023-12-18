namespace ECommerce.Data.Contexts;

using ECommerce.Data.Entities;
using Microsoft.EntityFrameworkCore;

public class ECommerceDbContext : DbContext
{
    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Customer> Customers { get; set; }

    public ECommerceDbContext (DbContextOptions <ECommerceDbContext> options) : base (options) { }
 
    // protected override void OnModelCreating(ModelBuilder builder)
    // {
    //     base.OnModelCreating(builder);
    //     // Configure relationships, constraints, etc.
    //     builder.Entity<OrderItem>()
    //         .HasKey(oi => new { oi.OrderItemId, oi.OrderId, oi.ProductId });
 
    //     builder.Entity<OrderItem>()
    //         .HasOne(oi => oi.Product)
    //         .WithMany(p => p.OrderItems)
    //         .HasForeignKey(oi => oi.ProductId);
 
    //     builder.Entity<OrderItem>()
    //         .HasOne(oi => oi.Order)
    //         .WithMany(o => o.OrderItems)
    //         .HasForeignKey(oi => oi.OrderId);
 
    //     // Add additional configurations as needed
    // }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=localhost,1433;Database=mydb;User Id=sa;Password=Sddk1234;MultipleActiveResultSets=true;Encrypt=false");
        }
        DbContextOptionsBuilder optionsBuilder = new DbContextOptionsBuilder();

        public ECommerceDbContext()
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=localhost,1433;Database=mydb;User Id=sa;Password=Sddk1234;MultipleActiveResultSets=true;Encrypt=false");
            }
        }

    }
