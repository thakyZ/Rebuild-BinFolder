using System.Linq;
using System.Text.RegularExpressions;

using Rebuild_BinFolder.Configuration;
using Rebuild_BinFolder.Extensions;

using static Rebuild_BinFolder.HandlePaths.Handler;

namespace Rebuild_BinFolder.HandlePaths;
public interface IAsGeneric {
  /// <summary>
  /// Run the task as the system.
  /// </summary>
  /// <returns></returns>
  ReturnedData Run();
}

public abstract class AsGeneric : IAsGeneric {
  /// <summary>
  /// The main environment variable FullName
  /// </summary>
  public List<string>? OldPath {
    get; set;
  }

  /// <summary>
  /// Auxiliary path for more items in the path variable.
  /// </summary>
  public List<string>? OldAuxiliaryPath {
    get; set;
  }

  /// <summary>
  /// Dictionary of equivalents matching other side.
  /// </summary>
  internal Dictionary<string, string> Equivalents { get; }

  /// <summary>
  /// Abstract constructor for the run as classes.
  /// </summary>
  /// <param name="oldPath"></param>
  /// <param name="oldAuxiliaryPath"></param>
  protected AsGeneric(List<string> oldPath, List<string> oldAuxiliaryPath) {
    Equivalents = Services.Config.Equivalents.GetDictionary();
    Equivalents = Equivalents.Concat(RegHandler.GetStandardVariablesRegex()).ToDictionary(x => x.Key, x => x.Value);
    OldPath = oldPath;
    OldAuxiliaryPath = oldAuxiliaryPath;
  }

  /// <summary>
  /// Run the task as the system.
  /// </summary>
  /// <returns></returns>
  public abstract ReturnedData Run();

  /// <summary>
  /// Transforms items in the string to their fully expanded string via the equivalents system.
  /// </summary>
  /// <param name="withDuplicates"></param>
  /// <returns></returns>
  internal abstract List<string> TransformEquivalent(List<string> withDuplicates);
  /// <summary>
  /// Loops over the list of programs and removes duplicates with method one.
  /// </summary>
  /// <param name="withDuplicates"></param>
  /// <param name="programs"></param>
  /// <returns></returns>
  internal abstract List<string> LoopRemoveDuplicatesOne(List<string> withDuplicates, List<ProgramPath> programs);
  /// <summary>
  /// Loops over the list of programs and removes duplicates with method two.
  /// </summary>
  /// <param name="withDuplicates"></param>
  /// <param name="programs"></param>
  /// <returns></returns>
  internal abstract List<string> LoopRemoveDuplicatesTwo(List<string> withDuplicates, List<ProgramPath> programs);
  /// <summary>
  /// Removes Duplicates from the list of programs.
  /// </summary>
  /// <param name="path"></param>
  /// <param name="programs"></param>
  /// <returns></returns>
  internal abstract List<string> RemoveDuplicates(List<string> path, List<ProgramPath> programs);
  /// <summary>
  /// Formats and expands strings in the path.
  /// </summary>
  /// <param name="programs"></param>
  /// <returns></returns>
  internal abstract List<string> DoFormatting(List<ProgramPath> programs);
  /// <summary>
  /// Updates the path with method one.
  /// </summary>
  /// <param name="path"></param>
  /// <returns></returns>
  internal abstract List<string> UpdatePathOne(List<string> path);
  /// <summary>
  /// Updates the path with method two.
  /// </summary>
  /// <param name="path"></param>
  /// <param name="newAux"></param>
  /// <returns></returns>
  internal abstract List<string> UpdatePathTwo(List<string> path, List<string> newAux);
}
