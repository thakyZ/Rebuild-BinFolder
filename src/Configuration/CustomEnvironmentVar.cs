#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using System.Runtime.InteropServices;

namespace Rebuild_BinFolder.Configuration;

public class CustomEnvironmentVar {
  public string Name { get; set; }
  public List<string> Value { get; set; }
  public string ValueSingle {
    get {
      if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
        return string.Join(';', Value);
      } else {
        return string.Join(':', Value);
      }
    }
  }
  private static List<string> StringToList(string value) {
    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && value.Contains(';')) {
      return [..value.Split(';')];
    } else if (value.Contains(':')) {
      return [..value.Split(':')];
    }
    return [value];
  }
  public CustomEnvironmentVar(string name, List<string> value) {
    this.Name=name;
    this.Value=value;
  }
  public CustomEnvironmentVar(string name, string? value) {
    this.Name=name;
    this.Value=StringToList(value ?? "<error>");
  }
  public CustomEnvironmentVar(string name, IEnumerable<string?> value) {
    this.Name=name;
    this.Value=[..value.Select(x => x is null ? "<error>" : x)];
  }
}
