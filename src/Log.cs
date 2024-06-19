using Newtonsoft.Json;

namespace Rebuild_BinFolder;

internal class Log {
  private static string LogFile => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "output.log");

  private static string GetDate => $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.ff}";

  private static ConsoleColor DefaultForegroundColor = ConsoleColor.White;

  private static void PushForegroundColor(ConsoleColor consoleColor) {
    DefaultForegroundColor = Console.ForegroundColor;
    Console.ForegroundColor = consoleColor;
  }

  private static void PopForegroundColor() {
    Console.ForegroundColor = DefaultForegroundColor;
  }

  private static bool TestLogLevel(LogLevel logLevel) {
    return Services.IsConfigNull() || Services.Config.LogLevel <= logLevel;
  }

  private static string LevelAbbrivation(LogLevel type) {
    return type switch {
      LogLevel.Verbose => "VRB",
      LogLevel.Debug => "DBG",
      LogLevel.Info => "INF",
      LogLevel.Warn => "WRN",
      LogLevel.Error => "ERR",
      LogLevel.Fatal => "FTL",
      _ => "UKN",
    };
  }

  private static void DefaultMessage(LogLevel type) {
    Console.Write("[");
    PushForegroundColor(ConsoleColor.DarkGray);
    Console.Write(GetDate);
    PopForegroundColor();
    Console.Write("][");
    var abbrivation = LevelAbbrivation(type);
    switch (type) {
      case LogLevel.Verbose:
        PushForegroundColor(ConsoleColor.DarkGray);
        break;
      case LogLevel.Debug:
        PushForegroundColor(ConsoleColor.Gray);
        break;
      case LogLevel.Info:
        PushForegroundColor(ConsoleColor.Blue);
        break;
      case LogLevel.Warn:
        PushForegroundColor(ConsoleColor.Yellow);
        break;
      case LogLevel.Error:
        PushForegroundColor(ConsoleColor.Red);
        break;
      case LogLevel.Fatal:
        PushForegroundColor(ConsoleColor.DarkRed);
        break;
    }
    Console.Write(abbrivation);
    PopForegroundColor();
    Console.Write("] ");
  }

  internal static void Error(string message) {
    if (TestLogLevel(LogLevel.Error)) {
      DefaultMessage(LogLevel.Error);
      Console.Write($"{message}\n");
      WriteLine($"[{GetDate}][ERR] {message}");
    }
  }

  internal static void Error(Exception exception, string message) {
    if (TestLogLevel(LogLevel.Error)) {
      DefaultMessage(LogLevel.Error);
      Console.Write($"{message}\n");
      PushForegroundColor(ConsoleColor.DarkRed);
      Console.Write($"{exception.Message}\n{exception.StackTrace}\n");
      PopForegroundColor();
      WriteLine($"[{GetDate}][ERR] {message}\n{exception.Message}\n{exception.StackTrace}");
    }
  }

  internal static void Warn(string message) {
    if (TestLogLevel(LogLevel.Warn)) {
      DefaultMessage(LogLevel.Warn);
      PushForegroundColor(ConsoleColor.Yellow);
      Console.Write($"{message}\n");
      PopForegroundColor();
      WriteLine($"[{GetDate}][WRN] {message}");
    }
  }

  internal static void Info(string message) {
    if (TestLogLevel(LogLevel.Info)) {
      DefaultMessage(LogLevel.Info);
      Console.Write($"{message}\n");
      WriteLine($"[{GetDate}][INF] {message}");
    }
  }

  internal static void Debug(string message) {
    if (TestLogLevel(LogLevel.Debug)) {
      DefaultMessage(LogLevel.Debug);
      Console.Write($"{message}\n");
      WriteLine($"[{GetDate}][DBG] {message}");
    }
  }

  internal static void Verbose(string message) {
    if (TestLogLevel(LogLevel.Verbose)) {
      DefaultMessage(LogLevel.Verbose);
      Console.Write($"{message}\n");
      WriteLine($"[{GetDate}][DBG] {message}");
    }
  }

  internal static void Object(object? obj, LogLevel level = LogLevel.Verbose) {
    if (TestLogLevel(level)) {
      DefaultMessage(level);
      string message = "unknown";
      if (obj is null) {
        message = "null";
      } else {
        if (obj.GetType().Name.Contains("List")) {
          message = JsonConvert.SerializeObject(obj, Formatting.Indented);
        } else if (obj.GetType().Name.Contains("Dictionary")) {
          message = JsonConvert.SerializeObject(obj, Formatting.Indented);
        }
      }
      Console.Write($"{message}\n");
      WriteLine($"[{GetDate}][{LevelAbbrivation(level)}] {message}");
    }
  }

  internal static void Additions(string message, string variable) {
    if (TestLogLevel(LogLevel.Info)) {
      DefaultMessage(LogLevel.Info);
      Console.Write($"{message} ");
      WriteLine($"[{GetDate}][INF] {message} {variable}");
      PushForegroundColor(ConsoleColor.Yellow);
      Console.Write($"{variable}");
      PopForegroundColor();
      Console.Write("\n");
    }
  }

  internal static void Changes(string message, string variable) {
    if (TestLogLevel(LogLevel.Info)) {
      DefaultMessage(LogLevel.Info);
      Console.Write($"{message}\n");
      WriteLine($"[{GetDate}][INF] {message}");
      PushForegroundColor(ConsoleColor.Yellow);
      foreach (var path in variable.Split(';')) {
        Console.Write($"    {path}");
        Console.Write("\n");
        WriteLine($"    {path}");
      }
      PopForegroundColor();
      Console.Write("\n");
    }
  }

  internal static void ResetLogFile() => File.WriteAllText(LogFile, string.Empty);

  private static void WriteLine(string message) {
    WriteToFile($"{message}\n");
  }

  private static void WriteToFile(string message) {
    using (StreamWriter writer = File.AppendText(LogFile)) {
      writer.Write(message);
    }
  }

  public static Log Get => Services.Log;
}

public enum LogLevel : uint {
  Trace = 0,
  Verbose = 1,
  Debug = 2,
  Info = 3,
  Warn = 4,
  Error = 5,
  Fatal = 6,
}
