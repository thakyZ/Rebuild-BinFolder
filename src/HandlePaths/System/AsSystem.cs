using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Rebuild_BinFolder.Configuration;
using Rebuild_BinFolder.Extensions;
using Rebuild_BinFolder.Helpers;

namespace Rebuild_BinFolder.HandlePaths;
public partial class Handler {
  internal class AsSystem : AsGeneric {
    private static Regex APROG_DIR_Regex => new($"^%{Constants.SystemProgramsDirectory}%");

    public AsSystem(List<string> oldPath, List<string> oldAuxiliaryPath) : base(oldPath, oldAuxiliaryPath) {
    }

    /// <summary>
    /// Run the task as the system.
    /// </summary>
    /// <returns></returns>
    public override ReturnedData Run() {
      var output = new ReturnedData(OldPath, OldAuxiliaryPath);
      return output;
    }

    /// <summary>
    /// Transforms items in the string to their fully expanded string via the equivalents system.
    /// </summary>
    /// <param name="withDuplicates"></param>
    /// <returns></returns>
    internal override List<string> TransformEquivalent(List<string> withDuplicates) {
      Log.Debug("TransformEquivalent:");
      var output = withDuplicates.ConvertAll(x => {
        foreach ((string key, string value) in Equivalents) {
          Log.Debug("key: " + key + " | " + value);
          string newKeyFromString = key;
          if (key.Contains("<userDir>")) {
            Log.Debug("Contains: <userDir>");
            newKeyFromString = key.Replace("<userDir>", Regex.Escape(@$"C:\\Users\\{Environment.UserName}"));
          }

          if (x.Contains(newKeyFromString)) {
            return newKeyFromString.Replace(x, value);
          }

          return x;
        }

        return x;
      }).Distinct().ToList();

      foreach (var item in output) {
        Log.Debug(item);
      }

      return output;
    }

    /// <summary>
    /// Loops over the list of programs and removes duplicates with method one.
    /// </summary>
    /// <param name="withDuplicates"></param>
    /// <param name="programs"></param>
    /// <returns></returns>
    internal override List<string> LoopRemoveDuplicatesOne(List<string> withDuplicates, List<ProgramPath> programs) {
      var withOutDuplicates = TransformEquivalent(withDuplicates);

      Log.Info("LoopRemoveDuplicates1:");
      var output = withOutDuplicates.FindAll(x => {
        var yz = false;
        var yr = false;
        if (!APROG_DIR_Regex.IsMatch(x) && !Regex.IsMatch(x, Regex.Escape(Services.Config.AdminConfig.Root.Path.FullName))) {
          yr = true;
          if (!programs.Exists(y => y.FullName == x)) {
            yz = true;
            if (!programs.Exists(y => Regex.IsMatch(x, Regex.Escape(y.FullName)))) {
              Log.Debug($"returned True: '{x}' if(1):'{yr}' if(2):'{yz}' if(3):'True'");
              return true;
            }
          }
        }
        Log.Debug($"returned False: '{x}' if(1):'{yr}' if(2):'{yz}' if(3):'False'");

        return false;
      });
      foreach (var item in output) {
        Log.Debug(item);
      }
      return output;
    }

