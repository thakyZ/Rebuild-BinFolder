using System;
using Microsoft.Extensions.Logging;
using Spectre.Console;
using Spectre.Console.Extensions.Logging;
using CommandLine;

namespace InitializeBinFolder.Standalone;

/// <summary>
/// Class to log to console.
/// </summary>
public sealed class Logger {
  /// <summary>
  /// Instance for the <see cref="Logger" /> class.
  /// </summary>
  private static readonly Lazy<Logger> _lazy = new(Logger.Init);

  /// <summary>
  /// Instance of the main <see cref="ILogger" /> tool.
  /// </summary>
  private readonly ILogger? _logger;

  /// <summary>
  /// Instance of the fallback <see cref="ILogger" /> tool.
  /// </summary>
  private readonly ILogger _fallbackLogger;

  /// <summary>
  /// Gets the main <see cref="ILogger" /> tool or if it is null then the fallback <see cref="ILogger" /> tool.
  /// </summary>
  private ILogger LoggingInstance => _logger ?? _fallbackLogger;

  /// <summary>
  /// Creates a new instance of <see cref="Logger" />.
  /// </summary>
  /// <param name="logger">The instance of the main <see cref="ILogger" /> tool.</param>
  private Logger(ILogger logger) {
    _logger = logger;
    using ILoggerFactory factory = LoggerFactory.Create((ILoggingBuilder b) => b.SetMinimumLevel(LogLevel.Trace).AddSpectreConsole(_config));
    _fallbackLogger = factory.CreateLogger("SampleCategory");
  }

  private static readonly SpectreConsoleLoggerConfiguration _config = new() {
    LogLevel = LogLevel.Trace,
    IncludeEventId = true,
    ConsoleSettings = new AnsiConsoleSettings {
      ColorSystem = ColorSystemSupport.Detect,
      Interactive = InteractionSupport.Yes,
      Ansi = AnsiSupport.Detect,
    },
  };

  /// <summary>
  /// Wrapper method for the lazy field <see cref="_lazy" /> that creates the default <see cref="Logger" /> instance.
  /// </summary>
  /// <returns>An instance of <see cref="Logger" />.</returns>
  private static Logger Init() {
    ILogger logger;
    using (ILoggerFactory factory = LoggerFactory.Create((ILoggingBuilder b) => {
#if DEBUG
      var temp =
#endif
      b.Configure((x) => x.ActivityTrackingOptions = ActivityTrackingOptions.Baggage)
#if DEBUG
      .SetMinimumLevel(LogLevel.Debug)
#else
      .SetMinimumLevel(LogLevel.Information)
#endif
      .AddSpectreConsole(_config)
#if DEBUG
      ;
      foreach (var item in temp.Services) {
          Console.WriteLine(item.GetType().FullName);
          Console.WriteLine(item);
          Console.WriteLine(item.ImplementationType);
          Console.WriteLine(item.ServiceType);
      }
    }
#endif
      )) {
      logger = factory.CreateLogger("SampleCategory");
    }
    return new Logger(logger);
  }

