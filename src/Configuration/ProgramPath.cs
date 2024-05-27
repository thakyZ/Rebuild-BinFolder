using System.Runtime.InteropServices;

using Newtonsoft.Json;

namespace Rebuild_BinFolder.Configuration;

[Serializable]
public class ProgramPath {
  public string FullName { get; } = string.Empty;

  public bool Exists => Directory.Exists(this.FullName);

  public DirectoryInfo GetDirectoryInfo => new(this.FullName);

  [JsonConstructor]
  public ProgramPath(string path) {
    this.FullName = path;
  }

  private ProgramPath() { }

  public static ProgramPath Empty => new();

  public override string ToString() => this.FullName;

  public string ToNativeString() {
    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
      return this.FullName.Replace(@"/", @"\");
    } else {
      return this.FullName.Replace(@"\\", "/").Replace(@"\", "/");
    }
  }
}
