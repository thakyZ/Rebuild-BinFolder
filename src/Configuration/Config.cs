#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using System.ComponentModel;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using Rebuild_BinFolder.Configuration.Converters;
using Rebuild_BinFolder.Extensions;

namespace Rebuild_BinFolder.Configuration;

[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
public partial class Config {
  /// <summary>
  /// TODO: Add field summary.
  /// </summary>
  private static string _configPath = string.Empty;

  /// <summary>
  /// TODO: Add property summary.
  /// </summary>
  internal static string CurrentUserSID { get; private set; }

  /// <summary>
  /// TODO: Add field summary.
  /// </summary>
  private const int _currentVersion = 2;

  /// <summary>
  /// TODO: Add property summary.
  /// </summary>
  [JsonConverter(typeof(VersionNumberMigrate))]
  [JsonProperty("version")]
  [DefaultValue(_currentVersion)]
  public long Version { get; set; }

  /// <summary>
  /// TODO: Add property summary.
  /// </summary>
  [JsonProperty("log_level")]
  [DefaultValue(LogLevel.Info)]
  public LogLevel LogLevel { get; set; }

  /// <summary>
  /// TODO: Add property summary.
  /// </summary>
  [JsonConverter(typeof(EquivalentConverter))]
  [JsonProperty("equivalents")]
  [DefaultValueNew(typeof(List<Equivalent>), [])]
  public List<Equivalent> Equivalents { get; set; }

  /// <summary>
  /// TODO: Add property summary.
  /// </summary>
  [JsonProperty("user_configs")]
  [DefaultValueNew(typeof(List<Equivalent>), [])]
  public List<UserConfig> UserConfigs { get; set; }

  /// <summary>
  /// TODO: Add property summary.
  /// </summary>
  [JsonProperty("admin_config")]
  [DefaultValueCallStaticProperty(typeof(AdminConfig), nameof(AdminConfig.Empty))]
  public AdminConfig AdminConfig { get; set; }

  public Config() {}

  static Config() {
    CurrentUserSID = GetCurrentUserSID();
  }
}
