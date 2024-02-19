using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Rebuild_BinFolder.Configuration;

[Serializable]
public class Equivalent {
  [JsonConverter(typeof(RegexConverter))]
  [JsonProperty("regex", NullValueHandling = NullValueHandling.Include)]
  [AllowNull]
  public Regex Regex { get; set; }

  [JsonProperty("token", NullValueHandling = NullValueHandling.Include)]
  public string Token { get; set; }

  [JsonConstructor]
  public Equivalent(Regex _regex, string _token) {
    this.Regex = _regex;
    this.Token = _token;
  }

  #pragma warning disable SYSLIB1045
  public static Equivalent Template => new(new(@"<userDir>\\AppData\\Roaming"), "%APPDATA%");

  public static List<Equivalent> Templates => new() {
    Template,
    new Equivalent(new(@"<userDir>\\AppData\\Local"), "%LOCALAPPDATA%"),
    new Equivalent(new("<userDir>"), "%USERPROFILE%")
  };
  #pragma warning restore
}
