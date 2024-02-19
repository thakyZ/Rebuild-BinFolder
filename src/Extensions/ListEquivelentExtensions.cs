using System.Text.RegularExpressions;

using Rebuild_BinFolder.Configuration;

namespace Rebuild_BinFolder.Extensions;
internal static class ListEquivalentExtensions {
  public static Dictionary<Regex, string> GetDictionary(this List<Equivalent> list) {
    return list.Select(x => new KeyValuePair<Regex, string>(x.Regex, x.Token)).ToDictionary(x => x.Key, x => x.Value);
  }
}
