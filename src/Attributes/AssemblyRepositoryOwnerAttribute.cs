namespace Rebuild_BinFolder.Attributes;

[AttributeUsage(AttributeTargets.Assembly)]
public class AssemblyRepositoryOwnerAttribute : Attribute {
  public string Value { get; }

  public AssemblyRepositoryOwnerAttribute(string value) {
    this.Value = value;
  }
}
