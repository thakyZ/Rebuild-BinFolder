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
  [GeneratedRegex("[A-Z]:\\\\")]
  private static partial Regex VolumeRegex();

  [NotNull, AllowNull]
  private AsSystem asSystem;

  [NotNull, AllowNull]
  private AsUser asUser;

  [Flags]
  public enum RunState {
    // Decimal     // Binary
    None  = 0,     // 000000
    Admin = 1,     // 000001
    User  = 2,     // 000010
    Both  = 4      // 000100
  }

  RunState currentRunState;

  public Handler(string path, string auxiliaryPath) {
    RunState runState = Services.RunState;
    // Equivalents = Singletons.Get<Config>().Equivalents.GetDictionary();
    var oldPath = path.Split(';').ToList();
    oldPath.RemoveAll(x => string.IsNullOrEmpty(x) || string.IsNullOrWhiteSpace(x));
    var oldAuxiliaryPath = auxiliaryPath.Split(';').ToList();
    oldAuxiliaryPath.RemoveAll(x => string.IsNullOrEmpty(x) || string.IsNullOrWhiteSpace(x));

    currentRunState = runState;
    if ((currentRunState & RunState.Admin) != 0) {
      asSystem = new(oldPath, oldAuxiliaryPath);
    }
    if ((currentRunState & RunState.User) != 0) {
      asUser = new(oldPath, oldAuxiliaryPath);
    }
  }

  public (ReturnedData? admin, ReturnedData? user) Run() {
    (ReturnedData? admin, ReturnedData? user) output = (null, null);

    if ((currentRunState & RunState.Admin) != 0) {
       output.admin = asSystem.Run();
    }

    if ((currentRunState & RunState.User) != 0) {
      output.user = asUser.Run();
    }

    return output;
  }
}
