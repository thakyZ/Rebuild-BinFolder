using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Rebuild_BinFolder.Configuration;

[Serializable]
public class Equivalent {
  [JsonConverter(typeof(RegexConverter))]
  [JsonProperty("regex", NullValueHandling = NullValueHandling.Ignore)]
  [AllowNull]
  public Regex? Regex { get; set; }

  [JsonProperty("key", NullValueHandling = NullValueHandling.Include)]
  [AllowNull]
  public string Key { get; set; }

  [JsonProperty("token", NullValueHandling = NullValueHandling.Include)]
  public string Token { get; set; }

  [JsonConstructor]
  public Equivalent(string key, string _token, Regex? _regex = null) {
    if (_regex is not null) {
      this.Regex = null;
      this.Key = _regex.ToString();
    } else {
      this.Key = key;
    }
    this.Token = _token;
  }

  public static Equivalent Template => new(@"<userDir>\\AppData\\Roaming", "%APPDATA%");

  public static List<Equivalent> Templates => [
    Template,
    new Equivalent(@"<userDir>\\AppData\\Local", "%LOCALAPPDATA%"),
    new Equivalent("<userDir>", "%USERPROFILE%")
  ];
}
