using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Rebuild_BinFolder.Extensions;
public static partial class StringExtensions {
  /// <summary>
  /// TODO: Add Method Summary.
  /// <see href="https://learn.microsoft.com/en-us/answers/questions/1189474/is-there-a-way-to-capitalize-first-letters-of-each?source=docs"/>
  /// </summary>
  /// <param name="source"></param>
  /// <returns></returns>
  public static string ToTitleCase (this string source) => ToTitleCase(source, null);

  /// <summary>
  /// TODO: Add Method Summary.
  /// <see href="https://learn.microsoft.com/en-us/answers/questions/1189474/is-there-a-way-to-capitalize-first-letters-of-each?source=docs"/>
  /// </summary>
  /// <param name="source"></param>
  /// <param name="culture"></param>
  /// <returns></returns>
  public static string ToTitleCase(this string source, CultureInfo? culture) {
    culture ??= CultureInfo.CurrentUICulture;
    return culture.TextInfo.ToTitleCase(source.ToLower());
  }

  /// <summary>
  /// TODO: Add Method Summary.
  /// </summary>
  /// <param name="source"></param>
  /// <returns></returns>
  public static string ToSnakeCase(this string source) {
    /*
     * Should match all of these strings
     * SoupCat
     * soupCat
     * Soup Cat
     * Soup cat
     * soup cat
     * soup Cat
     * Soup_Cat
     * Soup_cat
     * soup_cat
     * Soup_cat
     * Soup-Cat
     * Soup-cat
     * Soup-cat
     * soup-cat
     *
     * And result in:
     * soup_cat
     */
    var output = new StringBuilder();
    foreach (Match match in CaseMatchRegex().Matches(source)) {
      (int i, string Value)[] matches = match.Groups.Values.Where(x => x.Value != source).Select((x, i) => (i, x.Value)).ToArray();
      foreach (var (index, groupValue) in matches) {
        output.Append(groupValue.ToLower());
        if (index < matches.Length) {
          output.Append('_');
        }
      }
    }
    return output.ToString();
  }

  /// <summary>
  /// TODO: Add Method Summary.
  /// </summary>
  /// <param name="source"></param>
  /// <returns></returns>
  public static string ToKebabCase(this string source) {
    /*
     * Should match all of these strings
     * SoupCat
     * soupCat
     * Soup Cat
     * Soup cat
     * soup cat
     * soup Cat
     * Soup_Cat
     * Soup_cat
     * soup_cat
     * Soup_cat
     * Soup-Cat
     * Soup-cat
     * Soup-cat
     * soup-cat
     *
     * And result in:
     * soup-cat
     */
    var output = new StringBuilder();
    foreach (Match match in CaseMatchRegex().Matches(source)) {
      (int i, string Value)[] matches = match.Groups.Values.Where(x => x.Value != source).Select((x, i) => (i, x.Value)).ToArray();
      foreach (var (index, groupValue) in matches) {
        output.Append(groupValue.ToLower());
        if (index < matches.Length) {
          output.Append('-');
        }
      }
    }
    return output.ToString();
  }

  /// <summary>
  /// TODO: Add Method Summary.
  /// </summary>
  /// <param name="source"></param>
  /// <returns></returns>
  public static string ToPascalCase(this string source) => ToPascalCase(source, null);

  /// <summary>
  /// TODO: Add Method Summary.
  /// </summary>
  /// <param name="source"></param>
  /// <param name="culture"></param>
  /// <returns></returns>
  public static string ToPascalCase(this string source, CultureInfo? culture) {
    /*
     * Should match all of these strings
     * SoupCat
     * soupCat
     * Soup Cat
     * Soup cat
     * soup cat
     * soup Cat
     * Soup_Cat
     * Soup_cat
     * soup_cat
     * Soup_cat
     * Soup-Cat
     * Soup-cat
     * Soup-cat
     * soup-cat
     *
     * And result in:
     * SoupCat
     */
    var output = new StringBuilder();
    foreach (Match match in CaseMatchRegex().Matches(source)) {
      foreach (var groupValue in match.Groups.Values.Select(x => x.Value).Where(x => x != source)) {
        output.Append(groupValue.ToTitleCase(culture));
      }
    }
    return output.ToString();
  }

  /// <summary>
  /// TODO: Add Method Summary.
  /// </summary>
  /// <param name="source"></param>
  /// <returns></returns>
  public static string ToCamelCase(this string source) => ToCamelCase(source, null);

  /// <summary>
  /// TODO: Add Method Summary.
  /// </summary>
  /// <param name="source"></param>
  /// <param name="culture"></param>
  /// <returns></returns>
  public static string ToCamelCase(this string source, CultureInfo? culture) {
    /*
     * Should match all of these strings
     * SoupCat
     * soupCat
     * Soup Cat
     * Soup cat
     * soup cat
     * soup Cat
     * Soup_Cat
     * Soup_cat
     * soup_cat
     * Soup_cat
     * Soup-Cat
     * Soup-cat
     * Soup-cat
     * soup-cat
     *
     * And result in:
     * soupCat
     */
    var output = new StringBuilder();
    foreach (Match match in CaseMatchRegex().Matches(source)) {
      foreach (var (index, groupValue) in match.Groups.Values.Where(x => x.Value != source).Select((x, i) => (i, x.Value))) {
        output.Append(index == 0 ? groupValue.ToLower(culture) : groupValue.ToTitleCase(culture));
      }
    }
    return output.ToString();
  }

  [GeneratedRegex("(?:([A-Z]?[a-z]+)[ _-]?)+")]
  private static partial Regex CaseMatchRegex();
}
