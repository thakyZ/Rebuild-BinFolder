namespace Rebuild_BinFolder.Attributes;

[AttributeUsage(AttributeTargets.Assembly)]
public class AssemblyRepositoryNameAttribute : Attribute {
  public string Value { get; private set; }

  public AssemblyRepositoryNameAttribute(string value) {
    this.Value = value;
  }
}
