using System;

namespace InitializeBinFolder.Common.Attributes;

/// <summary>
/// An assembly attribute to supply the name of the git repository owner.
/// </summary>
[AttributeUsage(AttributeTargets.Assembly)]
internal class AssemblyRepositoryOwnerAttribute : Attribute {
  /// <summary>
  /// Gets the value of the repository owner of the assembly.
  /// </summary>
  internal string Value { get; }

  /// <summary>
  /// Creates a new instance of <see cref="AssemblyRepositoryOwnerAttribute" />.
  /// </summary>
  internal AssemblyRepositoryOwnerAttribute(string value) {
    this.Value = value;
  }
}