  /// <summary>
  /// Handles and prints <see cref="CommandLine" />.<see cref="Error" /> types.
  /// </summary>
  /// <param name="error">The instance of the <see cref="CommandLine" />.<see cref="Error" />.</param>
  internal static void HandleError(Error error) {
    if (error is BadFormatConversionError badFormatConversion) {
      Logger.Error("[{0}] Error, bad format conversion thrown for \"{1}\"!", badFormatConversion.Tag, badFormatConversion.NameInfo);
    } else if (error is BadFormatTokenError badFormatToken) {
      Logger.Error("[{0}] Error, bad format for token \"{1}\"!", badFormatToken.Tag, badFormatToken.Token);
    } else if (error is BadVerbSelectedError badVerbSelected) {
      Logger.Error("[{0}] Error, invalid verb selected for token of \"{1}\"!", badVerbSelected.Tag, badVerbSelected.Token);
    } else if (error is GroupOptionAmbiguityError groupOptionAmbiguity) {
      Logger.Error("[{0}] Error, ambiguous verb selected for option \"{1}\"!", groupOptionAmbiguity.Tag, groupOptionAmbiguity.Option, groupOptionAmbiguity.NameInfo);
    } else if (error is InvalidAttributeConfigurationError invalidAttributeConfiguration) {
      Logger.Error("[{0}] Error, invalid attribute configuration in code!", invalidAttributeConfiguration.Tag);
    } else if (error is MissingGroupOptionError missingGroupOption) {
      Logger.Error("[{0}] Error, missing a group option, \"{1}\", \"{2}\"!", "[{0}] Error, setting value of \"{1}\" to \"{2}\"!", missingGroupOption.Group, string.Join("\", \"", missingGroupOption.Names));
    } else if (error is MissingRequiredOptionError missingRequiredOption) {
      Logger.Error("[{0}] Error, missing a required option for \"{1}\"!", missingRequiredOption.Tag, missingRequiredOption.NameInfo);
    } else if (error is MissingValueOptionError missingValueOption) {
      Logger.Error("[{0}] Error, missing a value for option, \"{1}\"!", missingValueOption.Tag, missingValueOption.NameInfo);
    } else if (error is MultipleDefaultVerbsError multipleDefaultVerbs) {
      Logger.Error("[{0}] Error, multiple default verbs supplied!", multipleDefaultVerbs.Tag);
    } else if (error is NoVerbSelectedError noVerbSelected) {
      Logger.Error("[{0}] Error, no verb selected!", noVerbSelected.Tag);
    } else if (error is RepeatedOptionError repeatedOption) {
      Logger.Error("[{0}] Error, too many options of \"{1}\"!", repeatedOption.Tag, repeatedOption.NameInfo);
    } else if (error is SequenceOutOfRangeError sequenceOutOfRange) {
      Logger.Error("[{0}] Error, sequence of \"{1}\" is out of range!", sequenceOutOfRange.Tag, sequenceOutOfRange.NameInfo);
    } else if (error is SetValueExceptionError setValueException) {
      Logger.Error(setValueException.Exception, "[{0}] Error, setting value of \"{1}\" to \"{2}\"!", setValueException.Tag, setValueException.NameInfo, setValueException.Value);
    } else if (error is UnknownOptionError unknownOption) {
      Logger.Error("[{0}] Error, Unknown token \"{1}\"!", unknownOption.Tag, unknownOption.Token);
    } else if (error is TokenError token) {
      Logger.Error("[{0}] Error, with token of \"{1}\"!", token.Tag, token.Token);
    } else if (error is NamedError named) {
      Logger.Error("[{0}] Error, with name of \"{1}\"!", named.Tag, named.NameInfo);
    } else if (error is not VersionRequestedError or HelpRequestedError or HelpVerbRequestedError) {
      Logger.Error("[{0}] Unknown error!", error.Tag);
    }
  }

  /// <summary>
  /// Writes a message to the console with the specified log level.
  /// </summary>
  /// <param name="level">The level at which to log at.</param>
  /// <param name="message">The message to log.</param>
  /// <param name="args">The arguments for the message.</param>
  public static void Write(LogLevel level, string? message, params object?[] args) {
    WriteImpl(level, null, null, message, args);
  }

  /// <summary>
  /// Writes a message to the console with the specified log level.
  /// </summary>
  /// <param name="level">The level at which to log at.</param>
  /// <param name="exception">An exception to print.</param>
  /// <param name="message">The message to log.</param>
  /// <param name="args">The arguments for the message.</param>
  public static void Write(LogLevel level, Exception? exception, string? message, params object?[] args) {
    WriteImpl(level, null, exception, message, args);
  }

