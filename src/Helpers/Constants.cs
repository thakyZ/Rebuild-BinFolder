/* cspell: ignore APROG, UPROG */
// Ignore Spelling: APROG, UPROG
using System.Diagnostics.CodeAnalysis;

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
  public static string UserProgramsList => Services.Config.GetCurrentUserConfig().AuxListName;
  /// <summary>
  /// The environment variable key that is where all other programs are stored for the current user.
  /// </summary>
  public static string UserProgramsDirectory => Services.Config.GetCurrentUserConfig().Root.Name;
  /// <summary>
  /// The full registry path to the current user environment variables.
  /// </summary>
  public const string UserSubKeyPath = SubKey;
}
