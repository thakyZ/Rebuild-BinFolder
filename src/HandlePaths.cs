// Ignore Spelling: Admin

using System.Text.RegularExpressions;

using Rebuild_BinFolder.Configuration;
using Rebuild_BinFolder.Extensions;
using Rebuild_BinFolder.Helpers;

namespace Rebuild_BinFolder;

public partial class HandlePaths {
  private List<string> NewPath {
    get; set;
  }

  private List<string> OldPath {
    get; set;
  }

  private List<string> AuxPath {
    get; set;
  } = new();

  public string GetNewPath() => string.Join(';', NewPath);
  public string GetOldPath() => string.Join(';', OldPath);
  public string? GetAuxPath() => AuxPath.Count > 0 ? string.Join(';', AuxPath) : null;

  private Dictionary<Regex, string> Equivalents { get; set; }

  public HandlePaths(string path, bool isAdmin) {
    // Equivalents = Singletons.Get<Config>().Equivalents.GetDictionary();
    Equivalents = Services.Config.Equivalents.GetDictionary();
    Equivalents = Equivalents.Concat(RegHandler.GetStandardVariablesRegex()).ToDictionary(x => x.Key, x => x.Value);
    OldPath = path.Split(';').ToList();
    OldPath.RemoveAll(x => string.IsNullOrEmpty(x) || string.IsNullOrWhiteSpace(x));

    if (isAdmin) {
      AuxPath = DoFormatting(isAdmin);
      NewPath = UpdatePathTwo(OldPath, isAdmin, AuxPath);
    } else {
      NewPath = UpdatePath(OldPath, isAdmin);
    }
  }

