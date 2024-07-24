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
    new Equivalent("%LOCALAPPDATA%","<userDir>/AppData/Local"),
    new Equivalent("%USERPROFILE%","<userDir>"),
    new Equivalent("%HOME%","<userDir>"),
    new Equivalent("Progra~1","Program Files"),
    new Equivalent("Progra~2","Program Files (x86)")
  ];
}
