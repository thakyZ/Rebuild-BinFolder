using Newtonsoft.Json;

namespace Rebuild_BinFolder.Configuration;

[Serializable]
public class ProgramPath {
  public string Path { get; } = string.Empty;

  public bool Exists => Directory.Exists(Path);

  public DirectoryInfo GetDirectoryInfo => new(Path);

  [JsonConstructor]
  public ProgramPath(string path) {
    this.Path = !path.Contains(@"\\") ? path.Replace("/", @"\\") : path;
  }

  private ProgramPath() { }

  public static ProgramPath Empty => new();

  public override string ToString() => Path;
}
