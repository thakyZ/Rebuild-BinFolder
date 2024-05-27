using System.Text.RegularExpressions;

using Rebuild_BinFolder.Configuration;

namespace Rebuild_BinFolder.Extensions;
internal static class ListEquivalentExtensions {
  public static Dictionary<string, string> GetDictionary(this List<Equivalent> list) {
    return list.Select(x => new KeyValuePair<string, string>(x.Key, x.Token)).ToDictionary(x => x.Key, x => x.Value);
  }

  public static List<String> ToNativeString(this List<ProgramPath> list) {
    return list.ConvertAll(x => x.ToNativeString());
  }
}
