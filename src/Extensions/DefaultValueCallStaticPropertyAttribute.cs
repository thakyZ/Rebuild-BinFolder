using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Rebuild_BinFolder.Extensions;
internal class DefaultValueCallStaticPropertyAttribute : DefaultValueAttribute {
  public Type Type { get; }
  public string Property { get; }

  public DefaultValueCallStaticPropertyAttribute(Type type, string property) : base(null) {
    Type = type;
    Property = property;
  }

  public override object? Value => Type.GetProperty(Property, BindingFlags.Public | BindingFlags.Static)?.GetMethod?.Invoke(null, null);
}
