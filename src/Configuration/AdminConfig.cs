using Newtonsoft.Json;

namespace Rebuild_BinFolder.Configuration;

[Serializable]
public class AdminConfig {
  [JsonProperty("admin_root")]
  public AuxName AdminRoot {
    get; set;
  }

  [JsonProperty("admin_aux_list")]
  public string AdminAuxList {
    get; set;
  }

  [JsonProperty("admin_programs")]
  public List<ProgramPath> AdminPrograms {
    get; set;
  } = [];

  [JsonProperty("force_in_admin_path")]
  public List<ProgramPath> ForceInAdminPath {
    get; set;
  } = [];

  [JsonProperty("custom_environment_variables")]
  public Dictionary<string, ProgramPath> CustomEnvironmentVariables {
    get; set;
  } = [];

  [JsonConstructor]
  public AdminConfig(AuxName adminRoot, string adminAuxPath, List<ProgramPath> adminPrograms, List<ProgramPath> forceInAdminPath, Dictionary<string, ProgramPath> customEnvironmentVariables) {
    this.AdminRoot = adminRoot;
    this.AdminAuxList = adminAuxPath;
    this.AdminPrograms = adminPrograms;
    this.ForceInAdminPath = forceInAdminPath;
    this.CustomEnvironmentVariables = customEnvironmentVariables;
  }

  public AdminConfig(AuxName adminRoot, string adminAuxPath) {
    this.AdminAuxList = adminAuxPath;
    this.AdminRoot = adminRoot;
  }

  public AdminConfig(AuxName adminRoot) {
    this.AdminAuxList = "APROG_LIST";
    this.AdminRoot = adminRoot;
  }

  public AdminConfig(string adminAuxPath) {
    this.AdminAuxList = adminAuxPath;
    this.AdminRoot = new AuxName("APROG_DIR", ProgramPath.Empty);
  }

  private AdminConfig() {
    this.AdminAuxList = "APROG_LIST";
    this.AdminRoot = new AuxName("APROG_DIR", ProgramPath.Empty);
  }

  public static AdminConfig Empty => new();
}
