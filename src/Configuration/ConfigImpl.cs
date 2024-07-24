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
    var config = new Config {
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
  private static string? GetCurrentUserSsid() {
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
    return this.GetUserConfigBySsid();
  }

  /// <summary>
  /// TODO: Add method summary.
  /// </summary>
  /// <param name="sid"></param>
  /// <returns></returns>
  internal UserConfig? GetUserConfigBySsid(string sid) {
    try {
      UserConfig? output = this.UserConfigs.Find(x => x.Info.SID == sid);
      if (output is not null) {
        return output;
      }

      Log.Error($"UserConfig with SID, {sid}, not found... Creating blank.");
      UserConfig blank = UserConfig.Empty;
      this.UserConfigs.Add(blank);
      return blank;
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
  internal UserConfig? GetUserConfigBySsid() {
    try {
      UserConfig? output = this.UserConfigs.Find(x => x.Info.SID == CurrentUserSID);
      if (output is not null) {
        return output;
      }

      Log.Error($"UserConfig with SID, {CurrentUserSID}, not found... Creating blank.");
      throw new UserConfigException(CurrentUserSID, $"UserConfig with SID, {CurrentUserSID}, not found... Creating blank.");
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
      Console.WriteLine($"Could not find configuration file at {configPath}");
      Console.WriteLine("Creating default configuration file");
      return CreateDefault(configPath);
    }

    try {
      using StreamReader reader = File.OpenText(configPath);
      using JsonReader jsonReader = new JsonTextReader(reader);
      config = _serializer.Deserialize<Config>(jsonReader);
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

    using var sWriter = new StringWriter(new StringBuilder());
    try {
      using TextWriter textWriter = File.CreateText(configPath);
      using JsonWriter jsonWriter = new JsonTextWriter(textWriter);
      _serializer.Serialize(jsonWriter, config, config.GetType());
    } catch (Exception exception) {
      Log.Error(exception, $"Failed to write configuration to file: {configPath}");
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
    if (Services.IsConfigNull() || Services.Config is not Config Config) {
      return null;
    }

    if (Config.GetCurrentUserConfig() is { CustomEnvironmentVariables: { } uList } && uList.Exists(x => x.Name == value)) {
      return uList.First(x => x.Name == value).ValueSingle;
    }

    if (Config.AdminConfig is { CustomEnvironmentVariables: { } aList } && aList.Exists(x => x.Name == value)) {
      return aList.First(x => x.Name == value).ValueSingle;
    }
    return null;
  }
}
