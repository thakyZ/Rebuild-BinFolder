namespace Rebuild_BinFolder.Attributes;

[AttributeUsage(AttributeTargets.Assembly)]
public class AssemblyRepositoryNameAttribute : Attribute {
  public string Value { get; }

  public AssemblyRepositoryNameAttribute(string value) {
    this.Value = value;
  }
}
