#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using System.Diagnostics.CodeAnalysis;
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

  [SuppressMessage("Roslynator", "RCS1169:Make field read-only", Justification = "Json.NET applies value via reflection.")]
  [JsonExtensionData]
  private IDictionary<string, JToken> _additionalData;

  [OnDeserialized]
  private void OnDeserialized(StreamingContext context) {
    var custom = (JObject)this._additionalData["custom_environment_variables"];
    if (!custom.HasValues) return;
    foreach ((var key, JToken? value) in custom) {
      if (value is null) continue;
      // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
      switch (value.Type) {
        case JTokenType.Array:
          this.CustomEnvironmentVariables.Add(new CustomEnvironmentVar(key, value.Values<string>()));
          break;
        case JTokenType.String:
          this.CustomEnvironmentVariables.Add(new CustomEnvironmentVar(key, (string?)value));
          break;
        default:
          throw new JsonException($"Invalid type of JToken at key {key}, expected Array or String, got {value.Type}");
      }
    }
  }
}
