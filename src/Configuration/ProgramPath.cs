#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

using Rebuild_BinFolder.Helpers;

namespace Rebuild_BinFolder.Configuration;

public sealed partial class ProgramPath : IEquatable<ProgramPath> {
  /// <summary>
  /// TODO: Add field summary.
  /// </summary>
  private string _rawFullName;

  /// <summary>
  /// TODO: Add property summary.
  /// </summary>
  internal string FullName => Normalize(_rawFullName);

  /// <summary>
  /// TODO: Add property summary.
  /// </summary>
  internal string RawFullName => this._rawFullName;

  /// <summary>
  /// TODO: Add property summary.
  /// </summary>
  internal bool Exists => Directory.Exists(this.FullName);

  /// <summary>
  /// TODO: Add property summary.
  /// </summary>
  internal DirectoryInfo GetDirectoryInfo => new(this.FullName);

  /// <summary>
  /// TODO: Add method summary.
  /// </summary>
  /// <param name="path"></param>
  public ProgramPath(string path) {
    this._rawFullName = path;
  }

  /// <summary>
  /// TODO: Add method summary.
  /// </summary>
  /// <param name="str"></param>
  /// <returns></returns>
  public bool EqualsRaw(string? str) {
    if (str is null) return false;
    return str == RawFullName;
  }

  /// <summary>
  /// TODO: Add method summary.
  /// </summary>
  /// <param name="str"></param>
  /// <returns></returns>
  public bool Equals(string? str) {
    if (str is null) return false;
    return str == FullName;
  }

  /// <summary>
  /// TODO: Add method summary.
  /// </summary>
  /// <param name="path"></param>
  /// <returns></returns>
  public bool Equals(ProgramPath? path) {
    if (path is null) return false;
    return path.RawFullName == RawFullName && path.FullName == FullName;
  }

  /// <summary>
  /// TODO: Add method summary.
  /// </summary>
  /// <param name="obj"></param>
  /// <returns></returns>
  public override bool Equals(object? obj) {
    if (obj is null) return false;
    if (obj is string str) if(Equals(str)) return true; else if (EqualsRaw(str))  return true;
    if (obj is ProgramPath path) return Equals(path);
    return false;
  }

  /// <summary>
  /// TODO: Add method summary.
  /// </summary>
  /// <returns></returns>
  public override int GetHashCode() {
    return HashCode.Combine(FullName.GetHashCode(), RawFullName.GetHashCode(), Exists.GetHashCode());
  }

  /// <summary>
  /// TODO: Add property summary.
  /// </summary>
  internal static ProgramPath Empty => new("");

  /// <summary>
  /// TODO: Add property summary.
  /// </summary>
  internal static ProgramPath DefaultSystem => new(Constants.Default.SystemProgramsDirectoryValue);

  /// <summary>
  /// TODO: Add property summary.
  /// </summary>
  internal static ProgramPath DefaultUser => new(Constants.Default.UserProgramsDirectoryValue);

  /// <summary>
  /// TODO: Add method summary.
  /// </summary>
  /// <returns></returns>
  internal string ToRawString() => this._rawFullName;

  /// <summary>
  /// TODO: Add method summary.
  /// </summary>
  /// <returns></returns>
  internal string ToOSCompatibleString() => ConvertRawToOS(this._rawFullName);

  /// <inheritdoc/>
  public override string ToString() => this.FullName;

  private static Regex CmdEnvironmentVariables = CmdEnvVarRegex();
  private static Regex PwshEnvironmentVariables = PwshEnvVarRegex();
  private static Regex VSCodeEnvironmentVariables = VSCodeEnvVarRegex();
  private static Regex BashEnvironmentVariables = BashEnvVarRegex();

  /// <summary>
  /// TODO: Add method summary.
  /// </summary>
  /// <param name="path"></param>
  /// <returns></returns>
  private static string ConvertCmd(string path) {
    string output = path;
    if (CmdEnvironmentVariables.IsMatch(output)) {
      MatchCollection matches = CmdEnvironmentVariables.Matches(output);
      foreach (GroupCollection match in matches.Select(x => x.Groups)) {
        string? envVar = Environment.GetEnvironmentVariable(match[1].Value);
        if (envVar is string sEnvVar) {
          output = output.Replace(match[0].Value, sEnvVar);
        } else if (!Services.IsConfigNull()) {
          envVar = Config.GetEnvironmentVariable(match[1].Value);
          if (envVar is string cEnvVar) {
            output = output.Replace(match[0].Value, cEnvVar);
          }
        }
      }
    }
    return output;
  }

