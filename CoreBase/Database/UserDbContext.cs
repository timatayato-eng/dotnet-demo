using CoreBase.Entities;
using Microsoft.EntityFrameworkCore;

namespace CoreBase.Database;

public class UserDbContext : DbContext
{
  public UserDbContext(DbContextOptions<UserDbContext> options) : base(options) { }

  public DbSet<User> Users { get; set; } = null!;
  public DbSet<Employer> Employers { get; set; } = null!;
  public DbSet<Customer> Customers { get; set; } = null!;
  public DbSet<Product> Products { get; set; } = null!;
  public DbSet<Order> Orders { get; set; } = null!;
  public DbSet<OrderItem> OrderItems { get; set; } = null!;

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<User>()
      .HasIndex(x => x.EmployerId);
    modelBuilder.Entity<Customer>()
      .HasIndex(x => x.EmployerId);
    modelBuilder.Entity<Product>()
      .HasIndex(x => x.EmployerId);
    modelBuilder.Entity<Order>()
      .HasIndex(x => x.CustomerId);
    modelBuilder.Entity<Order>()
      .HasIndex(x => x.UserId);
    modelBuilder.Entity<OrderItem>()
      .HasIndex(x => x.OrderId);
    modelBuilder.Entity<OrderItem>()
      .HasIndex(x => x.ProductId);
  }
}
