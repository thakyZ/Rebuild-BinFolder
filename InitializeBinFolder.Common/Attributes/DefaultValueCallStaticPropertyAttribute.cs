using System;
using System.ComponentModel;
using System.Reflection;
using InitializeBinFolder.Common.Exceptions;

namespace InitializeBinFolder.Common.Attributes;

/// <summary>
/// A property attribute to set the default property by call of an external static property.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
internal class DefaultValueCallStaticPropertyAttribute : DefaultValueAttribute {
  /// <summary>
  /// The type of class where the static property is kept.
  /// </summary>
  private Type Type { get; }

  /// <summary>
  /// Gets the name of the property to get from the provided class type.
  /// </summary>
  private string Property { get; }

  /// <summary>
  /// Gets the supplied property value as a nullable object.
  /// </summary>
  public override object? Value => this.GetPropertyValue();

  /// <summary>
  /// Constructs a new instance of <see cref="DefaultValueCallStaticPropertyAttribute" />.
  /// </summary>
  internal DefaultValueCallStaticPropertyAttribute(Type type, string property) : base(null) {
    this.Type = type;
    this.Property = property;
  }

  /// <summary>
  /// Internal wrapper for the property getter <see cref="Value" />.
  /// </summary>
  /// <returns>An nullable instance of the property.</returns>
  /// <exception cref="FailedOperationException">Thrown when the operation failed to get the value of the property.</exception>
  private object? GetPropertyValue() {
    try {
      return this.Type.GetProperty(this.Property, BindingFlags.Public | BindingFlags.Static)?.GetMethod?.Invoke(null, null);
    } catch (Exception exception) {
      throw new FailedOperationException($"Failed to get property {this.Property} of type {this.Type.FullName}", exception);
    }
  }
}
