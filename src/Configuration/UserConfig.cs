using Newtonsoft.Json;

namespace Rebuild_BinFolder.Configuration;

[Serializable]
public class UserConfig {
  [JsonProperty("user_sid")]
  public string SID { get; set; } = "";

  [JsonProperty("user_aux_list")]
  public string UserAuxList {
    get; set;
  }

  [JsonProperty("user_programs")]
  public List<ProgramPath> UserPrograms {
    get; set;
  } = [];

  [JsonProperty("force_in_user_path")]
  public List<ProgramPath> ForceInUserPath {
    get; set;
  } = [];

  [JsonProperty("custom_environment_variables")]
  public Dictionary<string, ProgramPath> CustomEnvironmentVariables {
    get; set;
  } = [];

  [JsonConstructor]
  public UserConfig(string sid, string userAuxList, List<ProgramPath> userPrograms, List<ProgramPath> forceInUserPath, Dictionary<string, ProgramPath> customEnvironmentVariables) {
    this.SID = sid;
    this.UserAuxList = userAuxList;
    this.UserPrograms = userPrograms;
    this.ForceInUserPath = forceInUserPath;
    this.CustomEnvironmentVariables = customEnvironmentVariables;
  }

  public UserConfig(string sid) {
    this.SID = sid;
    this.UserAuxList = "UPROG_LIST";
  }

  private UserConfig() {
    this.UserAuxList = "UPROG_LIST";
  }

  public static UserConfig Empty => new();
}
