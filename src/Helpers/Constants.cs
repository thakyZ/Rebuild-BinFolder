/* cspell: ignore APROG, UPROG */
// Ignore Spelling: APROG, UPROG
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

using Rebuild_BinFolder.Attributes;

namespace Rebuild_BinFolder.Helpers;

/// <summary>
/// Static class of constants for the rebuilding of the bin folder.
/// </summary>
public static class Constants {
  [SuppressMessage("Critical Code Smell", "S3218:Inner class members should not shadow outer class \"static\" or type members", Justification = "Default values should be named the same.")]
  public static class Default {
    /// <summary>
    ///  environment variable key all programs list for the system.
    /// </summary>
    public const string SystemProgramsList = "APROG_LIST";

    /// <summary>
    /// The environment variable key that is where all other programs are stored for the system.
    /// </summary>
    public const string SystemProgramsDirectory = "APROG_DIR";

    /// <summary>
    /// The environment variable key all programs for the current user.
    /// </summary>
    public const string UserProgramsList = "UPROG_LIST";

    /// <summary>
    /// The environment variable key that is where all other programs are stored for the current user.
    /// </summary>
    public const string UserProgramsDirectory = "UPROG_DIR";

    /// <summary>
    /// TODO: Add field summary.
    /// </summary>
    public static string SystemProgramsDirectoryValue => Path.Join(Environment.GetEnvironmentVariable("SystemDrive"), "Files", "System", "Programs");

    /// <summary>
    /// TODO: Add field summary.
    /// </summary>
    public static string UserProgramsDirectoryValue => Path.Join(Environment.GetEnvironmentVariable("USERPROFILE"), ".local", "programs");
  }

  /// <summary>
  /// The universal sub-key for environment variables on the system.
  /// </summary>
  public const string SubKey = "Environment";

  /// <summary>
  ///  environment variable key all programs list for the system.
  /// </summary>
  public static string SystemProgramsList => Services.Config.AdminConfig.AuxListName;

  /// <summary>
  /// The environment variable key that is where all other programs are stored for the system.
  /// </summary>
  public static string SystemProgramsDirectory => Services.Config.AdminConfig.Root.Name;

  /// <summary>
  /// The full registry path to the universal system environment variables.
  /// </summary>
  public const string SystemSubKeyPath = @$"SYSTEM\CurrentControlSet\Control\Session Manager\{SubKey}";

  /// <summary>
  /// The environment variable key all programs for the current user.
  /// </summary>
  public static string UserProgramsList => Services.Config.GetCurrentUserConfig()?.AuxListName ?? Default.UserProgramsList;

  /// <summary>
  /// The environment variable key that is where all other programs are stored for the current user.
  /// </summary>
  public static string UserProgramsDirectory => Services.Config.GetCurrentUserConfig()?.Root.Name ?? Default.UserProgramsList;

  /// <summary>
  /// The full registry path to the current user environment variables.
  /// </summary>
  public const string UserSubKeyPath = SubKey;

  public static Uri GetRepositoryUri() {
    Assembly assembly = Assembly.GetExecutingAssembly();
    string? name  = assembly.GetCustomAttribute<AssemblyRepositoryNameAttribute>()?.Value;
    string? owner = assembly.GetCustomAttribute<AssemblyRepositoryOwnerAttribute>()?.Value;
    if (name is null || owner is null) return new Uri("");
    return new Uri($"https://github.com/{owner}/{name}", UriKind.Absolute);
  }

  public static Uri GetSchemaUri() {
    Assembly assembly = Assembly.GetExecutingAssembly();
    string? name  = assembly.GetCustomAttribute<AssemblyRepositoryNameAttribute>()?.Value;
    string? owner = assembly.GetCustomAttribute<AssemblyRepositoryOwnerAttribute>()?.Value;
    if (name is null || owner is null) return new Uri("");
    return new Uri($"{GetRepositoryUri()}/raw/main/config.schema.json", UriKind.Absolute);
  }
}