  /// <summary>
  /// Writes a message to the console with the specified log level.
  /// </summary>
  /// <param name="level">The level at which to log at.</param>
  /// <param name="eventId">The event id associated with this log message.</param>
  /// <param name="message">The message to log.</param>
  /// <param name="args">The arguments for the message.</param>
  public static void Write(LogLevel level, EventId eventId, string? message, params object?[] args) {
    WriteImpl(level, eventId, null, message, args);
  }

  /// <summary>
  /// Writes a message to the console with the specified log level.
  /// </summary>
  /// <param name="level">The level at which to log at.</param>
  /// <param name="eventId">The event id associated with this log message.</param>
  /// <param name="exception">An exception to print.</param>
  /// <param name="message">The message to log.</param>
  /// <param name="args">The arguments for the message.</param>
  public static void Write(LogLevel level, EventId eventId, Exception? exception, string? message, params object?[] args) {
    WriteImpl(level, eventId, exception, message, args);
  }

  /// <summary>
  /// Writes a message to the console with the specified log level.
  /// </summary>
  /// <param name="level">The level at which to log at.</param>
  /// <param name="eventId">The event id associated with this log message.</param>
  /// <param name="exception">An exception to print.</param>
  /// <param name="message">The message to log.</param>
  /// <param name="args">The arguments for the message.</param>
  private static void WriteImpl(LogLevel level, EventId? eventId, Exception? exception, string? message, params object?[] args) {
    switch (level) {
      case LogLevel.Trace when eventId.HasValue && exception is not null:
        Trace(eventId.Value, exception, message, args);
        break;
      case LogLevel.Trace when eventId.HasValue && exception is null:
        Trace(eventId.Value, message, args);
        break;
      case LogLevel.Trace when !eventId.HasValue && exception is not null:
        Trace(exception, message, args);
        break;
      case LogLevel.Trace:
        Trace(message, args);
        break;
      case LogLevel.Debug when eventId.HasValue && exception is not null:
        Debug(eventId.Value, exception, message, args);
        break;
      case LogLevel.Debug when eventId.HasValue && exception is null:
        Debug(eventId.Value, message, args);
        break;
      case LogLevel.Debug when !eventId.HasValue && exception is not null:
        Debug(exception, message, args);
        break;
      case LogLevel.Debug:
        Debug(message, args);
        break;
      case LogLevel.Information when eventId.HasValue && exception is not null:
        Information(eventId.Value, exception, message, args);
        break;
      case LogLevel.Information when eventId.HasValue && exception is null:
        Information(eventId.Value, message, args);
        break;
      case LogLevel.Information when !eventId.HasValue && exception is not null:
        Information(exception, message, args);
        break;
      case LogLevel.Information:
        Information(message, args);
        break;
      case LogLevel.Warning when eventId.HasValue && exception is not null:
        Warning(eventId.Value, exception, message, args);
        break;
      case LogLevel.Warning when eventId.HasValue && exception is null:
        Warning(eventId.Value, message, args);
        break;
      case LogLevel.Warning when !eventId.HasValue && exception is not null:
        Warning(exception, message, args);
        break;
      case LogLevel.Warning:
        Warning(message, args);
        break;
      case LogLevel.Error when eventId.HasValue && exception is not null:
        Error(eventId.Value, exception, message, args);
        break;
      case LogLevel.Error when eventId.HasValue && exception is null:
        Error(eventId.Value, message, args);
        break;
      case LogLevel.Error when !eventId.HasValue && exception is not null:
        Error(exception, message, args);
        break;
      case LogLevel.Error:
        Error(message, args);
        break;
      case LogLevel.Critical when eventId.HasValue && exception is not null:
        Critical(eventId.Value, exception, message, args);
        break;
      case LogLevel.Critical when eventId.HasValue && exception is null:
        Critical(eventId.Value, message, args);
        break;
      case LogLevel.Critical when !eventId.HasValue && exception is not null:
        Critical(exception, message, args);
        break;
      case LogLevel.Critical:
        Critical(message, args);
        break;
      case LogLevel.None when eventId.HasValue && exception is not null:
        None(eventId.Value, exception, message, args);
        break;
      case LogLevel.None when eventId.HasValue && exception is null:
        None(eventId.Value, message, args);
        break;
      case LogLevel.None when !eventId.HasValue && exception is not null:
        None(exception, message, args);
        break;
      case LogLevel.None:
      default:
        None(message, args);
        break;
    }
  }

#pragma warning disable CA2254 // Template should be a static expression
  /// <summary>
  /// Writes a message to the console with the <see cref="LogLevel.None" /> log level.
  /// </summary>
  /// <param name="message">The message to log.</param>
  /// <param name="args">The arguments for the message.</param>
  private static void None(string? message, params object?[] args) {
    _lazy.Value.LoggingInstance.Log(LogLevel.None, message, args);
  }

