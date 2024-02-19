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
    string[] prefixes = new string[4] { "-!", "-!+", "--!+", "/!" };
    List<string> output = new();

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
    CommandNames = new() { "-h", "-help", "--help", "-?", "/?" },
    DictionaryKey = "noHelp",
    HelpMessage = "<Help Message>",
  };

  private List<Argument> PredefinedArguments { get; }

  internal Arguments() {
    HelpArgument.Function = () => Log.Info(HelpArgument.HelpMessage);

    PredefinedArguments = new() {
      HelpArgument,
      new Argument() {
        CommandNames = new() { "-q", "-quick", "--quick" },
        DictionaryKey = "notQuick",
        Function = null,
        HelpMessage = "<Help Message>"
      }
    };
  }

  internal void HandleArguments(string[] args, out Dictionary<string, bool> output) {
    output = new() { { "noHelp", false }, { "isQuick", false } };
    if (args.Length == 0) {
      return;
    }

    foreach (var arg in args) {
      output[PredefinedArguments.FetchCommand(arg)] = true;
    }
  }
}
