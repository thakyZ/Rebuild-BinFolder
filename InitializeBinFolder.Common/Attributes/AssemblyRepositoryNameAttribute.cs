using System;

namespace InitializeBinFolder.Common.Attributes;

/// <summary>
/// An assembly attribute to supply the name of the git repository.
/// </summary>
[AttributeUsage(AttributeTargets.Assembly)]
internal class AssemblyRepositoryNameAttribute : Attribute {
  /// <summary>
  /// Gets the value of the repository name of the assembly.
  /// </summary>
  internal string Value { get; }

  /// <summary>
  /// Creates a new instance of <see cref="AssemblyRepositoryNameAttribute" />.
  /// </summary>
  internal AssemblyRepositoryNameAttribute(string value) {
    this.Value = value;
  }
}
