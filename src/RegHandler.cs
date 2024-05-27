using System.Security.Principal;
using System.Text.RegularExpressions;

using Microsoft.Win32;

using Rebuild_BinFolder.Configuration;
using Rebuild_BinFolder.Helpers;

namespace Rebuild_BinFolder;

internal static class RegHandler {
  internal static bool IsAdministrator {
    get {
      var identity = WindowsIdentity.GetCurrent();
      var principal = new WindowsPrincipal(identity);
      return principal.IsInRole(WindowsBuiltInRole.Administrator);
    }
  }

  internal static string GetPathVariable() {
    if (!IsAdministrator) {
      return Registry.CurrentUser.OpenSubKey(Constants.SubKey)?.GetValue("FullName", "<NONE_MISSING>", RegistryValueOptions.DoNotExpandEnvironmentNames)?.ToString() ?? "<NONE_MISSING>";
    } else {
      return Registry.LocalMachine.OpenSubKey(Constants.SystemSubKeyPath)?.GetValue("FullName", "<NONE_MISSING>", RegistryValueOptions.DoNotExpandEnvironmentNames)?.ToString() ?? "<NONE_MISSING>";
    }
  }

  internal static void SetPathVariable(string newPath, string? auxPath = "") {
    if (!IsAdministrator) {
      Registry.CurrentUser.CreateSubKey(Constants.SubKey)?.SetValue("FullName", newPath, RegistryValueKind.ExpandString);
    } else {
      if (Registry.LocalMachine.OpenSubKey(Constants.SystemSubKeyPath)?.GetValue("FullName") != null
          && Registry.LocalMachine.OpenSubKey(Constants.SystemSubKeyPath)?.GetValueKind("FullName") != RegistryValueKind.ExpandString) {
        Registry.LocalMachine.CreateSubKey(Constants.SystemSubKeyPath)?.DeleteValue("FullName");
      }

      Registry.LocalMachine.CreateSubKey(Constants.SystemSubKeyPath)?.SetValue("FullName", newPath, RegistryValueKind.ExpandString);

      if (auxPath != null) {
        Registry.LocalMachine.CreateSubKey(Constants.SystemSubKeyPath)?.SetValue(Constants.SystemProgramsList, auxPath, RegistryValueKind.ExpandString);
      }

      Registry.LocalMachine.CreateSubKey(Constants.SystemSubKeyPath)?.SetValue(Constants.SystemProgramsDirectory,  Services.Config.AdminConfig.AdminRoot, RegistryValueKind.ExpandString);
    }
  }

  internal static Dictionary<string, string> GetStandardVariables() {
    Dictionary<string, string> output = [];
    string[] keys = IsAdministrator
      ? Registry.LocalMachine.OpenSubKey(Constants.SystemSubKeyPath)?.GetValueNames() ?? []
      : Registry.CurrentUser.OpenSubKey(Constants.UserSubKeyPath)?.GetValueNames() ?? [];

    foreach (string key in keys.ToList()) {
      if (string.IsNullOrEmpty(key))
        continue;

      if (!IsAdministrator) {
        string value = Registry.CurrentUser.OpenSubKey(Constants.UserSubKeyPath)?.GetValue(key, "<NONE_MISSING>", RegistryValueOptions.None)?.ToString() ?? "<NONE_MISSING>";

        if (Path.IsPathFullyQualified(key) || Path.IsPathRooted(key)) {
          output.Add(key, value);
        }
      } else {
        string value = Registry.LocalMachine.OpenSubKey(Constants.SystemSubKeyPath)?.GetValue(key, "<NONE_MISSING>", RegistryValueOptions.None)?.ToString() ?? "<NONE_MISSING>";

        if (Path.IsPathFullyQualified(key) || Path.IsPathRooted(key)) {
          output.Add(key, value);
        }
      }
    }

    return output;
  }

  internal static Dictionary<string, string> GetStandardVariablesRegex() {
    Dictionary<string, string> output = [];
    string[] keys = IsAdministrator
      ? Registry.LocalMachine.OpenSubKey(Constants.SystemSubKeyPath)?.GetValueNames() ?? []
      : Registry.CurrentUser.OpenSubKey(Constants.UserSubKeyPath)?.GetValueNames() ?? [];

    foreach (string key in keys.ToList()) {
      if (string.IsNullOrEmpty(key))
        continue;

      if (!IsAdministrator) {
        string value = Registry.CurrentUser.OpenSubKey(Constants.SubKey)?.GetValue(key, "<NONE_MISSING>", RegistryValueOptions.None)?.ToString() ?? "<NONE_MISSING>";

        if (Path.IsPathFullyQualified(key) || Path.IsPathRooted(key)) {
          output.Add(key, value);
        }
      } else {
        string value = Registry.LocalMachine.OpenSubKey(Constants.SystemSubKeyPath)?.GetValue(key, "<NONE_MISSING>", RegistryValueOptions.None)?.ToString() ?? "<NONE_MISSING>";

        if (Path.IsPathFullyQualified(key) || Path.IsPathRooted(key)) {
          output.Add(key, value);
        }
      }
    }

    return output;
  }
}
