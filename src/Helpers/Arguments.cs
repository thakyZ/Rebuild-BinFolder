using Rebuild_BinFolder.Extensions;

namespace Rebuild_BinFolder.Helpers;

public interface IBaseArgument {
  public string[] CommandNames { get; }
  public string DictionaryKey { get; }
  public Action? Function { get; }
  public string HelpMessage { get; }
  public bool HasValue { get; }
  internal object? InternalValue { get; }
  public void CleanCommandNames(IEnumerable<string> otherCommands);
  public string Run();
}

internal abstract class BaseArgument : IBaseArgument {
  private string[] _commandNames = [];
  public string[] CommandNames {
    get => this._commandNames;
    private init {
      if (this._commandNames.Length == 0) {
        this._commandNames = ConvertCommandNames(value);
      }
    }
  }
  public string DictionaryKey { get; }
  public Action Function { get; }
  public string HelpMessage { get; }
  public bool HasValue => this.InternalValue is not null;
  public object? InternalValue { get; internal set; }

  protected BaseArgument(string[]? commandNames = null, string? dictionaryKey = null, Action? function = null, string? helpMessage = null) {
    this.CommandNames = commandNames ?? [];
    this.DictionaryKey = dictionaryKey ?? string.Empty;
    this.Function = function ?? (() => Log.Info(this.HelpMessage ?? string.Empty));
    this.HelpMessage = helpMessage ?? string.Empty;
    this.InternalValue = null;
  }

  private static string[] ConvertCommandNames(string[] commandNames) {
    List<string> output = [];
    foreach (var name in commandNames) {
      output.Add("-" + name.ToPascalCase());
      output.Add("/" + name.ToPascalCase());
      output.Add("-" + name.ToLower()[0]);
      output.Add("--" + name.ToSnakeCase());
    }
    return [..output];
  }

  public void CleanCommandNames(IEnumerable<string> otherCommands) {
    List<string> tempCommandNames = [];
    tempCommandNames.AddRange(this._commandNames.Where(x => !otherCommands.Contains(x)));
    this._commandNames = [..tempCommandNames];
  }

  public string Run() {
    this.Function?.Invoke();
    return this.DictionaryKey;
  }
}

public interface IArgument<out T> : IBaseArgument {
  public T? Value { get; }
}

internal abstract class Argument<T> : BaseArgument, IArgument<T> {
  public T? Value => (T?)this.InternalValue;
  protected Argument(string[]? commandNames = null, string? dictionaryKey = null, Action? function = null, string? helpMessage = null) : base(commandNames, dictionaryKey, function, helpMessage) {
  }
}

internal sealed class ArgumentEmpty<T> : Argument<T> {
  private ArgumentEmpty(T? value, string[]? commandNames = null, string? dictionaryKey = null, Action? function = null, string? helpMessage = null) : base(commandNames, dictionaryKey, function, helpMessage) {
    this.InternalValue = value;
  }
  public static ArgumentEmpty<T> Empty => new(default, null, null, null, null);
}

internal class ArgumentSwitch : Argument<bool> {
  public ArgumentSwitch(string[]? commandNames = null, string? dictionaryKey = null, Action? function = null, string? helpMessage = null) : base(commandNames, dictionaryKey, function, helpMessage) {
  }
}

internal sealed class Arguments {
  private readonly ArgumentSwitch _helpArgument = new(["help", "?"], "noHelp", null, "<Help Message>");

  private readonly IDictionary<string, string?> _passedArguments;

  private static Arguments? _instance;

  private List<IBaseArgument> PredefinedArguments { get; }

  private Arguments(string[] args) {
    this.PredefinedArguments = [
      this._helpArgument,
      new ArgumentSwitch(["quick"], "notQuick", null, "<Help Message>"),
      new ArgumentSwitch(["as system"], "asSystem", null, "<Help Message>"),
      new ArgumentSwitch(["as user"], "asSystem", null, "<Help Message>")
    ];
    this.PredefinedArguments.ForEach(x => x.CleanCommandNames(this.PredefinedArguments.SelectMany(y => y.CommandNames)));
    this._passedArguments = CommandLineArgumentsHelper.GetArgumentsDictionary(args);
    this.MapArguments();
  }

  internal static void Init(string[] args) {
    _instance ??= new Arguments(args);
  }

  private void MapArguments() {
    foreach (var (key, value) in this._passedArguments) {
      if (!this.PredefinedArguments.HasKey(key)) {
        // TODO: Print error if the argument does not exist.
        Log.Info($"Unknown argument {key}");
        break;
      }
    }
  }

  internal static IArgument<T> GetArgument<T>(string key) {
    if (_instance?.PredefinedArguments.Exists(x => x.GetType().IsGenericTypeDefinition && x.GetType().GenericTypeArguments[0] == typeof(T) && x.DictionaryKey == key) != true) {
      return ArgumentEmpty<T>.Empty;
    }
    return (IArgument<T>)_instance.PredefinedArguments.First(x => x.GetType().IsGenericTypeDefinition && x.GetType().GenericTypeArguments[0] == typeof(T) && x.DictionaryKey == key);
  }

  internal bool this[string key] => this._passedArguments.ContainsKey(key);
}
