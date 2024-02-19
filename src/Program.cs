using System.Diagnostics;

using Rebuild_BinFolder.Configuration;
using Rebuild_BinFolder.Exceptions;
using Rebuild_BinFolder.Helpers;

namespace Rebuild_BinFolder;

public class Program {
  public static string? PathVariable {
    get;
    private set;
  } = string.Empty;

  protected Program() {
    Trace.WriteLine("Initializing Program class.");
  }

  public static void Startup() {
    try {
      var config = Config.Load(GetConfigPath);
      // Singletons.Register(config);
      _ = new Services(config);
    } catch (Exception exception) {
      Log.Error(exception, "Failed to load configuration file.");
      Environment.Exit(1);
    }

    // Singletons.Register(new Arguments());
    // Singletons.Register(new Log());
  }

  public static void Main(string[] args) {
    Log.ResetLogFile();

    try {
      Startup();
    } catch (Exception e) {
      Log.Error(e, "Settings were corrupted, resetting");
      // File.Delete(GetConfigPath);
      // Startup();
      Environment.Exit(1);
    }

    // Singletons.Get<Arguments>().HandleArguments(args, out Dictionary<string, bool> output);
    Services.Arguments.HandleArguments(args, out Dictionary<string, bool> output);

    var notQuick = !output["isQuick"];

    if (!output["noHelp"]) {
      try {
        PathVariable = RegHandler.GetPathVariable();
      } catch (Exception exception) {
        Log.Error(exception, "Failed to get path variable.");
        Environment.Exit(1);
      }

      if (PathVariable is null) {
        Log.Error("Path variable returned null.");
        Environment.Exit(1);
      }

      var handlePath = new HandlePaths(PathVariable, RegHandler.IsAdministrator);
      Log.Changes("OldPath:", handlePath.GetOldPath());
      Log.Changes("NewPath:", handlePath.GetNewPath());
      if (handlePath.GetAuxPath() != null && RegHandler.IsAdministrator) {
        Log.Changes("AuxPath:", $"{handlePath.GetAuxPath()}");
      } else if (handlePath.GetAuxPath() == null && RegHandler.IsAdministrator) {
        throw new HandlePathNullException($"Aux path {(handlePath.GetAuxPath() == null ? "is null" : "is not null")}, {(RegHandler.IsAdministrator ? "is Administrator" : "is not Administrator")}");
      }

      if (GetConfirmation(notQuick, Console.GetCursorPosition())) {
        RegHandler.SetPathVariable(handlePath.GetNewPath(), handlePath.GetAuxPath());
      }
    }
  }

  private static string GetConfigPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");

  private static bool GetConfirmation(bool check, (int left, int top) cursor) {
    var donePrompting = false;
    var confirmed = true;
    if (check) {
      while (!donePrompting) {
        const string message = "Do you want to replace these variables? [Y/n] ";

        Console.SetCursorPosition(cursor.left, cursor.top);
        Console.Write(new string(' ', Console.WindowWidth - message.Length));
        Console.SetCursorPosition(cursor.left, cursor.top);
        Console.Write(message);
        Console.SetCursorPosition(cursor.left + message.Length, cursor.top);

        string read = Console.ReadLine() ?? " ";

        if (read.Equals("Y", StringComparison.Ordinal)) {
          donePrompting = true;
          confirmed = true;
        } else if (read.Equals("n", StringComparison.OrdinalIgnoreCase)) {
          donePrompting = true;
          confirmed = false;
        } else {
          Console.SetCursorPosition(cursor.left, cursor.top);
        }
      }
    }
    return confirmed;
  }
}
