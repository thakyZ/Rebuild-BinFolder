using System.Diagnostics;

using Rebuild_BinFolder.Configuration;
using Rebuild_BinFolder.Exceptions;
using Rebuild_BinFolder.HandlePaths;
using Rebuild_BinFolder.Helpers;

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

    var notQuick = !Arguments.GetArgument<bool>("isQuick").Value;

    if (Arguments.GetArgument<bool>("noHelp").Value) {
      return;
    }

    try {
      PathVariable = RegHandler.GetPathVariable();
      AuxiliaryPathVariable = RegHandler.GetAuxiliaryPathVariable();
    } catch (Exception exception) {
      Log.Error(exception, "Failed to get path variable.");
      Environment.Exit(1);
    }

    var handlePath = new Handler(PathVariable, AuxiliaryPathVariable);
    (ReturnedData? admin, ReturnedData? user) = handlePath.Run();
    if ((Services.RunState != Handler.RunState.User || Services.RunState == Handler.RunState.Both) && user is not null) {
      Log.Changes("OldPath:", $"{user.OldPath.PathString}");
      Log.Changes("NewPath:", $"{user.NewPath.PathString}");
      if (user.NewPath?.AuxiliaryPath is not null) {
        Log.Changes("AuxPath:", $"{user.NewPath.AuxiliaryPathString}");
      } else if (user.NewPath is not null && user.NewPath.AuxiliaryPath is null) {
        throw new HandlePathNullException($"Aux path {(user.NewPath.AuxiliaryPath == null ? "is null" : "is not null")}, {(RegHandler.IsAdministrator ? "is Administrator" : "is not Administrator")}");
      }
      if (GetConfirmation(notQuick, Console.GetCursorPosition()) && user.NewPath?.PathString is not null) {
        RegHandler.SetPathVariable(user.NewPath.PathString, user.NewPath.AuxiliaryPathString);
      }
    }
    // ReSharper disable once InvertIf
    if ((Services.RunState == Handler.RunState.Admin || Services.RunState == Handler.RunState.Both) && admin is not null) {
      Log.Changes("OldPath:", $"{admin.OldPath.PathString}");
      Log.Changes("NewPath:", $"{admin.NewPath.PathString}");
      if (admin.NewPath.PathString is not null) {
        Log.Changes("AuxPath:", $"{admin.NewPath.AuxiliaryPathString}");
        if (GetConfirmation(notQuick, Console.GetCursorPosition())) {
          RegHandler.SetPathVariable(admin.NewPath.PathString, admin.NewPath.AuxiliaryPathString);
        }
      } else if (admin.NewPath.AuxiliaryPath is null) {
        throw new HandlePathNullException($"Aux path {(admin.NewPath.AuxiliaryPath is null ? "is null" : "is not null")}, {(RegHandler.IsAdministrator ? "is Administrator" : "is not Administrator")}");
      }
      if (GetConfirmation(notQuick, Console.GetCursorPosition()) && admin.NewPath.PathString is not null) {
        RegHandler.SetPathVariable(admin.NewPath.PathString, admin.NewPath.AuxiliaryPathString);
      }
    }
  }

  private static string GetConfigPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");

  private static bool GetConfirmation(bool check, (int left, int top) cursor) {
    var donePrompting = false;
    var confirmed = true;
    if (!check) {
      return confirmed;
    }

    while (!donePrompting) {
      const string message = "Do you want to replace these variables? [Y/n] ";

      Console.SetCursorPosition(cursor.left, cursor.top);
      Console.Write(new string(' ', Console.WindowWidth - message.Length));
      Console.SetCursorPosition(cursor.left, cursor.top);
      Console.Write(message);
      Console.SetCursorPosition(cursor.left + message.Length, cursor.top);

      var read = Console.ReadLine() ?? " ";

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
    return confirmed;
  }
}