    /// <summary>
    /// Loops over the list of programs and removes duplicates with method two.
    /// </summary>
    /// <param name="withDuplicates"></param>
    /// <param name="programs"></param>
    /// <returns></returns>
    internal override List<string> LoopRemoveDuplicatesTwo(List<string> withDuplicates, List<ProgramPath> programs) {
      var withOutDuplicates = withDuplicates.Distinct().ToList();

      foreach ((string key, string value) in Equivalents) {
        string newKeyFromString = key;

        if (key.Contains("<userDir>")) {
          newKeyFromString = key.Replace("<userDir>", Regex.Escape(@$"C:\\Users\\{Environment.UserName}"));
        }

        withOutDuplicates = withOutDuplicates.ConvertAll(x => {
          if (x.Contains(newKeyFromString)) {
            return newKeyFromString.Replace(x, value);
          }
          return x;
        });

        withOutDuplicates = withOutDuplicates.Distinct().ToList();
      }

      int index = 0;
      while (index < withOutDuplicates.Count) {
        var replacedValueOne = Services.Config.AdminConfig.Root.Path.FullName.Replace(@"\", @"\\");

        if (Regex.IsMatch(withOutDuplicates[index], $"^{replacedValueOne}\\\\") || Regex.IsMatch(withOutDuplicates[index], $"^%{Constants.SystemProgramsDirectory}%")) {
          withOutDuplicates.RemoveAt(index);
          index = index - 1 < 0 ? 0 : index - 1;
        } else {
          foreach (ProgramPath program in programs) {
            var replacedValueTwo = program.FullName.Replace(@"\", @"\\");

            if (Regex.IsMatch(withOutDuplicates[index], replacedValueTwo)) {
              withOutDuplicates.RemoveAt(index);
              index = index - 1 < 0 ? 0 : index - 1;
            }
          }
        }

        index++;
      }

      return withOutDuplicates;
    }

    /// <summary>
    /// Removes Duplicates from the list of programs.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="programs"></param>
    /// <returns></returns>
    internal override List<string> RemoveDuplicates(List<string> path, List<ProgramPath> programs) {
      List<string> withDupes = path;
      var woDupes = withDupes.Distinct().ToList();

      // #pragma warning disable S1481
      var output1 = LoopRemoveDuplicatesOne(woDupes, programs);
      // #pragma warning restore S1481
      // var output2 = LoopRemoveDuplicatesTwo(woDupes, programs);

      return output1;
    }

    /// <summary>
    /// Formats and expands strings in the path.
    /// </summary>
    /// <param name="programs"></param>
    /// <returns></returns>
    internal override List<string> DoFormatting(List<ProgramPath> programs) {
      var _tempPath = new List<string>();

      foreach (var program in programs.Select(x => x.FullName)) {
        if (VolumeRegex().IsMatch(program)) {
          _tempPath.Add(program);
          Log.Additions("Adding FullName: ", program);
        } else {
          _tempPath.Add($"%{Constants.SystemProgramsDirectory}%\\{program}");
          Log.Additions("Adding FullName: ", $"%{Constants.SystemProgramsDirectory}%\\{program}");
        }
      }

      return _tempPath;
    }

    /// <summary>
    /// Updates the path with method two.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="newAux"></param>
    /// <returns></returns>
    internal override List<string> UpdatePathTwo(List<string> path, List<string> newAux) {
      _ = path.Remove($"%{Constants.SystemProgramsList}%");

      var updatedPath = UpdatePathOne(path);

      Log.Info("New FullName Removals: ");
      for (int i = 0; i < updatedPath.Count; i++) {
        if (Regex.IsMatch(updatedPath[i], Regex.Escape(Services.Config.AdminConfig.Root.Path.FullName.Replace(@"[\/]", @"\\"))) || APROG_DIR_Regex.IsMatch(updatedPath[i])) {
          Log.Additions("Removing FullName: ", updatedPath[i]);
          updatedPath.RemoveAt(i);
        } else {
          for (int j = 0; j < newAux.Count; j++) {
            if (j < updatedPath.Count && i < newAux.Count && updatedPath[i] == newAux[j]) {
              Log.Additions("Removing FullName: ", updatedPath[i]);
              updatedPath.RemoveAt(i);
            }
          }
        }
      }

      Log.Info("New FullName Additions: ");
      foreach (var programPath in Services.Config.AdminConfig.ForceInPath.Select(x => x.FullName)) {
        if (VolumeRegex().IsMatch(programPath)) {
          updatedPath.Add(programPath);
          Log.Additions("Adding FullName: ", programPath);
        } else {
          updatedPath.Add($"%{Constants.SystemProgramsDirectory}%\\{programPath}");
          Log.Additions("Adding FullName: ", $"%{Constants.SystemProgramsDirectory}%\\{programPath}");
        }
      }

      updatedPath.Add($"%{Constants.SystemProgramsList}%");

      return updatedPath;
    }

    internal override List<string> UpdatePathOne(List<string> path) {
      _ = path.Remove($"%{Constants.SystemProgramsList}%");

      var programs = Services.Config.AdminConfig.Programs;
      var tempPath = RemoveDuplicates(path, programs);

      _ = tempPath.Remove($"%{Constants.SystemProgramsList}%");

      return tempPath;
    }
  }
}
