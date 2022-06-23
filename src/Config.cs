using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Rebuild_BinFolder {
  [Serializable]
  public  class Config {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    [JsonProperty("equivelents")]
    public Dictionary<string, string> Equivelents {
      get; set;
    }
    [JsonProperty("global_root")]
    public string GlobalRoot {
      get; set;
    }
    [JsonProperty("global_programs")]
    public List<string> GlobalPrograms {
      get; set;
    }
    [JsonProperty("local_programs")]
    public List<string> LocalPrograms {
      get; set;
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private static Config CreateDefault(string configPath) {
      Config config = new Config {
        Equivelents = new Dictionary<string, string>(),
        GlobalPrograms = new List<string>(),
        LocalPrograms = new List<string>()
      };
      config.SaveConfig(configPath);
      return config;
    }

    public static Config Load(string configPath) {
      _configPath = configPath;
      Config config;
      if (!File.Exists(configPath)) {
        Console.WriteLine($"Could not find configuration file at {configPath}");
        Console.WriteLine($"Creating default configuration file");
        config = CreateDefault(configPath);
        return config;
      }

      using (StreamReader reader = File.OpenText(configPath)) {
        Config? _config = JToken.ReadFromAsync(new JsonTextReader(reader)).Result.ToObject<Config>();
        config = _config is not null ? _config : CreateDefault(configPath);
      }

      return config;
    }

    [JsonIgnore]
    private static string _configPath = "";

    public void SaveConfig(string configPath) {
      var config = this;
      try {
        if (File.Exists(Path.GetFileName(configPath))) {
          File.Copy(Path.GetFileName(configPath), $"{Path.GetFileNameWithoutExtension(configPath)}.backup.json");
        }
      } catch (Exception e) {
        Log.Error("Failed to backup config file.");
        Log.Error($"{e.Message}");
        Log.Error($"{e.StackTrace}");
      }
      try {
        TextWriter tw = File.CreateText(configPath);
        var serializer = new JsonSerializer {
          NullValueHandling = NullValueHandling.Ignore,
          TypeNameHandling = TypeNameHandling.Auto,
          Formatting = Formatting.Indented
        };
        using (JsonWriter writer = new JsonTextWriter(tw)) {
          serializer.Serialize(writer, config, config.GetType());
        }
        tw.Close();
      } catch (Exception e) {
        Log.Error($"Failed to write config to file: {configPath}");
        Log.Error(e.Message);
        Log.Error($"{e.StackTrace} ");
      }
    }

    internal void SaveConfig() => SaveConfig(_configPath);
  }
}
