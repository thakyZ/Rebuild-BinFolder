using Microsoft.VisualStudio.TestPlatform.CoreUtilities.Helpers;

namespace Rebuild_BinFolder.Helpers;

internal static class ArgumentsExtension {
  internal static string FetchCommand(this List<Argument> _arguments, string arg) {
    try {
      var argument = _arguments.Find(a => a.CommandNames.Contains(arg));
      return argument.Run();
    } catch (Exception exception) {
      Log.Error(exception, "Failed to find arguments");
    }

    return "";
  }

  internal static bool ContainsCommandName(this List<string> commandNames, string arg) {
    List<string> commandNamePrefixes = commandNames.CommandNamesToPrefixes();
    return commandNamePrefixes.Contains(arg);
  }

  internal static List<string> CommandNamesToPrefixes(this List<string> commandNames) {
    string[] prefixes = ["-!", "-!+", "--!+", "/!"];
    List<string> output = [];

    foreach (string commandName in commandNames) {
      foreach (string prefix in prefixes) {
        if (commandName.Length > 1 && prefix.EndsWith("!+")) {
          output.Add(prefix.Replace("!+", commandName));
        } else if (prefix.EndsWith('!')) {
          output.Add(prefix.Replace('!', commandName[0]));
        }
      }
    }
    return output;
  }
}

internal struct Argument {
  internal List<string> CommandNames { get; set; }
  internal string DictionaryKey { get; set; }
  internal Action? Function { get; set; }
  internal string HelpMessage { get; set; }

  internal readonly string Run() {
    Function?.Invoke();
    return DictionaryKey;
  }
}

internal class Arguments {
  private Argument HelpArgument = new() {
    CommandNames = ["-h", "-help", "--help", "-?", "/?"],
    DictionaryKey = "noHelp",
    HelpMessage = "<Help Message>",
  };

  private readonly IDictionary<string, string?> _passedArguments;

  private List<Argument> PredefinedArguments { get; }

  internal Arguments(string[] args) {
    _passedArguments = CommandLineArgumentsHelper.GetArgumentsDictionary(args);
    HelpArgument.Function = () => Log.Info(HelpArgument.HelpMessage);

    PredefinedArguments = [
      HelpArgument,
      new Argument() {
        CommandNames = ["-q", "-quick", "--quick"],
        DictionaryKey = "notQuick",
        Function = null,
        HelpMessage = "<Help Message>"
      }
    ];
  }

  internal string? Get(string key) {
    if (this._passedArguments.TryGetValue(key, out string? value)) {
      return value;
    }
    return null;
  }

  internal string? GetString(string key) {
    return this.Get(key);
  }

  internal int? GetInt(string key) {
    if (CommandLineArgumentsHelper.TryGetIntArgFromDictionary(_passedArguments, key, out int value)) {
      return value;
    }
    return null;
  }

  internal bool this[string key] => this._passedArguments.ContainsKey(key);
}
