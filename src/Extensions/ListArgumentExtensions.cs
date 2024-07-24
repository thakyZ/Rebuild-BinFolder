using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Rebuild_BinFolder.Helpers;

namespace Rebuild_BinFolder.Extensions;
public static class ListArgumentExtensions {
  internal static bool CheckArgumentType(this List<IBaseArgument> list, string key, Type type) {
    if (!list.Exists(x => x.DictionaryKey == key)) {
      return false;
    }
    Type item = list.First(x => x.DictionaryKey == key).GetType();
    if (!item.IsConstructedGenericType || !item.ContainsGenericParameters) {
      return false;
    }
    return Array.Exists(item.GenericTypeArguments, x => x.IsEquivalentTo(type));
  }
  /// <summary>
  /// TODO: Add Method Summary
  /// </summary>
  /// <param name="arguments"></param>
  /// <param name="arg"></param>
  /// <returns></returns>
  internal static string? FetchCommand(this List<IBaseArgument> arguments, string arg) {
    try {
      IBaseArgument? argument = arguments.Find(a => a.CommandNames.Contains(arg));
      return argument?.Run();
    } catch (Exception exception) {
      Log.Error(exception, "Failed to find arguments");
    }

    return "";
  }

  /// <summary>
  /// TODO: Add Method Summary
  /// </summary>
  /// <param name="commandNames"></param>
  /// <param name="arg"></param>
  /// <returns></returns>
  internal static bool ContainsCommandName(this List<string> commandNames, string arg) {
    List<string> commandNamePrefixes = commandNames.CommandNamesToPrefixes();
    return commandNamePrefixes.Contains(arg);
  }

  /// <summary>
  /// TODO: Add Method Summary
  /// </summary>
  /// <param name="commandNames"></param>
  /// <returns></returns>
  internal static List<string> CommandNamesToPrefixes(this List<string> commandNames) {
    string[] prefixes = ["-!", "-!+", "--!+", "/!"];
    List<string> output = [];

    foreach (var commandName in commandNames) {
      foreach (var prefix in prefixes) {
        if (commandName.Length > 1 && prefix.EndsWith("!+")) {
          output.Add(prefix.Replace("!+", commandName));
        } else if (prefix.EndsWith('!')) {
          output.Add(prefix.Replace('!', commandName[0]));
        }
      }
    }
    return output;
  }

  /// <summary>
  /// TODO: Add Method Summary
  /// </summary>
  /// <param name="list"></param>
  /// <param name="key"></param>
  /// <returns></returns>
  internal static bool HasKey(this List<IBaseArgument> list, string key) {
    return list.Exists(x => x.DictionaryKey == key);
  }

  /// <summary>
  /// TODO: Add Method Summary
  /// </summary>
  /// <param name="list"></param>
  /// <param name="key"></param>
  /// <returns></returns>
  internal static IBaseArgument GetFromKey(this List<IBaseArgument> list, string key) {
    return list.First(x => x.DictionaryKey == key);
  }

  /// <summary>
  /// TODO: Add Method Summary
  /// </summary>
  /// <param name="list"></param>
  /// <param name="key"></param>
  /// <param name="value"></param>
  /// <returns></returns>
  internal static bool SetKeyValue(this List<IBaseArgument> list, string key, string? value) {
    foreach (IBaseArgument item in list.Where(x => x.DictionaryKey == key)) {
      if (!list.HasKey(key) || (!list.CheckArgumentType(key, typeof(ArgumentSwitch)) && value is not null && list.CheckArgumentType(key, value.GetType()))) {
        continue;
      }
      Type genericType = item.GetType().GenericTypeArguments[0];
      if (genericType.IsEquivalentTo(typeof(string))) {
        if (value is null) {
          ((BaseArgument)item).InternalValue = default;
          return false;
        }
        ((BaseArgument)item).InternalValue = value;
        return true;
      }
      MethodInfo[] methods = genericType.GetMethods();
      if (Array.Exists(methods, x => x.Name.Equals("TryParse", StringComparison.OrdinalIgnoreCase))) {
        MethodInfo? method = Array.Find(methods, x => x.Name.Equals("TryParse", StringComparison.OrdinalIgnoreCase));
        if (method is null) {
          ((BaseArgument)item).InternalValue = default;
          return false;
        }
        var arguments = new object?[] { value, null };
        var attempt = method.Invoke(genericType, arguments);
        if (attempt is true) {
          ((BaseArgument)item).InternalValue = arguments[1];
          return true;
        }
      }
      ((BaseArgument)item).InternalValue = default;
      return false;
    }
    return false;
  }
}
