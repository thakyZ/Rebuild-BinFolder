using Rebuild_BinFolder.Configuration;
using Rebuild_BinFolder.Extensions;

namespace Rebuild_BinFolder.HandlePaths;

public class PathData {
  /// <summary>
  /// The main environment variable FullName
  /// </summary>
  public List<ProgramPath> Path {
    get; set;
  }
  /// <summary>
  /// The main environment variable FullName as a properly formatted string.
  /// </summary>
  public string? PathString => Path == null ? null : string.Join(';', this.Path.ToRawList());

  /// <summary>
  /// Auxiliary path for more items in the path variable.
  /// </summary>
  public List<ProgramPath> AuxiliaryPath {
    get; set;
  }

  /// <summary>
  /// The main environment variable FullName as a properly formatted string.
  /// </summary>
  public string? AuxiliaryPathString => AuxiliaryPath == null ? null : string.Join(';', this.AuxiliaryPath.ToRawList());

  public PathData() {
    this.Path = [];
    this.AuxiliaryPath = [];
  }

  public PathData(List<ProgramPath>? path, List<ProgramPath>? auxiliaryPath) {
    this.Path = path ?? [];
    this.AuxiliaryPath = auxiliaryPath ?? [];
  }

  public PathData(List<string>? path, List<string>? auxiliaryPath) {
    this.Path = [..path?.Select(x => new ProgramPath(x)) ?? []];
    this.AuxiliaryPath = [..auxiliaryPath?.Select(x => new ProgramPath(x)) ?? []];
  }
}