  /// <summary>
  /// Writes a message to the console with the <see cref="LogLevel.None" /> log level.
  /// </summary>
  /// <param name="exception">An exception to print.</param>
  /// <param name="message">The message to log.</param>
  /// <param name="args">The arguments for the message.</param>
  private static void None(Exception? exception, string? message, params object?[] args) {
    _lazy.Value.LoggingInstance.Log(LogLevel.None, exception, message, args);
  }

  /// <summary>
  /// Writes a message to the console with the <see cref="LogLevel.None" /> log level.
  /// </summary>
  /// <param name="eventId">The event id associated with this log message.</param>
  /// <param name="message">The message to log.</param>
  /// <param name="args">The arguments for the message.</param>
  private static void None(EventId eventId, string? message, params object?[] args) {
    _lazy.Value.LoggingInstance.Log(LogLevel.None, eventId, message, args);
  }

  /// <summary>
  /// Writes a message to the console with the <see cref="LogLevel.None" /> log level.
  /// </summary>
  /// <param name="eventId">The event id associated with this log message.</param>
  /// <param name="exception">An exception to print.</param>
  /// <param name="message">The message to log.</param>
  /// <param name="args">The arguments for the message.</param>
  private static void None(EventId eventId, Exception? exception, string? message, params object?[] args) {
    _lazy.Value.LoggingInstance.Log(LogLevel.None, eventId, exception, message, args);
  }

  /// <summary>
  /// Writes a message to the console with the <see cref="LogLevel.Trace" /> log level.
  /// </summary>
  /// <param name="message">The message to log.</param>
  /// <param name="args">The arguments for the message.</param>
  public static void Trace(string? message, params object?[] args) {
    _lazy.Value.LoggingInstance.LogTrace(message, args);
  }

  /// <summary>
  /// Writes a message to the console with the <see cref="LogLevel.Trace" /> log level.
  /// </summary>
  /// <param name="exception">An exception to print.</param>
  /// <param name="message">The message to log.</param>
  /// <param name="args">The arguments for the message.</param>
  public static void Trace(Exception? exception, string? message, params object?[] args) {
    _lazy.Value.LoggingInstance.LogTrace(exception, message, args);
  }

  /// <summary>
  /// Writes a message to the console with the <see cref="LogLevel.Trace" /> log level.
  /// </summary>
  /// <param name="eventId">The event id associated with this log message.</param>
  /// <param name="message">The message to log.</param>
  /// <param name="args">The arguments for the message.</param>
  public static void Trace(EventId eventId, string? message, params object?[] args) {
    _lazy.Value.LoggingInstance.LogTrace(eventId, message, args);
  }

  /// <summary>
  /// Writes a message to the console with the <see cref="LogLevel.Trace" /> log level.
  /// </summary>
  /// <param name="eventId">The event id associated with this log message.</param>
  /// <param name="exception">An exception to print.</param>
  /// <param name="message">The message to log.</param>
  /// <param name="args">The arguments for the message.</param>
  public static void Trace(EventId eventId, Exception? exception, string? message, params object?[] args) {
    _lazy.Value.LoggingInstance.LogTrace(eventId, exception, message, args);
  }

