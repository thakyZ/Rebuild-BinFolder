using System.Diagnostics;
using System.Runtime.InteropServices;

using Rebuild_BinFolder.Configuration;
using Rebuild_BinFolder.Exceptions;
using Rebuild_BinFolder.Extensions;
using Rebuild_BinFolder.HandlePaths;

using static Rebuild_BinFolder.HandlePaths.Handler;

namespace Rebuild_BinFolder;

public class Program {
  public static string? AuxiliaryPathVariable {
    get;
    private set;
  } = string.Empty;
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

    // Singletons.Get<Arguments>().HandleArguments(args, out Dictionary<string, bool> output);\

    var notQuick = !Services.Arguments["isQuick"];

    if (!Services.Arguments["noHelp"]) {
      var runState = RunState.None;

      try {
        PathVariable = RegHandler.GetPathVariable();
        AuxiliaryPathVariable = RegHandler.GetAuxiliaryPathVariable();
      } catch (Exception exception) {
        Log.Error(exception, "Failed to get path variable.");
        Environment.Exit(1);
      }

      if (!Services.Arguments["admin"]) {
        runState |= RunState.User;
      } else {
        runState |= RunState.Admin;
        if (Services.Arguments["user"]) {
          runState |= RunState.User;
        }
      }

      if (PathVariable is null) {
        Log.Error("FullName variable returned null.");
        Environment.Exit(1);
      }

      var handlePath = new Handler(PathVariable, AuxiliaryPathVariable, runState);
      var (admin, user) = handlePath.Run();
      if (!Services.Arguments["admin"] && user is ReturnedData UserPaths) {
        Log.Changes("OldPath:", $"{UserPaths.OldPath.PathString}");
        Log.Changes("NewPath:", $"{UserPaths.NewPath.PathString}");
        if (UserPaths.OldPath.AuxiliaryPath is not null && UserPaths.NewPath?.AuxiliaryPath is not null) {
          Log.Changes("AuxPath:", $"{UserPaths.NewPath.AuxiliaryPathString}");
        } else if (UserPaths.NewPath.AuxiliaryPath is null) {
          throw new HandlePathNullException($"Aux path {(UserPaths.NewPath.AuxiliaryPath == null ? "is null" : "is not null")}, {(RegHandler.IsAdministrator ? "is Administrator" : "is not Administrator")}");
        }
        if (GetConfirmation(notQuick, Console.GetCursorPosition())) {
          RegHandler.SetPathVariable(UserPaths.NewPath.PathString, UserPaths.NewPath.AuxiliaryPathString);
        }
      }
      if (Services.Arguments["admin"] && admin is ReturnedData AdminPaths) {
        Log.Changes("OldPath:", $"{AdminPaths.OldPath.PathString}");
        Log.Changes("NewPath:", $"{AdminPaths.NewPath.PathString}");
        if (AdminPaths.OldPath.AuxiliaryPath is not null && AdminPaths.NewPath.AuxiliaryPath is not null) {
          Log.Changes("AuxPath:", $"{AdminPaths.NewPath.AuxiliaryPathString}");
          if (GetConfirmation(notQuick, Console.GetCursorPosition())) {
            RegHandler.SetPathVariable(AdminPaths.NewPath.PathString, AdminPaths.NewPath.AuxiliaryPathString);
          }
        } else if (AdminPaths.NewPath.AuxiliaryPath is null) {
          throw new HandlePathNullException($"Aux path {(AdminPaths.NewPath.AuxiliaryPath is null ? "is null" : "is not null")}, {(RegHandler.IsAdministrator ? "is Administrator" : "is not Administrator")}");
        }
        if (GetConfirmation(notQuick, Console.GetCursorPosition())) {
          RegHandler.SetPathVariable(UserPaths.NewPath.PathString, UserPaths.NewPath.AuxiliaryPathString);
        }
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
