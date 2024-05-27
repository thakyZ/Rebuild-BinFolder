using Rebuild_BinFolder.Configuration;
using Rebuild_BinFolder.Extensions;

namespace Rebuild_BinFolder.HandlePaths;

public class PathData {
  /// <summary>
  /// The main environment variable FullName
  /// </summary>
  public List<ProgramPath>? Path {
    get; set;
  }
  /// <summary>
  /// The main environment variable FullName as a properly formatted string.
  /// </summary>
  public string? PathString => Path == null ? null : string.Join(';', this.Path.ToNativeString());

  /// <summary>
  /// Auxiliary path for more items in the path variable.
  /// </summary>
  public List<ProgramPath>? AuxiliaryPath {
    get; set;
  }

  /// <summary>
  /// The main environment variable FullName as a properly formatted string.
  /// </summary>
  public string? AuxiliaryPathString => AuxiliaryPath == null ? null : string.Join(';', this.AuxiliaryPath.ToNativeString());

  public PathData(List<ProgramPath>? path = null, List<ProgramPath>? auxiliaryPath = null) {
    this.Path = path;
    this.AuxiliaryPath = auxiliaryPath;
  }
}
