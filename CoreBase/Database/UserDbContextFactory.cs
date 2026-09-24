using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CoreBase.Database;

public class UserDbContextFactory : IDesignTimeDbContextFactory<UserDbContext>
{
  public UserDbContext CreateDbContext(string[] args)
  {
    var options = new DbContextOptionsBuilder<UserDbContext>()
      .UseNpgsql("Host=localhost;Port=5432;Database=dotnetdemo;Username=demo;Password=demo")
      .Options;

    return new UserDbContext(options);
  }
}
