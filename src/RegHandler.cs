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
      return Registry.CurrentUser.OpenSubKey(Constants.SubKey)?.GetValue("Path", "<NONE_MISSING>", RegistryValueOptions.DoNotExpandEnvironmentNames)?.ToString() ?? "<NONE_MISSING>";
    } else {
      return Registry.LocalMachine.OpenSubKey(string.Concat(@"SYSTEM\CurrentControlSet\Control\Session Manager\", Constants.SubKey))?.GetValue("Path", "<NONE_MISSING>", RegistryValueOptions.DoNotExpandEnvironmentNames)?.ToString() ?? "<NONE_MISSING>";
    }
  }

  internal static void SetPathVariable(string newPath, string? auxPath = "") {
    if (!IsAdministrator) {
      Registry.CurrentUser.CreateSubKey(Constants.SubKey)?.SetValue("Path", newPath, RegistryValueKind.ExpandString);
    } else {
      if (Registry.LocalMachine.OpenSubKey(string.Concat(@"SYSTEM\CurrentControlSet\Control\Session Manager\", Constants.SubKey))?.GetValue("Path") != null
          && Registry.LocalMachine.OpenSubKey(string.Concat(@"SYSTEM\CurrentControlSet\Control\Session Manager\", Constants.SubKey))?.GetValueKind("Path") != RegistryValueKind.ExpandString) {
        Registry.LocalMachine.CreateSubKey(string.Concat(@"SYSTEM\CurrentControlSet\Control\Session Manager\", Constants.SubKey))?.DeleteValue("Path");
      }

      Registry.LocalMachine.CreateSubKey(string.Concat(@"SYSTEM\CurrentControlSet\Control\Session Manager\", Constants.SubKey))?.SetValue("Path", newPath, RegistryValueKind.ExpandString);

      if (auxPath != null) {
        Registry.LocalMachine.CreateSubKey(string.Concat(@"SYSTEM\CurrentControlSet\Control\Session Manager\", Constants.SubKey))?.SetValue(Constants.AdminProgramsList, auxPath, RegistryValueKind.ExpandString);
      }

      Registry.LocalMachine.CreateSubKey(string.Concat(@"SYSTEM\CurrentControlSet\Control\Session Manager\", Constants.SubKey))?.SetValue(Constants.AdminProgramsDirectory,  Services.Config.AdminConfig.AdminRoot, RegistryValueKind.ExpandString);
    }
  }

  internal static Dictionary<string, string> GetStandardVariables() {
    Dictionary<string, string> output = new();
    string[] keys = IsAdministrator
      ? Registry.LocalMachine.OpenSubKey(string.Concat(@"SYSTEM\CurrentControlSet\Control\Session Manager\", Constants.SubKey))?.GetValueNames() ?? Array.Empty<string>()
      : Registry.CurrentUser.OpenSubKey(Constants.SubKey)?.GetValueNames() ?? Array.Empty<string>();

    foreach (string key in keys.ToList()) {
      if (string.IsNullOrEmpty(key))
        continue;

      if (!IsAdministrator) {
        string value = Registry.CurrentUser.OpenSubKey(Constants.SubKey)?.GetValue(key, "<NONE_MISSING>", RegistryValueOptions.None)?.ToString() ?? "<NONE_MISSING>";

        if (Path.IsPathFullyQualified(key) || Path.IsPathRooted(key)) {
          output.Add(key, value);
        }
      } else {
        string value = Registry.LocalMachine.OpenSubKey(string.Concat(@"SYSTEM\CurrentControlSet\Control\Session Manager\", Constants.SubKey))?.GetValue(key, "<NONE_MISSING>", RegistryValueOptions.None)?.ToString() ?? "<NONE_MISSING>";

        if (Path.IsPathFullyQualified(key) || Path.IsPathRooted(key)) {
          output.Add(key, value);
        }
      }
    }

    return output;
  }

  internal static Dictionary<Regex, string> GetStandardVariablesRegex() {
    Dictionary<Regex, string> output = new();
    string[] keys = IsAdministrator
      ? Registry.LocalMachine.OpenSubKey(string.Concat(@"SYSTEM\CurrentControlSet\Control\Session Manager\", Constants.SubKey))?.GetValueNames() ?? Array.Empty<string>()
      : Registry.CurrentUser.OpenSubKey(Constants.SubKey)?.GetValueNames() ?? Array.Empty<string>();

    foreach (string key in keys.ToList()) {
      if (string.IsNullOrEmpty(key))
        continue;

      if (!IsAdministrator) {
        string value = Registry.CurrentUser.OpenSubKey(Constants.SubKey)?.GetValue(key, "<NONE_MISSING>", RegistryValueOptions.None)?.ToString() ?? "<NONE_MISSING>";

        if (Path.IsPathFullyQualified(key) || Path.IsPathRooted(key)) {
          output.Add(new Regex(Regex.Escape(key)), value);
        }
      } else {
        string value = Registry.LocalMachine.OpenSubKey(string.Concat(@"SYSTEM\CurrentControlSet\Control\Session Manager\", Constants.SubKey))?.GetValue(key, "<NONE_MISSING>", RegistryValueOptions.None)?.ToString() ?? "<NONE_MISSING>";

        if (Path.IsPathFullyQualified(key) || Path.IsPathRooted(key)) {
          output.Add(new Regex(Regex.Escape(key)), value);
        }
      }
    }

    return output;
  }
}