  /// <summary>
  /// Writes a message to the console with the <see cref="LogLevel.Debug" /> log level.
  /// </summary>
  /// <param name="message">The message to log.</param>
  /// <param name="args">The arguments for the message.</param>
  public static void Debug(string? message, params object?[] args) {
    _lazy.Value.LoggingInstance.LogDebug(message, args);
  }

  /// <summary>
  /// Writes a message to the console with the <see cref="LogLevel.Debug" /> log level.
  /// </summary>
  /// <param name="exception">An exception to print.</param>
  /// <param name="message">The message to log.</param>
  /// <param name="args">The arguments for the message.</param>
  public static void Debug(Exception? exception, string? message, params object?[] args) {
    _lazy.Value.LoggingInstance.LogDebug(exception, message, args);
  }

  /// <summary>
  /// Writes a message to the console with the <see cref="LogLevel.Debug" /> log level.
  /// </summary>
  /// <param name="eventId">The event id associated with this log message.</param>
  /// <param name="message">The message to log.</param>
  /// <param name="args">The arguments for the message.</param>
  public static void Debug(EventId eventId, string? message, params object?[] args) {
    _lazy.Value.LoggingInstance.LogDebug(eventId, message, args);
  }

  /// <summary>
  /// Writes a message to the console with the <see cref="LogLevel.Debug" /> log level.
  /// </summary>
  /// <param name="eventId">The event id associated with this log message.</param>
  /// <param name="exception">An exception to print.</param>
  /// <param name="message">The message to log.</param>
  /// <param name="args">The arguments for the message.</param>
  public static void Debug(EventId eventId, Exception? exception, string? message, params object?[] args) {
    _lazy.Value.LoggingInstance.LogDebug(eventId, exception, message, args);
  }

  /// <summary>
  /// Writes a message to the console with the <see cref="LogLevel.Information" /> log level.
  /// </summary>
  /// <param name="message">The message to log.</param>
  /// <param name="args">The arguments for the message.</param>
  public static void Info(string? message, params object?[] args) {
    _lazy.Value.LoggingInstance.LogInformation(message, args);
  }

  /// <summary>
  /// Writes a message to the console with the <see cref="LogLevel.Information" /> log level.
  /// </summary>
  /// <param name="exception">An exception to print.</param>
  /// <param name="message">The message to log.</param>
  /// <param name="args">The arguments for the message.</param>
  public static void Info(Exception? exception, string? message, params object?[] args) {
    _lazy.Value.LoggingInstance.LogInformation(exception, message, args);
  }

  /// <summary>
  /// Writes a message to the console with the <see cref="LogLevel.Information" /> log level.
  /// </summary>
  /// <param name="eventId">The event id associated with this log message.</param>
  /// <param name="message">The message to log.</param>
  /// <param name="args">The arguments for the message.</param>
  public static void Info(EventId eventId, string? message, params object?[] args) {
    _lazy.Value.LoggingInstance.LogInformation(eventId, message, args);
  }

  /// <summary>
  /// Writes a message to the console with the <see cref="LogLevel.Information" /> log level.
  /// </summary>
  /// <param name="eventId">The event id associated with this log message.</param>
  /// <param name="exception">An exception to print.</param>
  /// <param name="message">The message to log.</param>
  /// <param name="args">The arguments for the message.</param>
  public static void Info(EventId eventId, Exception? exception, string? message, params object?[] args) {
    _lazy.Value.LoggingInstance.LogInformation(eventId, exception, message, args);
  }

  /// <summary>
  /// Writes a message to the console with the <see cref="LogLevel.Information" /> log level.
  /// </summary>
  /// <param name="message">The message to log.</param>
  /// <param name="args">The arguments for the message.</param>
  public static void Information(string? message, params object?[] args) {
    Info(message, args);
  }

  /// <summary>
  /// Writes a message to the console with the <see cref="LogLevel.Information" /> log level.
  /// </summary>
  /// <param name="exception">An exception to print.</param>
  /// <param name="message">The message to log.</param>
  /// <param name="args">The arguments for the message.</param>
  public static void Information(Exception? exception, string? message, params object?[] args) {
    Info(exception, message, args);
  }

