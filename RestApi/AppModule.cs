using System.Reflection;
using CoreBase.Cores.Attribute;

namespace RestApi;

public static class AppModule
{
  public static void AddAppModule(this IServiceCollection services)
  {
    var types = Assembly.GetExecutingAssembly()
      .GetTypes()
      .Where(t =>
          !t.IsAbstract &&
          !t.IsInterface &&
          t.GetCustomAttribute<InjectableAttribute>() != null);

    foreach (var type in types)
    {
      var attr = type.GetCustomAttribute<InjectableAttribute>();
      services.Add(new ServiceDescriptor(type, type, attr!.Lifetime));
    }
  }
}
