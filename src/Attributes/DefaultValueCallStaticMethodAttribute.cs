using System.ComponentModel;
using System.Reflection;

namespace Rebuild_BinFolder.Attributes;
public class DefaultValueCallStaticMethodAttribute : DefaultValueAttribute {
  public Type Type { get; }
  public string Method { get; }

  public DefaultValueCallStaticMethodAttribute(Type type, string method) : base(null) {
    Type = type;
    Method = method;
  }

  public override object? Value => Type.GetMethod(Method, BindingFlags.Public | BindingFlags.Static)?.Invoke(null, null);
}
