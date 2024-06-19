using System.Security.Principal;
using System.Text.RegularExpressions;

using Microsoft.Win32;

using Rebuild_BinFolder.Configuration;
using Rebuild_BinFolder.HandlePaths;
using Rebuild_BinFolder.Helpers;

namespace Rebuild_BinFolder;

internal static class RegHandler {
  internal static bool IsAdministrator {
    get {
      if (Services.RunState == Handler.RunState.Admin) {
        var identity = WindowsIdentity.GetCurrent();
        var principal = new WindowsPrincipal(identity);
        return principal.IsInRole(WindowsBuiltInRole.Administrator);
      } else {
        return false;
      }
    }
  }

  internal static string GetPathVariable() {
    if (!IsAdministrator) {
      return Registry.CurrentUser.OpenSubKey(Constants.SubKey)?.GetValue("Path", "<NONE_MISSING>", RegistryValueOptions.DoNotExpandEnvironmentNames)?.ToString() ?? "<NONE_MISSING>";
    } else {
      return Registry.LocalMachine.OpenSubKey(Constants.SystemSubKeyPath)?.GetValue("Path", "<NONE_MISSING>", RegistryValueOptions.DoNotExpandEnvironmentNames)?.ToString() ?? "<NONE_MISSING>";
    }
  }

  internal static void SetPathVariable(string newPath, string? auxPath = "") {
    if (!IsAdministrator) {
      Registry.CurrentUser.CreateSubKey(Constants.SubKey)?.SetValue("Path", newPath, RegistryValueKind.ExpandString);
    } else {
      if (Registry.LocalMachine.OpenSubKey(Constants.SystemSubKeyPath)?.GetValue("Path") != null
          && Registry.LocalMachine.OpenSubKey(Constants.SystemSubKeyPath)?.GetValueKind("Path") != RegistryValueKind.ExpandString) {
        Registry.LocalMachine.CreateSubKey(Constants.SystemSubKeyPath)?.DeleteValue("Path");
      }

      Registry.LocalMachine.CreateSubKey(Constants.SystemSubKeyPath)?.SetValue("Path", newPath, RegistryValueKind.ExpandString);

      if (auxPath != null) {
        Registry.LocalMachine.CreateSubKey(Constants.SystemSubKeyPath)?.SetValue(Constants.SystemProgramsList, auxPath, RegistryValueKind.ExpandString);
      }

      Registry.LocalMachine.CreateSubKey(Constants.SystemSubKeyPath)?.SetValue(Constants.SystemProgramsDirectory,  Services.Config.AdminConfig.Root, RegistryValueKind.ExpandString);
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

  internal static string GetAuxiliaryPathVariable() {
    if (!IsAdministrator) {
      return Registry.CurrentUser.OpenSubKey(Constants.SubKey)?.GetValue(Constants.UserProgramsList, "<NONE_MISSING>", RegistryValueOptions.DoNotExpandEnvironmentNames)?.ToString() ?? "<NONE_MISSING>";
    } else {
      return Registry.LocalMachine.OpenSubKey(Constants.SystemSubKeyPath)?.GetValue(Constants.SystemProgramsList, "<NONE_MISSING>", RegistryValueOptions.DoNotExpandEnvironmentNames)?.ToString() ?? "<NONE_MISSING>";
    }
  }
}
