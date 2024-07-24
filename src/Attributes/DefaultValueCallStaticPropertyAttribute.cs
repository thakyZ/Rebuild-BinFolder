using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Rebuild_BinFolder.Attributes;
internal class DefaultValueCallStaticPropertyAttribute : DefaultValueAttribute {
  public Type Type { get; }
  public string Property { get; }

  public DefaultValueCallStaticPropertyAttribute(Type type, string property) : base(null) {
    this.Type = type;
    this.Property = property;
  }

  public override object? Value {
    get {
      try {
        return this.Type.GetProperty(this.Property, BindingFlags.Public | BindingFlags.Static)?.GetMethod?.Invoke(null, null);
      } catch (Exception e) {
        Log.Error(e, $"Failed to get property {this.Property} of type {this.Type.FullName}");
        return null;
      }
    }
  }
}
