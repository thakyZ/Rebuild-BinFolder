using System.ComponentModel;

namespace Rebuild_BinFolder.Attributes;

/// <summary>
/// Creates a default value from calling a constructor for a tpye.
/// </summary>
internal class DefaultValueNewAttribute : DefaultValueAttribute {
  /// <summary>
  /// Gets the type of the class to construct.
  /// </summary>
  private Type Type { get; }

  /// <summary>
  /// Gets an array of values to use as arguments.
  /// </summary>
  private object?[] Args { get; }

  /// <summary>
  /// Gets the default value as a new instance of the type.
  /// </summary>
  public override object? Value => Activator.CreateInstance(this.Type, this.Args);

  /// <summary>
  /// Creates a new instance of <see cref="DefaultValueNewAttribute" />
  /// </summary>
  /// <param name="type">The type of the class to construct.</param>
  /// <param name="args">Values to use as arguments.</param>
  internal DefaultValueNewAttribute(Type type, params object?[] args) : base(null) {
    this.Type = type;
    this.Args = args;
  }
}
