using System.Diagnostics.CodeAnalysis;
using System.Diagnostics;

using Rebuild_BinFolder.Helpers;

namespace Rebuild_BinFolder.Exceptions;
public class GetSingletonException : RegisterSingletonException {
  [Obsolete("Use a constructor that takes a Type and/or string for the parameter name instead.")]
  [SuppressMessage("Info Code Smell", "S1133:Deprecated code should be removed", Justification = "This constructor is required to exist, however should not be used.")]
  public GetSingletonException() {
  }

  [Obsolete("Use a constructor that takes a Type and/or string for the parameter name instead.")]
  [SuppressMessage("Info Code Smell", "S1133:Deprecated code should be removed", Justification = "This constructor is required to exist, however should not be used.")]
  public GetSingletonException(string? message) : base(message) {
  }

  [Obsolete("Use a constructor that takes a Type and/or string for the parameter name instead.")]
  [SuppressMessage("Info Code Smell", "S1133:Deprecated code should be removed", Justification = "This constructor is required to exist, however should not be used.")]
  public GetSingletonException(string? message, Exception? innerException) : base(message, innerException) {
  }

  public GetSingletonException(Type type) : base(type) {
  }

  public GetSingletonException(Type type, string? message) : base(type, message) {
  }

  public GetSingletonException(string paramName, Type type) : base(paramName, type) {
  }

  public GetSingletonException(Type type, Exception? innerException) : base(type, innerException) {
  }

  public GetSingletonException(string paramName, Type type, Exception? innerException) : base(paramName, type, innerException) {
  }

  public GetSingletonException(Type type, string? message, Exception? innerException) : base(type, message, innerException) {
  }

  public GetSingletonException(string paramName, Type type, string? message, Exception? innerException) : base(paramName, type, message, innerException) {
  }

  protected new static string ModifyMessage(Type type) {
    return $"Failed to add Singleton to list of singletons.\tType: {type.FullName}";
  }

  protected new static string ModifyMessage(string paramName, Type type) {
    return $"Failed to add Singleton to list of singletons.\tType: {type.FullName}\tParameter Name: {paramName}";
  }

  protected new static string ModifyMessage(Type type, string? message) {
    Trace.WriteLine($"Original exception message was \"{message ?? "null"}\"");
    var lowerInvariantMessage = message?.ToLowerInvariant() is not null ? $"{message.ToLowerInvariant()}" : "Failed to get Singleton from list of singletons.";
    return $"{lowerInvariantMessage}\tType: {type.FullName}";
  }

  protected new static string ModifyMessage(string paramName, Type type, string? message) {
    Trace.WriteLine($"Original exception message was \"{message ?? "null"}\"");
    var lowerInvariantMessage = message?.ToLowerInvariant() is not null ? $"{message.ToLowerInvariant()}" : "Failed to get Singleton from list of singletons.";
    return $"{lowerInvariantMessage}\tType: {type.FullName}\tParameter Name: {paramName}";
  }
}
