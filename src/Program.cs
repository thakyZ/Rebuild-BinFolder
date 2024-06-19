using System.Diagnostics;

using Rebuild_BinFolder.Configuration;
using Rebuild_BinFolder.Exceptions;
using Rebuild_BinFolder.HandlePaths;

namespace Rebuild_BinFolder;

public class Program {
  public static string? AuxiliaryPathVariable {
    get;
    private set;
  }
  public static string? PathVariable {
    get;
    private set;
  }

  protected Program() {
    Debug.WriteLine("Initializing Program class.");
  }

  public static void Main(string[] args) {
    Log.ResetLogFile();
    try {
      var config = Config.Load(GetConfigPath);
      Services.Init(config, args);
    } catch (Exception exception) {
      Log.Error(exception, "Failed to load configuration file.");
      Environment.Exit(1);
    }

    var notQuick = !Services.Arguments["isQuick"];

    if (!Services.Arguments["noHelp"]) {

      try {
        PathVariable = RegHandler.GetPathVariable();
        AuxiliaryPathVariable = RegHandler.GetAuxiliaryPathVariable();
      } catch (Exception exception) {
        Log.Error(exception, "Failed to get path variable.");
        Environment.Exit(1);
      }

      if (PathVariable is null) {
        Log.Error("FullName variable returned null.");
        Environment.Exit(1);
      }

      var handlePath = new Handler(PathVariable, AuxiliaryPathVariable);
      var (admin, user) = handlePath.Run();
      if (!Services.Arguments["admin"] && user is ReturnedData UserPaths) {
        Log.Changes("OldPath:", $"{UserPaths.OldPath.PathString}");
        Log.Changes("NewPath:", $"{UserPaths.NewPath.PathString}");
        if (UserPaths.OldPath.AuxiliaryPath is not null && UserPaths.NewPath?.AuxiliaryPath is not null) {
          Log.Changes("AuxPath:", $"{UserPaths.NewPath.AuxiliaryPathString}");
        } else if (UserPaths.NewPath is not null && UserPaths.NewPath.AuxiliaryPath is null) {
          throw new HandlePathNullException($"Aux path {(UserPaths.NewPath.AuxiliaryPath == null ? "is null" : "is not null")}, {(RegHandler.IsAdministrator ? "is Administrator" : "is not Administrator")}");
        }
        if (GetConfirmation(notQuick, Console.GetCursorPosition()) && UserPaths.NewPath is not null && UserPaths.NewPath.PathString is not null) {
          RegHandler.SetPathVariable(UserPaths.NewPath.PathString, UserPaths.NewPath.AuxiliaryPathString);
        }
      }
      if (Services.Arguments["admin"] && admin is ReturnedData AdminPaths) {
        Log.Changes("OldPath:", $"{AdminPaths.OldPath.PathString}");
        Log.Changes("NewPath:", $"{AdminPaths.NewPath.PathString}");
        if (AdminPaths.OldPath.AuxiliaryPath is not null && AdminPaths.NewPath.AuxiliaryPath is not null && AdminPaths.NewPath.PathString is not null) {
          Log.Changes("AuxPath:", $"{AdminPaths.NewPath.AuxiliaryPathString}");
          if (GetConfirmation(notQuick, Console.GetCursorPosition())) {
            RegHandler.SetPathVariable(AdminPaths.NewPath.PathString, AdminPaths.NewPath.AuxiliaryPathString);
          }
        } else if (AdminPaths.NewPath.AuxiliaryPath is null) {
          throw new HandlePathNullException($"Aux path {(AdminPaths.NewPath.AuxiliaryPath is null ? "is null" : "is not null")}, {(RegHandler.IsAdministrator ? "is Administrator" : "is not Administrator")}");
        }
        if (GetConfirmation(notQuick, Console.GetCursorPosition()) && AdminPaths.NewPath.PathString is not null) {
          RegHandler.SetPathVariable(AdminPaths.NewPath.PathString, AdminPaths.NewPath.AuxiliaryPathString);
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