  private List<string> TransformEquivalent(List<string> withDuplicates) {
    Log.Debug("TransformEquivalent:");
    var output = withDuplicates.ConvertAll(x => {
      foreach ((Regex key, string value) in Equivalents) {
        Log.Debug("key: " + key + " | " + value);
        Regex newKeyFromString = key;
        if (key.ToString().Contains("<userDir>")) {
          Log.Debug("Contains: <userDir>");
          newKeyFromString = new Regex(key.ToString().Replace("<userDir>", Regex.Escape(@$"C:\\Users\\{Environment.UserName}")));
        }

        if (newKeyFromString.IsMatch(x)) {
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

  private List<string> LoopRemoveDuplicatesOne(List<string> withDuplicates, List<ProgramPath> programs, bool isAdmin) {
    var withOutDuplicates = TransformEquivalent(withDuplicates);

    Log.Info("LoopRemoveDuplicates1:");
    var output = withOutDuplicates.FindAll(x => {
      var yz = false;
      var yr = false;
      if (!isAdmin || (!AProgRegex().IsMatch(x) && !Regex.IsMatch(x, Regex.Escape(Services.Config.AdminConfig.AdminRoot.Path)))) {
        yr = true;
        if (!programs.Exists(y => y.Path == x)) {
          yz = true;
          if (!programs.Exists(y => Regex.IsMatch(x, Regex.Escape(y.Path)))) {
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

  #pragma warning disable SYSLIB1045, S1144, IDE0051
  private List<string> LoopRemoveDuplicatesTwo(List<string> withDuplicates, List<ProgramPath> programs, bool isAdmin) {
    var withOutDuplicates = withDuplicates.Distinct().ToList();

    foreach ((Regex key, string value) in Equivalents) {
      Regex newKeyFromString = key;

      if (key.ToString().Contains("<userDir>")) {
        newKeyFromString = new Regex(key.ToString().Replace("<userDir>", Regex.Escape(@$"C:\\Users\\{Environment.UserName}")));
      }

      withOutDuplicates = withOutDuplicates.ConvertAll(x => {
        if (newKeyFromString.IsMatch(x)) {
          return newKeyFromString.Replace(x, value);
        }
        return x;
      });

      withOutDuplicates = withOutDuplicates.Distinct().ToList();
    }

    if (isAdmin) {
      for (int index = 0; index < withOutDuplicates.Count; index++) {
        var replacedValueOne = Services.Config.AdminConfig.AdminRoot.Path.Replace(@"\", @"\\");

        if (Regex.IsMatch(withOutDuplicates[index], $"^{replacedValueOne}\\\\") || Regex.IsMatch(withOutDuplicates[index], $"^%{Constants.AdminProgramsDirectory}%")) {
          withOutDuplicates.RemoveAt(index);
          index = index - 1 < 0 ? 0 : index - 1;
        } else {
          foreach (ProgramPath program in programs) {
            var replacedValueTwo = program.Path.Replace(@"\", @"\\");

            if (Regex.IsMatch(withOutDuplicates[index], replacedValueTwo)) {
              withOutDuplicates.RemoveAt(index);
              index = index - 1 < 0 ? 0 : index - 1;
            }
          }
        }
      }
    }

    return withOutDuplicates;
  }
  #pragma warning restore SYSLIB1045, S1144, IDE0051

  private List<string> RemoveDuplicates(List<string> path, List<ProgramPath> programs, bool isAdmin) {
    List<string> withDupes = isAdmin ? path : path.Concat(programs.Select(x => x.Path)).ToList();
    var woDupes = withDupes.Distinct().ToList();

  #pragma warning disable S1481
    var output = LoopRemoveDuplicatesOne(woDupes, programs, isAdmin);
  #pragma warning restore S1481
    // var output = LoopRemoveDuplicates2(woDupes, programs, isAdmin);

    return woDupes;
  }

  private static List<string> DoFormatting(bool isAdmin) {
    var programs = isAdmin ?  Services.Config.AdminConfig.AdminPrograms :  Services.Config.GetUserConfigBySID().UserPrograms;
    var _tempPath = new List<string>();

    foreach (var program in programs.Select(x => x.Path)) {
      if (VolumeRegex().IsMatch(program)) {
        _tempPath.Add(program);
        Log.Additions("Adding Path: ", program);
      } else if (isAdmin) {
        _tempPath.Add($"%{Constants.AdminProgramsDirectory}%\\{program}");
        Log.Additions("Adding Path: ", $"%{Constants.AdminProgramsDirectory}%\\{program}");
      }
    }

    return _tempPath;
  }

  private List<string> UpdatePathTwo(List<string> path, bool isAdmin, List<string> newAux) {
    if (isAdmin) {
      _ = path.Remove($"%{Constants.AdminProgramsList}%");
    }

    var updatedPath = UpdatePath(path, isAdmin);

    Log.Info("New Path Removals: ");
    for (int i = 0; i < updatedPath.Count; i++) {
      if (Regex.IsMatch(updatedPath[i], Regex.Escape( Services.Config.AdminConfig.AdminRoot.Path.Replace(@"[\/]", @"\\"))) || AProgRegex().IsMatch(updatedPath[i])) {
        Log.Additions("Removing Path: ", updatedPath[i]);
        updatedPath.RemoveAt(i);
      } else {
        for (int j = 0; j < newAux.Count; j++) {
          if (j < updatedPath.Count && i < newAux.Count && updatedPath[i] == newAux[j]) {
            Log.Additions("Removing Path: ", updatedPath[i]);
            updatedPath.RemoveAt(i);
          }
        }
      }
    }

    Log.Info("New Path Additions: ");
    foreach (var programPath in  Services.Config.AdminConfig.ForceInAdminPath.Select(x => x.Path)) {
      if (VolumeRegex().IsMatch(programPath)) {
        updatedPath.Add(programPath);
        Log.Additions("Adding Path: ", programPath);
      } else {
        if (isAdmin) {
          updatedPath.Add($"%{Constants.AdminProgramsDirectory}%\\{programPath}");
          Log.Additions("Adding Path: ", $"%{Constants.AdminProgramsDirectory}%\\{programPath}");
        }
      }
    }

    updatedPath.Add($"%{Constants.AdminProgramsList}%");

    return updatedPath;
  }

  private List<string> UpdatePath(List<string> path, bool isAdmin) {
    if (isAdmin) {
      _ = path.Remove($"%{Constants.AdminProgramsList}%");
    }

    var programs = isAdmin ? Services.Config.AdminConfig.AdminPrograms : Services.Config.GetUserConfigBySID().UserPrograms;
    var tempPath = RemoveDuplicates(path, programs, isAdmin);

    if (isAdmin) {
      _ = tempPath.Remove($"%{Constants.AdminProgramsList}%");
    }

    return tempPath;
  }

  [GeneratedRegex($@"^%{Constants.AdminProgramsDirectory}%")]
  private static partial Regex AProgRegex();

  [GeneratedRegex("[A-Z]:\\\\")]
  private static partial Regex VolumeRegex();
}
