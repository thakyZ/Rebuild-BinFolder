#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using Rebuild_BinFolder.Configuration.Converters;
using Rebuild_BinFolder.Helpers;

namespace Rebuild_BinFolder.Configuration;

[JsonObject(MemberSerialization = MemberSerialization.OptIn, NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
public class AuxName {
  /// <summary>
  /// TODO: Add property summary.
  /// </summary>
  [JsonProperty("name", Order = 0)]
  public string Name { get; set; }

  /// <summary>
  /// TODO: Add property summary.
  /// </summary>
  [JsonConverter(typeof(ProgramPathConverter))]
  [JsonProperty("path", Order = 1)]
  public ProgramPath Path { get; set; }

  public AuxName() { }

  [JsonConstructor]
  public AuxName(string name, ProgramPath path) {
    this.Name = name;
    this.Path = path;
  }

  /// <summary>
  /// TODO: Add property summary.
  /// </summary>
  internal static AuxName DefaultSystem {
    get {
      return new(Constants.Default.SystemProgramsDirectory, ProgramPath.DefaultSystem);
    }
  }

  /// <summary>
  /// TODO: Add property summary.
  /// </summary>
  internal static AuxName DefaultUser {
    get {
      return new(Constants.Default.UserProgramsDirectory, ProgramPath.DefaultUser);
    }
  }
}
