#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using System.ComponentModel;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using Rebuild_BinFolder.Attributes;
using Rebuild_BinFolder.Configuration.Converters;
using Rebuild_BinFolder.Helpers;

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

  [JsonProperty("$schema", Order = 0)]
  [DefaultValueCallStaticMethod(typeof(Constants), nameof(Constants.GetSchemaUri))]
  public Uri Schema { get; set; }

  /// <summary>
  /// TODO: Add property summary.
  /// </summary>
  [JsonConverter(typeof(VersionNumberMigrate))]
  [JsonProperty("version", Order = 1)]
  [DefaultValue(_currentVersion)]
  public long Version { get; set; }

  /// <summary>
  /// TODO: Add property summary.
  /// </summary>
  [JsonProperty("log_level", Order = 2)]
  [DefaultValue(LogLevel.Info)]
  public LogLevel LogLevel { get; set; }

  /// <summary>
  /// TODO: Add property summary.
  /// </summary>
  [JsonConverter(typeof(EquivalentConverter))]
  [JsonProperty("equivalents", Order = 3)]
  [DefaultValueCallStaticProperty(typeof(Equivalent), nameof(Equivalent.Templates))]
  public List<Equivalent> Equivalents { get; set; }

  /// <summary>
  /// TODO: Add property summary.
  /// </summary>
  [JsonProperty("user_configs", Order = 4)]
  [DefaultValueNew(typeof(List<UserConfig>), [])]
  public List<UserConfig> UserConfigs { get; set; }

  /// <summary>
  /// TODO: Add property summary.
  /// </summary>
  [JsonProperty("admin_config", Order = 5)]
  [DefaultValueNew(typeof(AdminConfig))]
  public AdminConfig AdminConfig { get; set; }

  public Config() {}

  static Config() {
    CurrentUserSID = GetCurrentUserSsid() ?? "unknown";
  }
}
