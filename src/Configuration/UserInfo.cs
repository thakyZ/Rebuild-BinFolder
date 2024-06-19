using System.Security.Principal;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using Rebuild_BinFolder.Exceptions;

namespace Rebuild_BinFolder.Configuration;

[JsonObject(MemberSerialization = MemberSerialization.OptIn, NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
public class UserInfo {
  /// <summary>
  /// TODO: Add property summary.
  /// </summary>
  [JsonProperty("sid", Order = 0)]
  public string SID { get; set; }

  /// <summary>
  /// TODO: Add property summary.
  /// </summary>
  [JsonProperty("username", Order = 1)]
  public string Username { get; set; }

  public UserInfo(string sid, string username) {
    this.SID = sid;
    this.Username = username;
  }

  /// <summary>
  /// TODO: Add property summary.
  /// </summary>
  internal static UserInfo Default {
    get {
      WindowsIdentity    currentUser = WindowsIdentity.GetCurrent();
      SecurityIdentifier currentOwner = currentUser.Owner ?? throw new NullVariableException("Current Windows Identity Owner is null.");
      return new(currentOwner.ToString(), currentUser.Name);
    }
  }
}
