#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
namespace Rebuild_BinFolder.Configuration;

public class Equivalent {
  /// <summary>
  /// TODO: Add property summary.
  /// </summary>
  public string Key { get; set; }

  /// <summary>
  /// TODO: Add property summary.
  /// </summary>
  public string Value { get; set; }

  public Equivalent(string key, string value) {
    this.Key = key;
    this.Value = value;
  }

  /// <summary>
  /// TODO: Add property summary.
  /// </summary>
  internal static Equivalent Template => new("%APPDATA%", "<userDir>/AppData/Roaming");

  /// <summary>
  /// TODO: Add property summary.
  /// </summary>
  internal static List<Equivalent> Templates => [
    Template,
    new("%LOCALAPPDATA%","<userDir>/AppData/Local"),
    new("%USERPROFILE%","<userDir>"),
    new("%HOME%","<userDir>"),
    new("Progra~1","Program Files"),
    new("Progra~2","Program Files (x86)")
  ];
}
