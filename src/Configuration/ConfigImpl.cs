using System.Security.Principal;
using System.Text;

using Newtonsoft.Json;

using Rebuild_BinFolder.Exceptions;

namespace Rebuild_BinFolder.Configuration;

public partial class Config {
  /// <summary>
  /// TODO: Add field summary.
  /// </summary>
  private static JsonSerializer _serializer = new() {
    NullValueHandling = NullValueHandling.Include,
    DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
    TypeNameHandling = TypeNameHandling.None,
    Formatting = Formatting.Indented,
    ObjectCreationHandling = ObjectCreationHandling.Replace,
  };

  /// <summary>
  /// TODO: Add property summary.
  /// </summary>
  internal static bool Failed { get; }

  /// <summary>
  /// TODO: Add method summary.
  /// </summary>
  /// <param name="configPath"></param>
  /// <returns></returns>
  private static Config CreateDefault(string configPath) {
    Config config = new Config() {
      UserConfigs = [
        UserConfig.Empty
      ],
      Equivalents = Equivalent.Templates
    };

    config.SaveConfig(configPath);
    return config;
  }

  /// <summary>
  /// TODO: Add method summary.
  /// </summary>
  /// <returns></returns>
  private static string? GetCurrentUserSID() {
    try {
      return WindowsIdentity.GetCurrent().Owner!.ToString();
    } catch (Exception exception) {
      Log.Error(exception, "Could not get current user SID.");
      return null;
    }
  }

  /// <summary>
  /// TODO: Add method summary.
  /// </summary>
  /// <returns></returns>
  internal UserConfig? GetCurrentUserConfig() {
    return GetUserConfigBySID();
  }

  /// <summary>
  /// TODO: Add method summary.
  /// </summary>
  /// <param name="sid"></param>
  /// <returns></returns>
  internal UserConfig? GetUserConfigBySID(string sid) {
    try {
      var output = UserConfigs.Find(x => x.Info.SID == sid);
      if (output is null) {
        Log.Error($"UserConfig with SID, {sid}, not found... Creating blank.");
        var blank = UserConfig.Empty;
        UserConfigs.Add(blank);
        return blank;
      }
      return output;
    } catch (Exception exception) {
      Log.Error(exception, $"Failed to get User Config with SID, {sid}.");
      return null;
    }
  }

  /// <summary>
  /// TODO: Add method summary.
  /// </summary>
  /// <returns></returns>
  /// <exception cref="UserConfigException"></exception>
  internal UserConfig? GetUserConfigBySID() {
    try {
      var output = UserConfigs.Find(x => x.Info.SID == CurrentUserSID);
      if (output is null) {
        Log.Error($"UserConfig with SID, {CurrentUserSID}, not found... Creating blank.");
        throw new UserConfigException(CurrentUserSID, $"UserConfig with SID, {CurrentUserSID}, not found... Creating blank.");
      }
      return output;
    } catch (Exception exception) {
      Log.Error(exception, $"Failed to get User Config with SID, {CurrentUserSID}.");
      return null;
    }
  }

  /// <summary>
  /// TODO: Add method summary.
  /// </summary>
  private static void BackupOldConfig() {
    try {
      File.Move(_configPath, $"{_configPath}.{DateTime.Now.ToFileTimeUtc()}");
    } catch (Exception exception) {
      Log.Error(exception, "Failed to move config file to backup path");
      throw;
    }
  }

  /// <summary>
  /// TODO: Add method summary.
  /// </summary>
  /// <param name="configPath"></param>
  /// <returns></returns>
  /// <exception cref="NullVariableException"></exception>
  internal static Config Load(string configPath) {
    _configPath = configPath;
    Config? config = null;

    if (!File.Exists(configPath)) {
      Console.WriteLine(string.Format("Could not find configuration file at {0}", configPath));
      Console.WriteLine("Creating default configuration file");
      return CreateDefault(configPath);
    }

    try {
      using (StreamReader reader = File.OpenText(configPath)) {
        using (JsonReader jsonReader = new JsonTextReader(reader)) {
          config = _serializer.Deserialize<Config>(jsonReader);
        }
      }
    } catch (Exception exception) {
      if (config is null) {
        Log.Error(exception, "Failed to load config. It has returned null.");
        BackupOldConfig();
        return CreateDefault(configPath);
        throw;
      }
      Log.Error(exception, "Failed to load config.");
      throw;
    }
    if (config is not null) {
      config.SaveConfig($"{_configPath}.temp");
      return config;
    } else {
      throw new NullVariableException("Variable of parsed config returned null.");
    }
  }

  /// <summary>
  /// TODO: Add method summary.
  /// </summary>
  /// <param name="configPath"></param>
  /// <param name="config"></param>
  private static void SaveConfig(string configPath, Config config) {
    try {
      if (File.Exists(Path.GetFileName(configPath))) {
        File.Copy(Path.GetFileName(configPath), $"{Path.GetFileNameWithoutExtension(configPath)}.backup.json", true);
      }
    } catch (Exception exception) {
      Log.Error(exception, "Failed to backup configuration file.");
      throw;
    }

    using (StringWriter s_writer = new StringWriter(new StringBuilder()))

    try {
      using (TextWriter textWriter = File.CreateText(configPath)) {
        using (JsonWriter jsonWriter = new JsonTextWriter(textWriter)) {
          _serializer.Serialize(jsonWriter, config, config.GetType());
        }
      }
    } catch (Exception exception) {
      Log.Error(exception, string.Format("Failed to write configuration to file: {0}", configPath));
      throw;
    }
  }

  /// <summary>
  /// TODO: Add method summary.
  /// </summary>
  /// <param name="configPath"></param>
  private void SaveConfig(string configPath) {
    SaveConfig(configPath, this);
  }

  /// <summary>
  /// TODO: Add method summary.
  /// </summary>
  internal void SaveConfig() => SaveConfig(_configPath);

  internal static string? GetEnvironmentVariable(string value) {
    if (!Services.IsConfigNull() && Services.Config is Config Config) {
      if (Config.GetCurrentUserConfig() is UserConfig UserConfig && UserConfig.CustomEnvironmentVariables is List<CustomEnvironmentVar> uList && uList.Exists(x => x.Name == value)) {
        return uList.First(x => x.Name == value).ValueSingle;
      } else if (Config.AdminConfig is AdminConfig AdminConfig && AdminConfig.CustomEnvironmentVariables is List<CustomEnvironmentVar> aList && aList.Exists(x => x.Name == value)) {
        return aList.First(x => x.Name == value).ValueSingle;
      }
    }
    return null;
  }
}
