using System;
using System.ComponentModel;
using System.Reflection;

using InitializeBinFolder.Common.Exceptions;

namespace InitializeBinFolder.Common.Attributes;

/// <summary>
/// Gets a default value from calling a static method in another class.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
internal class DefaultValueCallStaticMethodAttribute : DefaultValueAttribute {
  /// <summary>
  /// The type of class where the static method is kept.
  /// </summary>
  private Type Type { get; }

  /// <summary>
  /// Gets the name of the method to get from the provided class type.
  /// </summary>
  private string Method { get; }

  /// <summary>
  /// Gets the supplied method's return value as a nullable object.
  /// </summary>
  public override object? Value => this.GetMethodValue();

  /// <summary>
  /// Constructs a new instance of <see cref="DefaultValueCallStaticMethodAttribute"/>.
  /// </summary>
  public DefaultValueCallStaticMethodAttribute(Type type, string method) : base(null) {
    this.Type = type;
    this.Method = method;
  }

  /// <summary>
  /// Internal wrapper for the property getter <see cref="Value" />.
  /// </summary>
  /// <returns>An nullable instance of the method's return type.</returns>
  /// <exception cref="FailedOperationException">Thrown when the operation failed to get the value of the method's return type.</exception>
  private object? GetMethodValue() {
    try {
      return this.Type.GetMethod(this.Method, BindingFlags.Public | BindingFlags.Static)?.Invoke(null, null);
    } catch (Exception exception) {
      throw new FailedOperationException($"Failed to get method {this.Method} of type {this.Type.FullName}.", exception);
    }
  }
}
