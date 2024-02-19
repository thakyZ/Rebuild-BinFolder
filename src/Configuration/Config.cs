// Ignore Spelling: Json serializer, Newtonsoft

using System.Security.Principal;

using Newtonsoft.Json;

using Rebuild_BinFolder.Configuration.Converters;
using Rebuild_BinFolder.Exceptions;

namespace Rebuild_BinFolder.Configuration;

[Serializable]
public class Config {
  [JsonIgnore]
  private static string _configPath = string.Empty;

  [JsonIgnore]
  internal static string CurrentUserSID { get; private set; } = string.Empty;

  [JsonIgnore]
  public const int CurrentVersion = 2;

  [JsonProperty("version", ItemConverterType = typeof(VersionNumberMigrate))]
  public int Version {
    get; set;
  } = 0;

  [JsonProperty("log_level")]
  public LogLevel LogLevel {
    get; set;
  } = LogLevel.Info;

  [JsonProperty("equivalents")]
  public List<Equivalent> Equivalents {
    get; set;
  } = new();

  [JsonProperty("user_configs")]
  public List<UserConfig> UserConfigs { get; set; } = new();

  [JsonProperty("admin_config")]
  public AdminConfig AdminConfig { get; set; } = AdminConfig.Empty;

  [JsonConstructor]
  public Config(int version, List<Equivalent> equivalents, List<UserConfig> userConfigs, AdminConfig adminConfig) {
    CurrentUserSID = GetCurrentUserSID();
    Version = version;
    UserConfigs = userConfigs;
    AdminConfig = adminConfig;
    Equivalents = equivalents;
  }

  private static Config CreateDefault(string configPath) {
    Config config = new Config() {
      UserConfigs = new() {
        new UserConfig(CurrentUserSID)
      },
      Equivalents = Equivalent.Templates
    };

    config.SaveConfig(configPath);
    return config;
  }

  internal Config() {
    CurrentUserSID = GetCurrentUserSID();
    Version = CurrentVersion;
  }

  internal static bool Failed { get; }

  private static string GetCurrentUserSID() {
    try {
      return WindowsIdentity.GetCurrent().Owner!.ToString();
    } catch (Exception exception) {
      Log.Error(exception, "Could not get current user SID");
      throw;
    }
  }

  internal UserConfig GetUserConfigBySID(string sid) {
    try {
      var output = UserConfigs.Find(x => x.SID == sid);
      if (output is null) {
        Log.Error($"UserConfig with SID, {sid}, not found... Creating blank.");
        var blank = new UserConfig(sid);
        UserConfigs.Add(blank);
        return blank;
      }
      return output;
    } catch (Exception exception) {
      Log.Error(exception, $"Failed to get User Config with SID, {sid}.");
      throw;
    }
  }

  internal UserConfig GetUserConfigBySID() {
    try {
      var output = UserConfigs.Find(x => x.SID == CurrentUserSID);
      if (output is null) {
        Log.Error($"UserConfig with SID, {CurrentUserSID}, not found... Creating blank.");
        throw new UserConfigException(CurrentUserSID, $"UserConfig with SID, {CurrentUserSID}, not found... Creating blank.");
      }
      return output;
    } catch (Exception exception) {
      Log.Error(exception, $"Failed to get User Config with SID, {CurrentUserSID}.");
      throw;
    }
  }

  private static Config MigrateConfigToV2(Config config) {
    return new Config() {
      Version = 2,
      Equivalents = config.Equivalents,
      UserConfigs = new() {
        new UserConfig(CurrentUserSID) {
          UserPrograms = new(),
          ForceInUserPath = new(),
          CustomEnvironmentVariables = new()
        }
      },
      AdminConfig = AdminConfig.Empty
    };
  }

  private static void BackupOldConfig() {
    try {
      File.Move(_configPath, $"{_configPath}.{DateTime.Now.ToFileTimeUtc()}");
    } catch (Exception exception) {
      Log.Error(exception, "Failed to move config file to backup path");
      throw;
    }
  }

  private static Config MigrateConfig(Config config) {
    switch (config.Version) {
      case 2:
        return config;
      default:
        BackupOldConfig();
        var newConfig = MigrateConfigToV2(config);
        SaveConfig(_configPath, newConfig);
        return Load(_configPath);
    }
  }

  internal static Config Load(string configPath) {
    _configPath = configPath;

    if (!File.Exists(configPath)) {
      Console.WriteLine(string.Format("Could not find configuration file at {0}", configPath));
      Console.WriteLine("Creating default configuration file");
      return CreateDefault(configPath);
    }

    try {
      using (StreamReader reader = File.OpenText(configPath)) {
        string configFile = reader.ReadToEnd();
        reader.Close();

        if (string.IsNullOrEmpty(configFile) || string.IsNullOrWhiteSpace(configFile)) {
          BackupOldConfig();
          return CreateDefault(configPath);
        }

        Config? config = JsonConvert.DeserializeObject<Config>(configFile, new ProgramPathConverter());
        return MigrateConfig(config!);
      }
    } catch (Exception exception) {
      Log.Error(exception, "Failed to load config.");
      throw;
    }
  }

  private static void SaveConfig(string configPath, Config config) {
    try {
      if (File.Exists(Path.GetFileName(configPath))) {
        File.Copy(Path.GetFileName(configPath), string.Concat(Path.GetFileNameWithoutExtension(configPath), ".backup.json"), true);
      }
    } catch (Exception exception) {
      Log.Error(exception, "Failed to backup configuration file.");
      throw;
    }

    try {
      using (TextWriter textWriter = File.CreateText(configPath)) {
        var serializer = new JsonSerializer() {
          NullValueHandling = NullValueHandling.Ignore,
          TypeNameHandling = TypeNameHandling.Auto,
          Formatting = Formatting.Indented
        };

        serializer.Converters.Add(new ProgramPathConverter());

        using (JsonWriter jsonWriter = new JsonTextWriter(textWriter)) {
          serializer.Serialize(jsonWriter, config, config.GetType());
        }
      }
    } catch (Exception exception) {
      Log.Error(exception, string.Format("Failed to write configuration to file: {0}", configPath));
      throw;
    }
  }

  private void SaveConfig(string configPath) {
    SaveConfig(configPath, this);
  }

  internal void SaveConfig() => SaveConfig(_configPath);
}
