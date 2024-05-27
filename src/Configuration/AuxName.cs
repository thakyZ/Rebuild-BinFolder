using Newtonsoft.Json;

namespace Rebuild_BinFolder.Configuration;
[Serializable]
public class AuxName {
  [JsonProperty(propertyName: "name")]
  public string Name { get; set; }
  [JsonProperty("path")]
  public ProgramPath Path { get; set; }

  [JsonConstructor]
  public AuxName(string name, ProgramPath path) {
    this.Name = name;
    this.Path = path;
  }
}
