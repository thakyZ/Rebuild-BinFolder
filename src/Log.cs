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

  private static void DefaultMessage(LogLevel type) {
    Console.Write("[");
    PushForegroundColor(ConsoleColor.DarkGray);
    Console.Write(GetDate);
    PopForegroundColor();
    Console.Write("][");
    switch (type) {
      case LogLevel.Verbose:
        PushForegroundColor(ConsoleColor.DarkGray);
        Console.Write("VRB");
        break;
      case LogLevel.Debug:
        PushForegroundColor(ConsoleColor.Gray);
        Console.Write("DBG");
        break;
      case LogLevel.Info:
        PushForegroundColor(ConsoleColor.Blue);
        Console.Write("INF");
        break;
      case LogLevel.Warn:
        PushForegroundColor(ConsoleColor.Yellow);
        Console.Write("WRN");
        break;
      case LogLevel.Error:
        PushForegroundColor(ConsoleColor.Red);
        Console.Write("ERR");
        break;
      case LogLevel.Fatal:
        PushForegroundColor(ConsoleColor.DarkRed);
        Console.Write("FTL");
        break;
    }
    PopForegroundColor();
    Console.Write("] ");
  }

  internal static void Error(string message) {
    if (Services.Config.LogLevel <= LogLevel.Error) {
      DefaultMessage(LogLevel.Error);
      Console.Write($"{message}\n");
      WriteLine($"[{GetDate}][ERR] {message}");
    }
  }

  internal static void Error(Exception exception, string message) {
    if (Services.Config.LogLevel <= LogLevel.Error) {
      DefaultMessage(LogLevel.Error);
      Console.Write($"{message}\n");
      PushForegroundColor(ConsoleColor.DarkRed);
      Console.Write($"{exception.Message}\n{exception.StackTrace}\n");
      PopForegroundColor();
      WriteLine($"[{GetDate}][ERR] {message}\n{exception.Message}\n{exception.StackTrace}");
    }
  }

  internal static void Warn(string message) {
    if (Services.Config.LogLevel <= LogLevel.Warn) {
      DefaultMessage(LogLevel.Warn);
      PushForegroundColor(ConsoleColor.Yellow);
      Console.Write($"{message}\n");
      PopForegroundColor();
      WriteLine($"[{GetDate}][WRN] {message}");
    }
  }

  internal static void Info(string message) {
    if (Services.Config.LogLevel <= LogLevel.Info) {
      DefaultMessage(LogLevel.Info);
      Console.Write($"{message}\n");
      WriteLine($"[{GetDate}][INF] {message}");
    }
  }

  internal static void Debug(string message) {
    if (Services.Config.LogLevel <= LogLevel.Debug) {
      DefaultMessage(LogLevel.Debug);
      Console.Write($"{message}\n");
      WriteLine($"[{GetDate}][DBG] {message}");
    }
  }

  internal static void Additions(string message, string variable) {
    if (Services.Config.LogLevel <= LogLevel.Info) {
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
    if (Services.Config.LogLevel <= LogLevel.Info) {
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
