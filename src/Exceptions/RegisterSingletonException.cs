using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rebuild_BinFolder.Exceptions;
public class RegisterSingletonException : Exception {
  public Type ParameterType { get; } = typeof(object);
  public string ParameterName { get; } = string.Empty;

  [Obsolete("Use a constructor that takes a Type and/or string for the parameter name instead.")]
  [SuppressMessage("Info Code Smell", "S1133:Deprecated code should be removed", Justification = "This constructor is required to exist, however should not be used.")]
  public RegisterSingletonException() {
  }

  [Obsolete("Use a constructor that takes a Type and/or string for the parameter name instead.")]
  [SuppressMessage("Info Code Smell", "S1133:Deprecated code should be removed", Justification = "This constructor is required to exist, however should not be used.")]
  public RegisterSingletonException(string? message) : base(message) {
  }

  [Obsolete("Use a constructor that takes a Type and/or string for the parameter name instead.")]
  [SuppressMessage("Info Code Smell", "S1133:Deprecated code should be removed", Justification = "This constructor is required to exist, however should not be used.")]
  public RegisterSingletonException(string? message, Exception? innerException) : base(message, innerException) {
  }

  public RegisterSingletonException(Type type) : base(ModifyMessage(type)) {
    ParameterType = type;
  }

  public RegisterSingletonException(Type type, string? message) : base(ModifyMessage(type, message)) {
    ParameterType = type;
  }

  public RegisterSingletonException(string paramName, Type type) : base(ModifyMessage(paramName, type)) {
    ParameterType = type;
    ParameterName = paramName;
  }

  public RegisterSingletonException(Type type, Exception? innerException) : base(ModifyMessage(type), innerException) {
    ParameterType = type;
  }

  public RegisterSingletonException(string paramName, Type type, Exception? innerException) : base(ModifyMessage(paramName, type), innerException) {
    ParameterType = type;
    ParameterName = paramName;
  }

  public RegisterSingletonException(Type type, string? message, Exception? innerException) : base(ModifyMessage(type, message), innerException) {
    ParameterType = type;
  }

  public RegisterSingletonException(string paramName, Type type, string? message, Exception? innerException) : base(ModifyMessage(paramName, type, message), innerException) {
    ParameterType = type;
    ParameterName = paramName;
  }

  protected static string ModifyMessage(Type type) {
    return $"Failed to add Singleton to list of singletons.\tType: {type.FullName}";
  }

  protected static string ModifyMessage(string paramName, Type type) {
    return $"Failed to add Singleton to list of singletons.\tType: {type.FullName}\tParameter Name: {paramName}";
  }

  protected static string ModifyMessage(Type type, string? message) {
    Trace.WriteLine($"Original exception message was \"{message ?? "null"}\"");
    var lowerInvariantMessage = message?.ToLowerInvariant() is not null ? $"{message.ToLowerInvariant()}" : "Failed to add Singleton to list of singletons.";
    return $"{lowerInvariantMessage}\tType: {type.FullName}";
  }

  protected static string ModifyMessage(string paramName, Type type, string? message) {
    Trace.WriteLine($"Original exception message was \"{message ?? "null"}\"");
    var lowerInvariantMessage = message?.ToLowerInvariant() is not null ? $"{message.ToLowerInvariant()}" : "Failed to add Singleton to list of singletons.";
    return $"{lowerInvariantMessage}\tType: {type.FullName}\tParameter Name: {paramName}";
  }
}
