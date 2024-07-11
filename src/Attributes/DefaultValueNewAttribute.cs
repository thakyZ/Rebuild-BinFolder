using System.ComponentModel;

namespace Rebuild_BinFolder.Attributes;
public class DefaultValueNewAttribute : DefaultValueAttribute {
  public Type Type { get; }
  public object[] Args { get; }

  public DefaultValueNewAttribute(Type type, params object[] args) : base(null) {
    Type = type;
    Args = args;
  }

  public override object? Value => Activator.CreateInstance(Type, Args);
}
