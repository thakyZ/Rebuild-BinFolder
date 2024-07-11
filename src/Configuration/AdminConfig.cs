#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using System.Runtime.Serialization;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

using Rebuild_BinFolder.Configuration.Converters;

namespace Rebuild_BinFolder.Configuration;

[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
public class AdminConfig {
  /// <summary>
  /// TODO: Add property summary.
  /// </summary>
  [JsonProperty("root", Order = 0)]
  public AuxName Root { get; set; }

  /// <summary>
  /// TODO: Add property summary.
  /// </summary>
  [JsonProperty("aux_list_name", Order = 1)]
  public string AuxListName { get; set; }

  /// <summary>
  /// TODO: Add property summary.
  /// </summary>
  [JsonConverter(typeof(ProgramPathsConverter))]
  [JsonProperty("programs", Order = 2)]
  public List<ProgramPath> Programs { get; set; }

  /// <summary>
  /// TODO: Add property summary.
  /// </summary>
  [JsonConverter(typeof(ProgramPathsConverter))]
  [JsonProperty("force_in_path", Order = 3)]
  public List<ProgramPath> ForceInPath { get; set; }

  /// <summary>
  /// TODO: Add property summary.
  /// </summary>
  public List<CustomEnvironmentVar> CustomEnvironmentVariables { get; set; } = [];

  /// <summary>
  /// TODO: Add property summary.
  /// </summary>
  internal static AdminConfig Empty => new();

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