  /// <summary>
  /// TODO: Add method summary.
  /// </summary>
  /// <param name="path"></param>
  /// <param name="os"></param>
  /// <returns></returns>
  private static string ConvertCmdToOS(string path, OSPlatform os) {
    string output = path;
    if (os == OSPlatform.Windows) {
      return path;
    }
    if (CmdEnvironmentVariables.IsMatch(output)) {
      MatchCollection matches = CmdEnvironmentVariables.Matches(output);
      foreach (GroupCollection match in matches.Select(x => x.Groups)) {
        output = output.Replace(match[0].Value, $"%{match[1].Value}%");
      }
    }
    return output;
  }

  /// <summary>
  /// TODO: Add method summary.
  /// </summary>
  /// <param name="path"></param>
  /// <returns></returns>
  private static string ConvertPwsh(string path) {
    string output = path;
    if (PwshEnvironmentVariables.IsMatch(output)) {
      MatchCollection matches = PwshEnvironmentVariables.Matches(output);
      foreach (GroupCollection match in matches.Select(x => x.Groups)) {
        string? envVar = Environment.GetEnvironmentVariable(match[1].Value);
        if (envVar is string sEnvVar) {
          output = output.Replace(match[0].Value, sEnvVar);
        } else if (!Services.IsConfigNull()) {
          envVar = Config.GetEnvironmentVariable(match[1].Value);
          if (envVar is string cEnvVar) {
            output = output.Replace(match[0].Value, cEnvVar);
          }
        }
      }
    }
    return output;
  }

  /// <summary>
  /// TODO: Add method summary.
  /// </summary>
  /// <param name="path"></param>
  /// <param name="os"></param>
  /// <returns></returns>
  private static string ConvertPwshToOS(string path, OSPlatform os) {
    string output = path;
    if (os == OSPlatform.Windows) {
      if (PwshEnvironmentVariables.IsMatch(output)) {
        MatchCollection matches = PwshEnvironmentVariables.Matches(output);
        foreach (GroupCollection match in matches.Select(x => x.Groups)) {
          output = output.Replace(match[0].Value, $"%{match[1].Value}%");
        }
      }
    } else {
      if (PwshEnvironmentVariables.IsMatch(output)) {
        MatchCollection matches = PwshEnvironmentVariables.Matches(output);
        foreach (GroupCollection match in matches.Select(x => x.Groups)) {
          output = output.Replace(match[0].Value, $"${match[1].Value}");
        }
      }
    }
    return output;
  }

  /// <summary>
  /// TODO: Add method summary.
  /// </summary>
  /// <param name="path"></param>
  /// <returns></returns>
  private static string ConvertVSCode(string path) {
    string output = path;
    if (VSCodeEnvironmentVariables.IsMatch(output)) {
      MatchCollection matches = VSCodeEnvironmentVariables.Matches(output);
      foreach (GroupCollection match in matches.Select(x => x.Groups)) {
        string? envVar = Environment.GetEnvironmentVariable(match[1].Value);
        if (envVar is string sEnvVar) {
          output = output.Replace(match[0].Value, sEnvVar);
        } else if (!Services.IsConfigNull()) {
          envVar = Config.GetEnvironmentVariable(match[1].Value);
          if (envVar is string cEnvVar) {
            output = output.Replace(match[0].Value, cEnvVar);
          }
        }
      }
    }
    return output;
  }

