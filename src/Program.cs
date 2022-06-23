using System;
using System.Diagnostics.Metrics;
using System.Diagnostics.Tracing;
using System.IO;
using System.Security.Principal;

using Microsoft.Win32;

using static System.Collections.Specialized.BitVector32;

namespace Rebuild_BinFolder {
  public class Program {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    internal static Config config;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    private static bool IsAdministrator {
      get {
        var identity = WindowsIdentity.GetCurrent();
        var principal = new WindowsPrincipal(identity);
        return principal.IsInRole(WindowsBuiltInRole.Administrator);
      }
    }

#pragma warning disable CS8603 // Possible null reference return.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0046:Convert to conditional expression", Justification = "<Pending>")]
    private static string GetPathVariable() {
      string subKey = "Environment";
      if (!IsAdministrator) {
        return Registry.CurrentUser.OpenSubKey(subKey)?.GetValue("Path", "<NONE_MISSING>", RegistryValueOptions.DoNotExpandEnvironmentNames).ToString();
      } else {
        return Registry.LocalMachine.OpenSubKey($"SYSTEM\\CurrentControlSet\\Control\\Session Manager\\{subKey}")?.GetValue("Path", "<NONE_MISSING>", RegistryValueOptions.DoNotExpandEnvironmentNames).ToString();
      }
    }
#pragma warning restore CS8603 // Possible null reference return.
#pragma warning restore CS8602 // Dereference of a possibly null reference.

    private static void SetPathVariable(string newPath, string auxPath = "") {
      string subKey = "Environment";
      if (!IsAdministrator) {
        Registry.CurrentUser.CreateSubKey(subKey)?.SetValue("Path", newPath, RegistryValueKind.ExpandString);
      } else {
        Registry.CurrentUser.CreateSubKey($"SYSTEM\\CurrentControlSet\\Control\\Session Manager\\{subKey}")?.SetValue("Path", newPath, RegistryValueKind.ExpandString);
        Registry.CurrentUser.CreateSubKey($"SYSTEM\\CurrentControlSet\\Control\\Session Manager\\{subKey}")?.SetValue("ARPOG_LIST", auxPath, RegistryValueKind.ExpandString);
        Registry.CurrentUser.CreateSubKey($"SYSTEM\\CurrentControlSet\\Control\\Session Manager\\{subKey}")?.SetValue("ARPOG_DIR", config.GlobalRoot, RegistryValueKind.ExpandString);
      }
    }

    public static string PathVariable {
      get;
      private set;
    } = "";

    public static string HelpMessage {
      get;
    } = "<Help Message>";

    public static void Main(string[] args) {
      Log.ResetLogFile();
      try {
        SetupSettings();
      } catch (Exception e) {
        Log.Error(e, "Settings were corrupted, resetting");
        File.Delete(GetConfigPath);
        SetupSettings();
      }
      var noHelp = false;
      var notQuick = true;
      if (args.Length > 0) {
        for (int i = 0; i < args.Length; i++) {
          if (string.Equals(args[i], "-help", StringComparison.OrdinalIgnoreCase) || string.Equals(args[i], "--help", StringComparison.OrdinalIgnoreCase) || string.Equals(args[i], "-?", StringComparison.OrdinalIgnoreCase) || string.Equals(args[i], "/?", StringComparison.OrdinalIgnoreCase)) {
            Log.Info(HelpMessage);
            noHelp = true;
          }
          if (string.Equals(args[i], "-quick", StringComparison.OrdinalIgnoreCase) || string.Equals(args[i], "--quick", StringComparison.OrdinalIgnoreCase)) {
            notQuick = false;
          }
        }
      }
      if (!noHelp) {
        PathVariable = GetPathVariable();
        var handlePath = new HandlePaths(PathVariable, IsAdministrator);
        Log.Info($"OldPath: {handlePath.OldPath}");
        Log.Info($"NewPath: {handlePath.NewPath}");
        Log.Info($"AuxPath: {handlePath.AuxPath}");
        var confirmed = GetConfirmation(notQuick, Console.GetCursorPosition());
        if (confirmed) {
          SetPathVariable(handlePath.NewPath, handlePath.AuxPath);
        }
      }
    }
    private static void SetupSettings() => config = Config.Load(GetConfigPath);
    private static string GetConfigPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"config.json");

    private static bool GetConfirmation(bool check, (int left, int top) cursor) {
      var donePrompting = false;
      var confirmed = true;
      if (check) {
        do {
          var message = $"Do you want to replace these variables? [Y/n] ";
          Console.SetCursorPosition(cursor.left, cursor.top);
          Console.Write(new string(' ', Console.WindowWidth));
          Console.SetCursorPosition(cursor.left, cursor.top);
          Console.WriteLine(message);
          Console.SetCursorPosition(cursor.left + message.Length, cursor.top);
          var read = Convert.ToChar(Console.Read()).ToString();
          switch (read) {
            case "Y":
              donePrompting = true;
              confirmed = true;
              break;
            case "n":
            case "N":
              donePrompting = true;
              confirmed = false;
              break;
            default:
              break;
          }
        } while (!donePrompting);
      }
      return confirmed;
    }
  }
}
