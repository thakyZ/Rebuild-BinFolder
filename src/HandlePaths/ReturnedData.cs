using Rebuild_BinFolder.Configuration;

namespace Rebuild_BinFolder.HandlePaths;

public class ReturnedData {
  /// <summary>
  /// The old original path variable
  /// </summary>
  public PathData NewPath {
    get; set;
  }

  /// <summary>
  /// The old original path variable
  /// </summary>
  public PathData OldPath {
    get; set;
  }

  public ReturnedData() {
    this.NewPath = new();
    this.OldPath = new();
  }

  public ReturnedData(PathData newPath, PathData oldPath) {
    this.NewPath = newPath ?? new();
    this.OldPath = oldPath ?? new();
  }

  public ReturnedData(List<ProgramPath> newPath, List<ProgramPath> oldPath, List<ProgramPath>? newAuxiliaryPath = null, List<ProgramPath>? oldAuxiliaryPath = null) {
    if (newAuxiliaryPath is not null && oldAuxiliaryPath is not null) {
      this.NewPath = new(newPath, newAuxiliaryPath);
      this.OldPath = new(oldPath, oldAuxiliaryPath);
    } else {
      this.NewPath = new(newPath);
      this.OldPath = new(oldPath);
    }
  }
}
