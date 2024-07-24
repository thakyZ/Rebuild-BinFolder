using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

using Rebuild_BinFolder.Configuration;
using Rebuild_BinFolder.Helpers;

namespace Rebuild_BinFolder.HandlePaths;
public partial class Handler {
  internal class AsUser : AsGeneric {
    private static Regex UPROG_DIR_Regex {
      get {
        return new($"^%{Constants.UserProgramsDirectory}%");
      }
    }

    public AsUser(List<string> oldPath, List<string> oldAuxiliaryPath) : base(oldPath, oldAuxiliaryPath) {
    }

    /// <summary>
    /// Run the task as the current user.
    /// </summary>
    /// <returns></returns>
    public override ReturnedData Run() {
      return new ReturnedData();
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
      if (Services.Config.GetUserConfigBySsid() is not UserConfig userConfig) {
        return [];
      }
      List<string> withOutDuplicates = this.TransformEquivalent(withDuplicates);

      Log.Info("LoopRemoveDuplicates1:");
      List<string> output = withOutDuplicates.FindAll(x => {
        var yz = false;
        var yr = false;
        if (!UPROG_DIR_Regex.IsMatch(x) && !Regex.IsMatch(x, Regex.Escape(userConfig.Root.Path.FullName))) {
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
    [SuppressMessage("Major Code Smell", "S127:\"for\" loop stop conditions should be invariant", Justification = "Unnecessary SonarLint Warning")]
    internal override List<string> LoopRemoveDuplicatesTwo(List<string> withDuplicates, List<ProgramPath> programs) {
      if (Services.Config.GetUserConfigBySsid() is not UserConfig userConfig) {
        return [];
      }

      var withOutDuplicates = withDuplicates.Distinct().ToList();

      foreach (var (key, value) in this.Equivalents) {
        withOutDuplicates = withOutDuplicates.ConvertAll(x => {
          if (key == x) {
            return x.Replace(key, value);
          }
          return x;
        });

        withOutDuplicates = withOutDuplicates.Distinct().ToList();
      }

      for (var index = 0; index < withOutDuplicates.Count;) {
        var replacedValueOne = userConfig.Root.Path.FullName.Replace(@"\\", @"\").Replace("/", @"\");

        if (replacedValueOne == withOutDuplicates[index] || Regex.IsMatch(withOutDuplicates[index], $"^%{userConfig.Root.Name}%")) {
          withOutDuplicates.RemoveAt(index);
          index = Math.Max(0, index - 1);
        } else {
          foreach (var program in programs.Select(x => x.FullName.Replace(@"\", @"\\"))) {
            if (!Regex.IsMatch(withOutDuplicates[index], program)) {
              continue;
            }

            withOutDuplicates.RemoveAt(index);
            index = Math.Max(0, index - 1);
          }
        }

        index++;
      }

      return withOutDuplicates;
    }
    // #pragma warning restore SYSLIB1045, S1144, IDE0051

    /// <summary>
    /// TODO: Add Summary
    /// </summary>
    /// <param name="path"></param>
    /// <param name="programs"></param>
    /// <returns></returns>
    internal override List<string> RemoveDuplicates(List<string> path, List<ProgramPath> programs) {
      List<string> withDupes = [.. path, .. programs.Select(x => x.FullName)];
      List<string> woDupes = [.. withDupes.Distinct()];

      List<string> cleanedPathsOne = this.LoopRemoveDuplicatesOne(woDupes, programs);
      List<string> cleanedPathsTwo = this.LoopRemoveDuplicatesTwo(cleanedPathsOne, programs);

      return cleanedPathsTwo;
    }

    /// <summary>
    /// Formats and expands strings in the path.
    /// </summary>
    /// <param name="programs"></param>
    /// <returns></returns>
    internal override List<string> DoFormatting(List<ProgramPath> programs) {
      if (Services.Config.GetUserConfigBySsid() is not UserConfig userConfig) {
        return [];
      }
      var tempPath = new List<string>();

      foreach (var program in programs.Select(x => x.FullName)) {
        if (VolumeRegex().IsMatch(program)) {
          tempPath.Add(program);
          Log.Additions("Adding FullName: ", program);
        } else {
          tempPath.Add($"%{userConfig.Root.Name}%\\{program}");
          Log.Additions("Adding FullName: ", $"%{userConfig.Root.Name}%\\{program}");
        }
      }

      return tempPath;
    }

    /// <summary>
    /// Updates the path with method two.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="newAux"></param>
    /// <returns></returns>
    internal override List<string> UpdatePathTwo(List<string> path, List<string> newAux) {
      if (Services.Config.GetUserConfigBySsid() is not UserConfig userConfig) {
        return [];
      }
      path.Remove($"%{userConfig.AuxListName}%");

      List<string> updatedPath = this.UpdatePathOne(path);

      Log.Info("New FullName Removals: ");
      for (var i = 0; i < updatedPath.Count; i++) {
        if (Regex.IsMatch(updatedPath[i], Regex.Escape(userConfig.Root.Path.FullName.Replace(@"[\/]", @"\\"))) || UPROG_DIR_Regex.IsMatch(updatedPath[i])) {
          Log.Additions("Removing FullName: ", updatedPath[i]);
          updatedPath.RemoveAt(i);
        } else {
          for (var j = 0; j < newAux.Count; j++) {
            if (j >= updatedPath.Count || i >= newAux.Count || updatedPath[i] != newAux[j]) {
              continue;
            }
            Log.Additions("Removing FullName: ", updatedPath[i]);
            updatedPath.RemoveAt(i);
          }
        }
      }

      Log.Info("New FullName Additions: ");

      foreach (var programPath in userConfig.ForceInPath.Select(x => x.FullName)) {
        if (VolumeRegex().IsMatch(programPath)) {
          updatedPath.Add(programPath);
          Log.Additions("Adding FullName: ", programPath);
        } else {
          updatedPath.Add($"%{userConfig.Root.Name}%\\{programPath}");
          Log.Additions("Adding FullName: ", $"%{userConfig.Root.Name}%\\{programPath}");
        }
      }

      updatedPath.Add($"%{userConfig.AuxListName}%");

      return updatedPath;
    }

    /// <summary>
    /// Updates the path with method one.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    internal override List<string> UpdatePathOne(List<string> path) {
      path.Remove($"%{Constants.UserProgramsList}%");
      if (Services.Config.GetUserConfigBySsid() is not { } userConfig) {
        return [];
      }
      List<ProgramPath> programs = userConfig.Programs;
      List<string> tempPath = RemoveDuplicates(path, programs);

      tempPath.Remove($"%{Constants.UserProgramsList}%");

      return tempPath;
    }
  }
}
