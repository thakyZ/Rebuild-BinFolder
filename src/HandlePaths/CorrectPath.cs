using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Win32;

using Newtonsoft.Json.Linq;

using Rebuild_BinFolder.Configuration;
using Rebuild_BinFolder.Extensions;
using Rebuild_BinFolder.Helpers;

using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Rebuild_BinFolder.HandlePaths;
internal static class CorrectPath {
  internal static List<ProgramPath> CorrectPaths() {
    List<ProgramPath> systemPathsListClone = [];
    List<ProgramPath> systemProgramListClone = [];

    foreach (var path in PathData.SystemPaths) {
      systemPathsListClone.Add(path);
    }

    foreach (var path in PathData.SystemProgramList) {
      systemProgramListClone.Add(path);
    }


    List<(int index, ProgramPath path)> toRemove = [];

    foreach ((int index, ProgramPath path) in systemPathsListClone.Select((x, i) => (i, x))) {
      if (systemPathsListClone.Count((x) => x == path) > 1) {
        Log.Debug($"Duplicate:                {path}");

        if (toRemove.Exists(x => x.path == path)) {
          toRemove.Add((index, path));
        }
      }

      if (!Directory.Exists(path.FullName)) {
        Log.Debug($"Non-Existent:             {path}");

        if (!toRemove.TrueForAll(x => x.path == path)) {
          toRemove.Add((index, path));
        }
      }

      if (systemProgramListClone.Exists(x => x == path)) {
        Log.Debug($"Duplicate in APROG_LIST: {path}");

        if (!toRemove.TrueForAll(x => x.path == path)) {
          toRemove.Add((index, path));
        }
      }
    }

    var newToRemove = toRemove.OrderBy(x => x.index);

    int oldOffsetIndex = 0;

    int limit = toRemove.Count * -1;

    foreach ((int index, ProgramPath path) in newToRemove) {

      if (oldOffsetIndex < limit) {
        throw new ArithmeticException($"Variable oldOffsetIndex didn't get properly set outside loop, is at: {oldOffsetIndex}");
      }

      Log.Verbose($"{path.FullName} {path.FullName} {oldOffsetIndex}");
      oldOffsetIndex -= 1;
    }

    int offsetIndex = 0;

    foreach ((int index, ProgramPath path) in newToRemove) {
      if (offsetIndex < limit) {
        throw new ArithmeticException($"Variable offsetIndex didn't get properly set outside loop, is at: {offsetIndex}");
      }

      var offset = index + offsetIndex;
      Log.Verbose($"Removing at index {offset}/{systemPathsListClone.Count} (Offset by {offsetIndex})");
      systemPathsListClone.RemoveAt(offset);
      offsetIndex -= 1;
    }

    DisposeDictionaries(toRemove, newToRemove, oldOffsetIndex, offsetIndex);

    Log.Object(systemPathsListClone);

    return systemPathsListClone;
  }

  private static void DisposeDictionaries([In] params object[] items) {
    foreach (var item in items) {
      if (item is IDictionary dictionary) {
        foreach (var index in dictionary) {
          dictionary.Remove(index);
        }
      }
    }
  }
}
