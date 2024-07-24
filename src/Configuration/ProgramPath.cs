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
    return str == this.RawFullName;
  }

  /// <summary>
  /// TODO: Add method summary.
  /// </summary>
  /// <param name="str"></param>
  /// <returns></returns>
  public bool Equals(string? str) {
    if (str is null) return false;
    return str == this.FullName;
  }

  /// <summary>
  /// TODO: Add method summary.
  /// </summary>
  /// <param name="path"></param>
  /// <returns></returns>
  public bool Equals(ProgramPath? path) {
    if (path is null) return false;
    return path.RawFullName == this.RawFullName && path.FullName == this.FullName;
  }

  /// <summary>
  /// TODO: Add method summary.
  /// </summary>
  /// <param name="obj"></param>
  /// <returns></returns>
  public override bool Equals(object? obj) {
    return obj switch {
      null => false,
      string str when this.Equals(str) => true,
      string str when this.EqualsRaw(str) => true,
      ProgramPath path => this.Equals(path),
      _ => false
    };
  }

  /// <summary>
  /// TODO: Add method summary.
  /// </summary>
  /// <returns></returns>
  public override int GetHashCode() {
    return HashCode.Combine(this.FullName.GetHashCode(), this.RawFullName.GetHashCode(), this.Exists.GetHashCode());
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
  internal string ToOsCompatibleString() => ConvertRawToOs(this._rawFullName);

  /// <inheritdoc/>
  public override string ToString() => this.FullName;

  private static readonly Regex CmdEnvironmentVariables = CmdEnvVarRegex();
  private static readonly Regex PwshEnvironmentVariables = PwshEnvVarRegex();
  private static readonly Regex VSCodeEnvironmentVariables = VsCodeEnvVarRegex();
  private static readonly Regex BashEnvironmentVariables = BashEnvVarRegex();

  /// <summary>
  /// TODO: Add method summary.
  /// </summary>
  /// <param name="path"></param>
  /// <returns></returns>
  private static string ConvertCmd(string path) {
    var output = path;
    if (!CmdEnvironmentVariables.IsMatch(output)) {
      return output;
    }

    MatchCollection matches = CmdEnvironmentVariables.Matches(output);
    foreach (GroupCollection match in matches.Select(x => x.Groups)) {
      var envVar = Environment.GetEnvironmentVariable(match[1].Value);
      if (envVar is { } sEnvVar) {
        output = output.Replace(match[0].Value, sEnvVar);
      } else if (!Services.IsConfigNull()) {
        envVar = Config.GetEnvironmentVariable(match[1].Value);
        if (envVar is { } cEnvVar) {
          output = output.Replace(match[0].Value, cEnvVar);
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
  private static string ConvertCmdToOs(string path, OSPlatform os) {
    var output = path;
    if (os == OSPlatform.Windows) {
      return path;
    }

    if (!CmdEnvironmentVariables.IsMatch(output)) {
      return output;
    }

    MatchCollection matches = CmdEnvironmentVariables.Matches(output);
    foreach (GroupCollection match in matches.Select(x => x.Groups)) {
      output = output.Replace(match[0].Value, $"%{match[1].Value}%");
    }
    return output;
  }

  /// <summary>
  /// TODO: Add method summary.
  /// </summary>
  /// <param name="path"></param>
  /// <returns></returns>
  private static string ConvertPwsh(string path) {
    var output = path;
    if (!PwshEnvironmentVariables.IsMatch(output)) {
      return output;
    }

    MatchCollection matches = PwshEnvironmentVariables.Matches(output);
    foreach (GroupCollection match in matches.Select(x => x.Groups)) {
      var envVar = Environment.GetEnvironmentVariable(match[1].Value);
      if (envVar is { } sEnvVar) {
        output = output.Replace(match[0].Value, sEnvVar);
      } else if (!Services.IsConfigNull()) {
        envVar = Config.GetEnvironmentVariable(match[1].Value);
        if (envVar is { } cEnvVar) {
          output = output.Replace(match[0].Value, cEnvVar);
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
  private static string ConvertPwshToOs(string path, OSPlatform os) {
    var output = path;
    if (os == OSPlatform.Windows) {
      if (!PwshEnvironmentVariables.IsMatch(output)) {
        return output;
      }

      MatchCollection matches = PwshEnvironmentVariables.Matches(output);
      foreach (GroupCollection match in matches.Select(x => x.Groups)) {
        output = output.Replace(match[0].Value, $"%{match[1].Value}%");
      }
    } else {
      if (!PwshEnvironmentVariables.IsMatch(output)) {
        return output;
      }

      MatchCollection matches = PwshEnvironmentVariables.Matches(output);
      foreach (GroupCollection match in matches.Select(x => x.Groups)) {
        output = output.Replace(match[0].Value, $"${match[1].Value}");
      }
    }
    return output;
  }

  /// <summary>
  /// TODO: Add method summary.
  /// </summary>
  /// <param name="path"></param>
  /// <returns></returns>
  private static string ConvertVsCode(string path) {
    var output = path;
    if (!VSCodeEnvironmentVariables.IsMatch(output)) {
      return output;
    }

    MatchCollection matches = VSCodeEnvironmentVariables.Matches(output);
    foreach (GroupCollection match in matches.Select(x => x.Groups)) {
      var envVar = Environment.GetEnvironmentVariable(match[1].Value);
      if (envVar is { } sEnvVar) {
        output = output.Replace(match[0].Value, sEnvVar);
      } else if (!Services.IsConfigNull()) {
        envVar = Config.GetEnvironmentVariable(match[1].Value);
        if (envVar is { } cEnvVar) {
          output = output.Replace(match[0].Value, cEnvVar);
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
  private static string ConvertVsCodeToOs(string path, OSPlatform os) {
    var output = path;
    if (os == OSPlatform.Windows) {
      if (!VSCodeEnvironmentVariables.IsMatch(output)) {
        return output;
      }

      MatchCollection matches = VSCodeEnvironmentVariables.Matches(output);
      foreach (GroupCollection match in matches.Select(x => x.Groups)) {
        output = output.Replace(match[0].Value, $"%{match[1].Value}%");
      }
    } else {
      if (!VSCodeEnvironmentVariables.IsMatch(output)) {
        return output;
      }

      MatchCollection matches = VSCodeEnvironmentVariables.Matches(output);
      foreach (GroupCollection match in matches.Select(x => x.Groups)) {
        output = output.Replace(match[0].Value, $"${match[1].Value}");
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
    var output = path;
    if (!BashEnvironmentVariables.IsMatch(output)) {
      return output;
    }

    MatchCollection matches = BashEnvironmentVariables.Matches(output);
    foreach (GroupCollection match in matches.Select(x => x.Groups)) {
      var envVar = Environment.GetEnvironmentVariable(match[1].Value);
      if (envVar is { } sEnvVar) {
        output = output.Replace(match[0].Value, sEnvVar);
      } else if (!Services.IsConfigNull()) {
        envVar = Config.GetEnvironmentVariable(match[1].Value);
        if (envVar is { } cEnvVar) {
          output = output.Replace(match[0].Value, cEnvVar);
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
  private static string ConvertBashToOs(string path, OSPlatform os) {
    var output = path;
    if (os != OSPlatform.Windows) {
      return path;
    }

    if (!BashEnvironmentVariables.IsMatch(output)) {
      return output;
    }

    MatchCollection matches = BashEnvironmentVariables.Matches(output);
    foreach (GroupCollection match in matches.Select(x => x.Groups)) {
      output = output.Replace(match[0].Value, $"%{match[1].Value}%");
    }
    return output;
  }

  /// <summary>
  /// TODO: Add method summary.
  /// </summary>
  /// <param name="path"></param>
  /// <returns></returns>
  private static string Normalize(string path) {
    var output = path;
    output = ConvertCmd(output);
    output = ConvertPwsh(output);
    output = ConvertVsCode(output);
    output = ConvertBash(output);
    output = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
      ? output.Replace(@"\\", "\\").Replace("/", "\\")
      : output.Replace(@"\\", "\\").Replace("\\", "/");
    return output;
  }

  /// <summary>
  /// TODO: Add method summary.
  /// </summary>
  /// <param name="path"></param>
  /// <returns></returns>
  private static string ConvertRawToOs(string path) {
    var output = path;
    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
      output = ConvertCmdToOs(output, OSPlatform.Windows);
      output = ConvertPwshToOs(output, OSPlatform.Windows);
      output = ConvertVsCodeToOs(output, OSPlatform.Windows);
      output = ConvertBashToOs(output, OSPlatform.Windows);
      return output.Replace(@"\\", "\\").Replace("/", "\\");
    } else {
      output = ConvertCmdToOs(output, OSPlatform.Linux);
      output = ConvertPwshToOs(output, OSPlatform.Linux);
      output = ConvertVsCodeToOs(output, OSPlatform.Linux);
      output = ConvertBashToOs(output, OSPlatform.Linux);
      return output.Replace(@"\\", "\\").Replace("\\", "/");
    }
  }

  [GeneratedRegex(@"%([^%]+)%")]
  private static partial Regex CmdEnvVarRegex();

  [GeneratedRegex(@"\$env:([\w\d_]+)")]
  private static partial Regex PwshEnvVarRegex();

  [GeneratedRegex(@"\$\{env:([^\}]+)\}")]
  private static partial Regex VsCodeEnvVarRegex();

  [GeneratedRegex(@"\$([\w\d_]+)")]
  private static partial Regex BashEnvVarRegex();
}
