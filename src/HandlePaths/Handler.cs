// Ignore Spelling: Admin

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

using Rebuild_BinFolder.Configuration;
using Rebuild_BinFolder.Extensions;
using System.Diagnostics.CodeAnalysis;

namespace Rebuild_BinFolder.HandlePaths;
public partial class Handler {
  [GeneratedRegex(@"[A-Z]:\\")]
  private static partial Regex VolumeRegex();

  private readonly AsSystem? _asSystem;

  private readonly AsUser? _asUser;

  public enum RunState {
    Admin = 0,
    User  = 1,
    Both  = 2,
  }

  public Handler(string path, string auxiliaryPath) {
    var oldPath = path.Split(';').ToList();
    oldPath.RemoveAll(x => string.IsNullOrEmpty(x) || string.IsNullOrWhiteSpace(x));
    var oldAuxiliaryPath = auxiliaryPath.Split(';').ToList();
    oldAuxiliaryPath.RemoveAll(x => string.IsNullOrEmpty(x) || string.IsNullOrWhiteSpace(x));

    if (Services.RunState == RunState.Admin) {
      this._asSystem = new AsSystem(oldPath, oldAuxiliaryPath);
    }
    if (Services.RunState == RunState.User) {
      this._asUser = new AsUser(oldPath, oldAuxiliaryPath);
    }
  }

  public (ReturnedData? admin, ReturnedData? user) Run() {
    (ReturnedData? admin, ReturnedData? user) output = (null, null);

    if (Services.RunState == RunState.Admin) {
       output.admin = this._asSystem?.Run();
    }
    if (Services.RunState == RunState.User) {
      output.user = this._asUser?.Run();
    }

    return output;
  }
}
