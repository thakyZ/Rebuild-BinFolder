using Newtonsoft.Json;

namespace Rebuild_BinFolder.Configuration;

[Serializable]
public class AdminConfig {
  [JsonProperty("admin_root")]
  public ProgramPath AdminRoot {
    get; set;
  }

  [JsonProperty("admin_programs")]
  public List<ProgramPath> AdminPrograms {
    get; set;
  } = new();

  [JsonProperty("force_in_admin_path")]
  public List<ProgramPath> ForceInAdminPath {
    get; set;
  } = new();

  [JsonProperty("custom_environment_variables")]
  public Dictionary<string, ProgramPath> CustomEnvironmentVariables {
    get; set;
  } = new();

  [JsonConstructor]
  public AdminConfig(ProgramPath adminRoot, List<ProgramPath> adminPrograms, List<ProgramPath> forceInAdminPath, Dictionary<string, ProgramPath> customEnvironmentVariables) {
    this.AdminRoot = adminRoot;
    this.AdminPrograms = adminPrograms;
    this.ForceInAdminPath = forceInAdminPath;
    this.CustomEnvironmentVariables = customEnvironmentVariables;
  }

  private AdminConfig() {
    this.AdminRoot = ProgramPath.Empty;
  }

  public AdminConfig(ProgramPath adminRoot) {
    this.AdminRoot = adminRoot;
  }

  public static AdminConfig Empty => new();
}