  /// <summary>
  /// Writes a message to the console with the <see cref="LogLevel.Information" /> log level.
  /// </summary>
  /// <param name="eventId">The event id associated with this log message.</param>
  /// <param name="message">The message to log.</param>
  /// <param name="args">The arguments for the message.</param>
  public static void Information(EventId eventId, string? message, params object?[] args) {
    Info(eventId, message, args);
  }

  /// <summary>
  /// Writes a message to the console with the <see cref="LogLevel.Information" /> log level.
  /// </summary>
  /// <param name="eventId">The event id associated with this log message.</param>
  /// <param name="exception">An exception to print.</param>
  /// <param name="message">The message to log.</param>
  /// <param name="args">The arguments for the message.</param>
  public static void Information(EventId eventId, Exception? exception, string? message, params object?[] args) {
    Info(eventId, exception, message, args);
  }

  /// <summary>
  /// Writes a message to the console with the <see cref="LogLevel.Warning" /> log level.
  /// </summary>
  /// <param name="message">The message to log.</param>
  /// <param name="args">The arguments for the message.</param>
  public static void Warn(string? message, params object?[] args) {
    _lazy.Value.LoggingInstance.LogWarning(message, args);
  }

  /// <summary>
  /// Writes a message to the console with the <see cref="LogLevel.Warning" /> log level.
  /// </summary>
  /// <param name="exception">An exception to print.</param>
  /// <param name="message">The message to log.</param>
  /// <param name="args">The arguments for the message.</param>
  public static void Warn(Exception? exception, string? message, params object?[] args) {
    _lazy.Value.LoggingInstance.LogWarning(exception, message, args);
  }

  /// <summary>
  /// Writes a message to the console with the <see cref="LogLevel.Warning" /> log level.
  /// </summary>
  /// <param name="eventId">The event id associated with this log message.</param>
  /// <param name="message">The message to log.</param>
  /// <param name="args">The arguments for the message.</param>
  public static void Warn(EventId eventId, string? message, params object?[] args) {
    _lazy.Value.LoggingInstance.LogWarning(eventId, message, args);
  }

  /// <summary>
  /// Writes a message to the console with the <see cref="LogLevel.Warning" /> log level.
  /// </summary>
  /// <param name="eventId">The event id associated with this log message.</param>
  /// <param name="exception">An exception to print.</param>
  /// <param name="message">The message to log.</param>
  /// <param name="args">The arguments for the message.</param>
  public static void Warn(EventId eventId, Exception? exception, string? message, params object?[] args) {
    _lazy.Value.LoggingInstance.LogWarning(eventId, exception, message, args);
  }

  /// <summary>
  /// Writes a message to the console with the <see cref="LogLevel.Warning" /> log level.
  /// </summary>
  /// <param name="message">The message to log.</param>
  /// <param name="args">The arguments for the message.</param>
  public static void Warning(string? message, params object?[] args) {
    Warn(message, args);
  }

  /// <summary>
  /// Writes a message to the console with the <see cref="LogLevel.Warning" /> log level.
  /// </summary>
  /// <param name="exception">An exception to print.</param>
  /// <param name="message">The message to log.</param>
  /// <param name="args">The arguments for the message.</param>
  public static void Warning(Exception? exception, string? message, params object?[] args) {
    Warn(exception, message, args);
  }

  /// <summary>
  /// Writes a message to the console with the <see cref="LogLevel.Warning" /> log level.
  /// </summary>
  /// <param name="eventId">The event id associated with this log message.</param>
  /// <param name="message">The message to log.</param>
  /// <param name="args">The arguments for the message.</param>
  public static void Warning(EventId eventId, string? message, params object?[] args) {
    Warn(eventId, message, args);
  }

