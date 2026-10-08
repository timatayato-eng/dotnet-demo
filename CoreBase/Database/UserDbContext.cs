using CoreBase.Entities;
using Microsoft.EntityFrameworkCore;

namespace CoreBase.Database;

public class UserDbContext : DbContext
{
  public UserDbContext(DbContextOptions<UserDbContext> options) : base(options) { }

  public DbSet<User> Users { get; set; } = null!;
}
