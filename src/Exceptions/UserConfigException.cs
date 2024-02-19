using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace Rebuild_BinFolder.Exceptions;
public class UserConfigException : Exception {
  public string FetchedSID { get; } = string.Empty;

  [Obsolete("Use a constructor that takes a string for the UserSID instead.")]
  [SuppressMessage("Info Code Smell", "S1133:Deprecated code should be removed", Justification = "This constructor is required to exist, however should not be used.")]
  public UserConfigException() {
  }

  [Obsolete("Use a constructor that takes a string for the UserSID instead.")]
  [SuppressMessage("Info Code Smell", "S1133:Deprecated code should be removed", Justification = "This constructor is required to exist, however should not be used.")]
  public UserConfigException(string? message) : base(message) {
  }

  [Obsolete("Use a constructor that takes a string for the UserSID instead.")]
  [SuppressMessage("Info Code Smell", "S1133:Deprecated code should be removed", Justification = "This constructor is required to exist, however should not be used.")]
  public UserConfigException(string? message, Exception? innerException) : base(message, innerException) {
  }

  public UserConfigException(string sid, string? message) : base(ModifyMessage(sid, message)) {
    FetchedSID = sid;
  }

  public UserConfigException(string sid, string? message, Exception? innerException) : base(ModifyMessage(sid, message), innerException) {
    FetchedSID = sid;
  }

  protected static string ModifyMessage(string sid) {
    return $"Failed to get user config instance for current user SID.\tFetched SID: {sid}";
  }

  protected static string ModifyMessage(string sid, string? message) {
    Trace.WriteLine($"Original exception message was \"{message ?? "null"}\"");
    var lowerInvariantMessage = message?.ToLowerInvariant() is not null ? $"{message.ToLowerInvariant()}" : "Failed to get user config instance for current user SID.";
    return $"{lowerInvariantMessage}\tFetched SID: {sid}";
  }
}
