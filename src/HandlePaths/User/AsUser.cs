using System.Text.RegularExpressions;

using Rebuild_BinFolder.Configuration;
using Rebuild_BinFolder.Helpers;

namespace Rebuild_BinFolder.HandlePaths;
public partial class Handler {
  internal class AsUser : AsGeneric {
    private static Regex UPROG_DIR_Regex => new($"^%{Constants.UserProgramsDirectory}%");

    public AsUser(List<string> oldPath, List<string> oldAuxiliaryPath) : base(oldPath, oldAuxiliaryPath) {
    }

    /// <summary>
    /// Run the task as the current user.
    /// </summary>
    /// <returns></returns>
    public override ReturnedData Run() {
      throw new NotImplementedException();
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
        if (!UPROG_DIR_Regex.IsMatch(x) && !Regex.IsMatch(x, Regex.Escape(Services.Config.AdminConfig.AdminRoot.FullName))) {
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
    // #pragma warning disable SYSLIB1045, S1144, IDE0051
    internal override List<string> LoopRemoveDuplicatesTwo(List<string> withDuplicates, List<ProgramPath> programs) {
      var withOutDuplicates = withDuplicates.Distinct().ToList();

      foreach ((string key, string value) in Equivalents) {

        withOutDuplicates = withOutDuplicates.ConvertAll(x => {
          if (key == x) {
            return x.Replace(key, value);
          }
          return x;
        });

        withOutDuplicates = withOutDuplicates.Distinct().ToList();
      }

      int index = 0;
      while (index < withOutDuplicates.Count) {
        var replacedValueOne = Services.Config.AdminConfig.AdminRoot.FullName.Replace(@"\\", @"\").Replace(@"/", @"\");

        if (replacedValueOne == withOutDuplicates[index] || Regex.IsMatch(withOutDuplicates[index], $"^%{Constants.SystemProgramsDirectory}%")) {
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
    // #pragma warning restore SYSLIB1045, S1144, IDE0051

    /// <summary>
    ///
    /// </summary>
    /// <param name="path"></param>
    /// <param name="programs"></param>
    /// <returns></returns>
    internal override List<string> RemoveDuplicates(List<string> path, List<ProgramPath> programs) {
      List<string> withDupes = [.. path, .. programs.Select(x => x.FullName)];
      List<string> woDupes = [.. withDupes.Distinct()];

      // #pragma warning disable S1481
      List<string> output = LoopRemoveDuplicatesOne(woDupes, programs);
      // #pragma warning restore S1481
      // var output = LoopRemoveDuplicates2(woDupes, programs, isAdmin);

      return woDupes;
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
          _tempPath.Add($"%{Constants.UserProgramsDirectory}%\\{program}");
          Log.Additions("Adding FullName: ", $"%{Constants.UserProgramsDirectory}%\\{program}");
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
      _ = path.Remove($"%{Constants.UserProgramsList}%");

      var updatedPath = UpdatePathOne(path);

      Log.Info("New FullName Removals: ");
      for (int i = 0; i < updatedPath.Count; i++) {
        if (Regex.IsMatch(updatedPath[i], Regex.Escape(Services.Config.AdminConfig.AdminRoot.FullName.Replace(@"[\/]", @"\\"))) || UPROG_DIR_Regex.IsMatch(updatedPath[i])) {
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
      foreach (var programPath in Services.Config.AdminConfig.ForceInAdminPath.Select(x => x.FullName)) {
        if (VolumeRegex().IsMatch(programPath)) {
          updatedPath.Add(programPath);
          Log.Additions("Adding FullName: ", programPath);
        } else {
          updatedPath.Add($"%{Constants.UserProgramsDirectory}%\\{programPath}");
          Log.Additions("Adding FullName: ", $"%{Constants.UserProgramsDirectory}%\\{programPath}");
        }
      }

      updatedPath.Add($"%{Constants.UserProgramsList}%");

      return updatedPath;
    }

    /// <summary>
    /// Updates the path with method one.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    internal override List<string> UpdatePathOne(List<string> path) {
      _ = path.Remove($"%{Constants.UserProgramsList}%");

      var programs = Services.Config.GetUserConfigBySID().UserPrograms;
      var tempPath = RemoveDuplicates(path, programs);

      _ = tempPath.Remove($"%{Constants.UserProgramsList}%");

      return tempPath;
    }
  }
}
