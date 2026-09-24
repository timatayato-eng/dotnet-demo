using CoreBase.Database;
using Microsoft.EntityFrameworkCore;

namespace RestApi.Config;

public static class DbConfig
{
  public static IServiceCollection ConfigDbContext(this IServiceCollection services, IConfiguration config)
  {
    var conn = config.GetConnectionString("DemoDb")
      ?? throw new InvalidOperationException("Connection string 'DemoDb' is missing.");

    services.AddDbContext<UserDbContext>(options => options.UseNpgsql(conn));
    return services;
  }
}