  /// <summary>
  /// TODO: Add method summary.
  /// </summary>
  /// <param name="path"></param>
  /// <param name="os"></param>
  /// <returns></returns>
  private static string ConvertVSCodeToOS(string path, OSPlatform os) {
    string output = path;
    if (os == OSPlatform.Windows) {
      if (VSCodeEnvironmentVariables.IsMatch(output)) {
        MatchCollection matches = VSCodeEnvironmentVariables.Matches(output);
        foreach (GroupCollection match in matches.Select(x => x.Groups)) {
          output = output.Replace(match[0].Value, $"%{match[1].Value}%");
        }
      }
    } else {
      if (VSCodeEnvironmentVariables.IsMatch(output)) {
        MatchCollection matches = VSCodeEnvironmentVariables.Matches(output);
        foreach (GroupCollection match in matches.Select(x => x.Groups)) {
          output = output.Replace(match[0].Value, $"${match[1].Value}");
        }
      }
    }
    return output;
  }

  /// <summary>
  /// TODO: Add method summary.
  /// </summary>
  /// <param name="path"></param>
  /// <returns></returns>
  private static string ConvertBash(string path) {
    string output = path;
    if (BashEnvironmentVariables.IsMatch(output)) {
      MatchCollection matches = BashEnvironmentVariables.Matches(output);
      foreach (GroupCollection match in matches.Select(x => x.Groups)) {
        string? envVar = Environment.GetEnvironmentVariable(match[1].Value);
        if (envVar is string sEnvVar) {
          output = output.Replace(match[0].Value, sEnvVar);
        } else if (!Services.IsConfigNull()) {
          envVar = Config.GetEnvironmentVariable(match[1].Value);
          if (envVar is string cEnvVar) {
            output = output.Replace(match[0].Value, cEnvVar);
          }
        }
      }
    }
    return output;
  }

  /// <summary>
  /// TODO: Add method summary.
  /// </summary>
  /// <param name="path"></param>
  /// <param name="os"></param>
  /// <returns></returns>
  private static string ConvertBashToOS(string path, OSPlatform os) {
    string output = path;
    if (os != OSPlatform.Windows) {
      return path;
    }
    if (BashEnvironmentVariables.IsMatch(output)) {
      MatchCollection matches = BashEnvironmentVariables.Matches(output);
      foreach (GroupCollection match in matches.Select(x => x.Groups)) {
        output = output.Replace(match[0].Value, $"%{match[1].Value}%");
      }
    }
    return output;
  }

  /// <summary>
  /// TODO: Add method summary.
  /// </summary>
  /// <param name="path"></param>
  /// <returns></returns>
  private static string Normalize(string path) {
    string output = path;
    output = ConvertCmd(output);
    output = ConvertPwsh(output);
    output = ConvertVSCode(output);
    output = ConvertBash(output);
    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
      output = output.Replace("\\\\", "\\").Replace("/", "\\");
    } else {
      output = output.Replace("\\\\", "\\").Replace("\\", "/");
    }
    return output;
  }

  /// <summary>
  /// TODO: Add method summary.
  /// </summary>
  /// <param name="path"></param>
  /// <returns></returns>
  private static string ConvertRawToOS(string path) {
    string output = path;
    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
      output = ConvertCmdToOS(output, OSPlatform.Windows);
      output = ConvertPwshToOS(output, OSPlatform.Windows);
      output = ConvertVSCodeToOS(output, OSPlatform.Windows);
      output = ConvertBashToOS(output, OSPlatform.Windows);
      output = output.Replace("\\\\", "\\").Replace("/", "\\");
    } else {
      output = ConvertCmdToOS(output, OSPlatform.Linux);
      output = ConvertPwshToOS(output, OSPlatform.Linux);
      output = ConvertVSCodeToOS(output, OSPlatform.Linux);
      output = ConvertBashToOS(output, OSPlatform.Linux);
      output = output.Replace("\\\\", "\\").Replace("\\", "/");
    }
    return output;
  }

  [GeneratedRegex(@"%([^%]+)%")]
  private static partial Regex CmdEnvVarRegex();

  [GeneratedRegex(@"\$env:([\w\d_]+)")]
  private static partial Regex PwshEnvVarRegex();

  [GeneratedRegex(@"\$\{env:([^\}]+)\}")]
  private static partial Regex VSCodeEnvVarRegex();

  [GeneratedRegex(@"\$([\w\d_]+)")]
  private static partial Regex BashEnvVarRegex();
}
