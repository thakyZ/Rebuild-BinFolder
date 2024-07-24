using System.Runtime.InteropServices;

namespace Rebuild_BinFolder.Configuration;

public class CustomEnvironmentVar {
  /// <summary>
  /// TODO: Add property summary.
  /// </summary>
  public string Name { get; set; }

  /// <summary>
  /// TODO: Add property summary.
  /// </summary>
  public List<string> Value { get; set; }

  /// <summary>
  /// TODO: Add property summary.
  /// </summary>
  public string ValueSingle {
    get {
      if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
        return string.Join(';', this.Value);
      } else {
        return string.Join(':', this.Value);
      }
    }
  }

  /// <summary>
  /// TODO: Add method summary.
  /// </summary>
  /// <param name="value"></param>
  /// <returns></returns>
  private static List<string> StringToList(string value) {
    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && value.Contains(';')) {
      return [..value.Split(';')];
    } else if (value.Contains(':')) {
      return [..value.Split(':')];
    }
    return [value];
  }

  /// <summary>
  /// TODO: Add method summary.
  /// </summary>
  /// <param name="name"></param>
  /// <param name="value"></param>
  public CustomEnvironmentVar(string name, List<string> value) {
    this.Name=name;
    this.Value=value;
  }

  /// <summary>
  /// TODO: Add method summary.
  /// </summary>
  /// <param name="name"></param>
  /// <param name="value"></param>
  public CustomEnvironmentVar(string name, string? value) {
    this.Name=name;
    this.Value=StringToList(value ?? "<error>");
  }

  /// <summary>
  /// TODO: Add method summary.
  /// </summary>
  /// <param name="name"></param>
  /// <param name="value"></param>
  public CustomEnvironmentVar(string name, IEnumerable<string?> value) {
    this.Name=name;
    this.Value=[..value.Select(x => x ?? "<error>")];
  }
}
