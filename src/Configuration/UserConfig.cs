using Newtonsoft.Json;

namespace Rebuild_BinFolder.Configuration;

[Serializable]
public class UserConfig {
  [JsonProperty("user_sid")]
  public string SID { get; set; } = "";

  [JsonProperty("user_programs")]
  public List<ProgramPath> UserPrograms {
    get; set;
  } = new();

  [JsonProperty("force_in_user_path")]
  public List<ProgramPath> ForceInUserPath {
    get; set;
  } = new();

  [JsonProperty("custom_environment_variables")]
  public Dictionary<string, ProgramPath> CustomEnvironmentVariables {
    get; set;
  } = new();

  [JsonConstructor]
  public UserConfig(string sid, List<ProgramPath> userPrograms, List<ProgramPath> forceInUserPath, Dictionary<string, ProgramPath> customEnvironmentVariables) {
    this.SID = sid;
    this.UserPrograms = userPrograms;
    this.ForceInUserPath = forceInUserPath;
    this.CustomEnvironmentVariables = customEnvironmentVariables;
  }

  public UserConfig(string sid) {
    this.SID = sid;
  }

  private UserConfig() { }

  public static UserConfig Empty => new();
}