  /// <summary>
  /// Writes a message to the console with the <see cref="LogLevel.Warning" /> log level.
  /// </summary>
  /// <param name="eventId">The event id associated with this log message.</param>
  /// <param name="exception">An exception to print.</param>
  /// <param name="message">The message to log.</param>
  /// <param name="args">The arguments for the message.</param>
  public static void Warning(EventId eventId, Exception? exception, string? message, params object?[] args) {
    Warn(eventId, exception, message, args);
  }

  /// <summary>
  /// Writes a message to the console with the <see cref="LogLevel.Error" /> log level.
  /// </summary>
  /// <param name="message">The message to log.</param>
  /// <param name="args">The arguments for the message.</param>
  public static void Error(string? message, params object?[] args) {
    _lazy.Value.LoggingInstance.LogError(message, args);
  }

  /// <summary>
  /// Writes a message to the console with the <see cref="LogLevel.Error" /> log level.
  /// </summary>
  /// <param name="exception">An exception to print.</param>
  /// <param name="message">The message to log.</param>
  /// <param name="args">The arguments for the message.</param>
  public static void Error(Exception? exception, string? message, params object?[] args) {
    _lazy.Value.LoggingInstance.LogError(exception, message, args);
  }

  /// <summary>
  /// Writes a message to the console with the <see cref="LogLevel.Error" /> log level.
  /// </summary>
  /// <param name="eventId">The event id associated with this log message.</param>
  /// <param name="message">The message to log.</param>
  /// <param name="args">The arguments for the message.</param>
  public static void Error(EventId eventId, string? message, params object?[] args) {
    _lazy.Value.LoggingInstance.LogError(eventId, message, args);
  }

  /// <summary>
  /// Writes a message to the console with the <see cref="LogLevel.Error" /> log level.
  /// </summary>
  /// <param name="eventId">The event id associated with this log message.</param>
  /// <param name="exception">An exception to print.</param>
  /// <param name="message">The message to log.</param>
  /// <param name="args">The arguments for the message.</param>
  public static void Error(EventId eventId, Exception? exception, string? message, params object?[] args) {
    _lazy.Value.LoggingInstance.LogError(eventId, exception, message, args);
  }

  /// <summary>
  /// Writes a message to the console with the <see cref="LogLevel.Critical" /> log level.
  /// </summary>
  /// <param name="message">The message to log.</param>
  /// <param name="args">The arguments for the message.</param>
  public static void Critical(string? message, params object?[] args) {
    _lazy.Value.LoggingInstance.LogCritical(message, args);
  }

  /// <summary>
  /// Writes a message to the console with the <see cref="LogLevel.Critical" /> log level.
  /// </summary>
  /// <param name="exception">An exception to print.</param>
  /// <param name="message">The message to log.</param>
  /// <param name="args">The arguments for the message.</param>
  public static void Critical(Exception? exception, string? message, params object?[] args) {
    _lazy.Value.LoggingInstance.LogCritical(exception, message, args);
  }

  /// <summary>
  /// Writes a message to the console with the <see cref="LogLevel.Critical" /> log level.
  /// </summary>
  /// <param name="eventId">The event id associated with this log message.</param>
  /// <param name="message">The message to log.</param>
  /// <param name="args">The arguments for the message.</param>
  public static void Critical(EventId eventId, string? message, params object?[] args) {
    _lazy.Value.LoggingInstance.LogCritical(eventId, message, args);
  }

  /// <summary>
  /// Writes a message to the console with the <see cref="LogLevel.Critical" /> log level.
  /// </summary>
  /// <param name="eventId">The event id associated with this log message.</param>
  /// <param name="exception">An exception to print.</param>
  /// <param name="message">The message to log.</param>
  /// <param name="args">The arguments for the message.</param>
  public static void Critical(EventId eventId, Exception? exception, string? message, params object?[] args) {
    _lazy.Value.LoggingInstance.LogCritical(eventId, exception, message, args);
  }
#pragma warning restore CA2254 // Template should be a static expression
}
