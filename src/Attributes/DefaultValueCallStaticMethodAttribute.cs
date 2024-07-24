using System.ComponentModel;
using System.Reflection;

namespace Rebuild_BinFolder.Attributes;
public class DefaultValueCallStaticMethodAttribute : DefaultValueAttribute {
  public Type Type { get; }
  public string Method { get; }

  public DefaultValueCallStaticMethodAttribute(Type type, string method) : base(null) {
    this.Type = type;
    this.Method = method;
  }

  public override object? Value {
    get {
      try {
        return this.Type.GetMethod(this.Method, BindingFlags.Public | BindingFlags.Static)?.Invoke(null, null);
      } catch (Exception e) {
        Log.Error(e, $"Failed to get method {this.Method} of type {this.Type.FullName}.");
      }
      return null;
    }
  }
}
