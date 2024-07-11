namespace Rebuild_BinFolder.Attributes;

[AttributeUsage(AttributeTargets.Assembly)]
public class AssemblyRepositoryOwnerAttribute : Attribute {
  public string Value { get; private set; }

  public AssemblyRepositoryOwnerAttribute(string value) {
    this.Value = value;
  }
}
