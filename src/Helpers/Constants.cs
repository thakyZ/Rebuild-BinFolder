/* cspell: ignore APROG, UPROG */
// Ignore Spelling: APROG, UPROG
namespace Rebuild_BinFolder.Helpers;

/// <summary>
/// Static class of constants for the rebuilding of the bin folder.
/// </summary>
public static class Constants {
  /// <summary>
  /// The universal sub-key for environment variables on the system.
  /// </summary>
  public static string SubKey => "Environment";
  /// <summary>
  ///  environment variable key all programs list for the system.
  /// </summary>
  public static string SystemProgramsList => "APROG_LIST";
  /// <summary>
  /// The environment variable key that is where all other programs are stored for the system.
  /// </summary>
  public static string SystemProgramsDirectory => "APROG_DIR";
  /// <summary>
  /// The full registry path to the universal system environment variables.
  /// </summary>
  public static string SystemSubKeyPath => @$"SYSTEM\CurrentControlSet\Control\Session Manager\{SubKey}";
  /// <summary>
  /// The environment variable key all programs for the current user.
  /// </summary>
  public static string UserProgramsList => "UPROG_LIST";
  /// <summary>
  /// The environment variable key that is where all other programs are stored for the current user.
  /// </summary>
  public static string UserProgramsDirectory => "UPROG_DIR";
  /// <summary>
  /// The full registry path to the current user environment variables.
  /// </summary>
  public static string UserSubKeyPath => SubKey;
}
