#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using System.ComponentModel;
using System.Runtime.Serialization;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

using Rebuild_BinFolder.Attributes;
using Rebuild_BinFolder.Configuration.Converters;
using Rebuild_BinFolder.Helpers;

namespace Rebuild_BinFolder.Configuration;

[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
public class UserConfig {
  /// <summary>
  /// TODO: Add property summary.
  /// </summary>
  [JsonProperty("info", Order = 0)]
  [DefaultValueCallStaticProperty(typeof(UserInfo), nameof(UserInfo.Default))]
  public UserInfo Info { get; set; }

  /// <summary>
  /// TODO: Add property summary.
  /// </summary>
  [JsonProperty("root", Order = 1)]
  [DefaultValueCallStaticProperty(typeof(AuxName), nameof(AuxName.DefaultUser))]
  public AuxName Root { get; set; }

  /// <summary>
  /// TODO: Add property summary.
  /// </summary>
  [JsonProperty("aux_list_name", Order = 2)]
  [DefaultValue(Constants.Default.UserProgramsList)]
  public string AuxListName { get; set; }

  /// <summary>
  /// TODO: Add property summary.
  /// </summary>
  [JsonConverter(typeof(ProgramPathsConverter))]
  [JsonProperty("programs", Order = 3)]
  [DefaultValueNew(typeof(List<ProgramPath>), [])]
  public List<ProgramPath> Programs { get; set; }

  /// <summary>
  /// TODO: Add property summary.
  /// </summary>
  [JsonConverter(typeof(ProgramPathsConverter))]
  [JsonProperty("force_in_path", Order = 4)]
  [DefaultValueNew(typeof(List<ProgramPath>), [])]
  public List<ProgramPath> ForceInPath { get; set; }

  /// <summary>
  /// TODO: Add property summary.
  /// </summary>
  public List<CustomEnvironmentVar> CustomEnvironmentVariables { get; set; } = [];

  /// <summary>
  /// TODO: Add property summary.
  /// </summary>
  internal static UserConfig Empty => new();

  public UserConfig() {}

  [JsonExtensionData]
  private IDictionary<string, JToken> _additionalData;

  [OnDeserialized]
  private void OnDeserialized(StreamingContext context) {
    JObject custom = (JObject)_additionalData["custom_environment_variables"];
    if (!custom.HasValues) return;
    foreach ((string key, JToken? value) in custom) {
      if (value is null) continue;
      if (value.Type == JTokenType.Array) {
        CustomEnvironmentVariables.Add(new(key, value.Values<string>()));
      } else if (value.Type == JTokenType.String) {
        CustomEnvironmentVariables.Add(new(key, (string?)value));
      } else {
        throw new JsonException($"Invalid type of JToken at key {key}, expected Array or String, got {value.Type}");
      }
    }
  }
}
