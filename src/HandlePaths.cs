using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Microsoft.VisualBasic;

using Newtonsoft.Json.Linq;

namespace Rebuild_BinFolder {
  public class HandlePaths {
    public string NewPath {
      get; private set;
    }
    public string OldPath {
      get; private set;
    }
    public string AuxPath {
      get; private set;
    }
    public HandlePaths(string path, bool isAdmin) {
      OldPath = path;
      if (isAdmin) {
        AuxPath = DoFormatting(isAdmin);
        NewPath = UpdatePathTwo(path, isAdmin, AuxPath);
      } else {
        AuxPath = "";
        NewPath = $"{UpdatePath(path, isAdmin)}";
      }
    }

    private static string RemoveDuplicates(string path, List<string> programs, bool isAdmin) {
      var pathArray = path.Split(";").ToList();
      List<string> withDupes = isAdmin ? pathArray : $"{path};{string.Join(';', programs)}".Split(";").ToList();
      var woDupes = withDupes.Distinct().ToList();
      foreach ((string key, string value) in Program.config.Equivelents) {
        var newKey = key;
        if (Regex.IsMatch(key, @"^<userDir>")) {
          newKey = Regex.Replace(newKey, @"^<userDir>$", @"^C:\\Users\\[\w]+(?!(\\AppData))");
          newKey = Regex.Replace(newKey, @"^<userDir>", @"^C:\\Users\\[\w]+");
        }
        var _newKey = new Regex(newKey);
        for (int i = 1; i < woDupes.Count; i++) {
          if (_newKey.IsMatch(woDupes[i])) {
            woDupes[i] = _newKey.Replace(woDupes[i], value);
          }
        }
        woDupes = woDupes.Distinct().ToList();
      }
      if (isAdmin) {
        for (int i = 0; i < woDupes.Count; i++) {
          var replaced2 = Program.config.GlobalRoot.Replace(@"\", @"\\");
          if (Regex.IsMatch(woDupes[i], $"^{replaced2}\\\\")) {
            woDupes.RemoveAt(i);
            i = i - 1 < 0 ? 0 : i - 1;
          } else if (Regex.IsMatch(woDupes[i], $"^%APROG_DIR%")) {
            woDupes.RemoveAt(i);
            i = i - 1 < 0 ? 0 : i - 1;
          } else {
            foreach (string program in programs) {
              var replaced = program.Replace(@"\", @"\\");
              if (Regex.IsMatch(woDupes[i], replaced)) {
                woDupes.RemoveAt(i);
                i = i - 1 < 0 ? 0 : i - 1;
              }
            }
          }
        }
      }
      return string.Join(";",woDupes);
    }

    private static string DoFormatting(bool isAdmin) {
      var programs = isAdmin ? Program.config.GlobalPrograms : Program.config.LocalPrograms;
      var _tempPath = "";
      foreach (string program in programs) {
        var tempProgram = program;
        if (Regex.IsMatch(tempProgram, @"[A-Z]:\\")) {
          tempProgram = $"{program}";
        } else {
          if (isAdmin) {
            tempProgram = $"%APROG_DIR%\\{program}";
          }
        }
        _tempPath += $"{tempProgram};";
        Log.Debug($"Adding Path: [{tempProgram}]");
      }
      return _tempPath;
    }

    private static string UpdatePathTwo(string path, bool isAdmin, string newAux) {
      var updatedPath = UpdatePath(path, isAdmin).Split(";").ToList();
      var auxList = newAux.Split(";").ToList();
      for (int i = 0; i < updatedPath.Count; i++) {
        if (Regex.IsMatch(updatedPath[i], Program.config.GlobalRoot.Replace(@"\", @"\\"))) {
          updatedPath.RemoveAt(i);
        } else if (Regex.IsMatch(updatedPath[i], @"^%APROG_DIR%")) {
          updatedPath.RemoveAt(i);
        } else {
          for (int j = 0; j < auxList.Count; j++) {
            if (updatedPath[i] == auxList[j]) {
              updatedPath.RemoveAt(i);
            }
          }
        }
      }
      if (isAdmin) {
        _ = updatedPath.Remove("%APROG_LIST%");
      }
      var tempPath = string.Join(";", updatedPath);
      if (!tempPath[tempPath.Length - 1].ToString().Equals(";")) {
        tempPath = $"{tempPath};";
      }
      return isAdmin ? $"{tempPath}%APROG_LIST%;" : tempPath;
    }

    private static string UpdatePath(string path, bool isAdmin) {
      var programs = isAdmin ? Program.config.GlobalPrograms : Program.config.LocalPrograms;
      var tempPath = RemoveDuplicates(path, programs, isAdmin);
      var pathArray = tempPath.Split(";").ToList();
      if (isAdmin) {
        _ = pathArray.Remove("%APROG_LIST%");
      }
      tempPath = string.Join(";", pathArray);
      if (!tempPath[tempPath.Length - 1].Equals(";")) {
        tempPath = $"{tempPath};";
      }
      return isAdmin ? $"{tempPath}%APROG_LIST%;" : tempPath;
    }
  }
}
