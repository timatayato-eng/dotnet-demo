using Microsoft.Extensions.DependencyInjection;

namespace CoreBase.Cores.Attribute;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class InjectableAttribute(ServiceLifetime lifetime = ServiceLifetime.Scoped, string? name = null) : System.Attribute
{
  public ServiceLifetime Lifetime { get; } = lifetime;
  public string? Name { get; } = name;
}
