using System.Diagnostics.CodeAnalysis;

using Microsoft.VisualStudio.TestPlatform.CoreUtilities.Helpers;

using Rebuild_BinFolder.Configuration;
using Rebuild_BinFolder.Helpers;

using static Rebuild_BinFolder.HandlePaths.Handler;

namespace Rebuild_BinFolder;
internal class Services {
  [NotNull, AllowNull]
  private static Services? _instance;
  private readonly Config _config;
  private readonly Log _log;
  private readonly Arguments _arguments;
  private readonly RunState _runState = RunState.None;

  internal static RunState RunState => _instance._runState;
  internal static Config Config => _instance._config;
  internal static Log Log => _instance._log;
  internal static Arguments Arguments => _instance._arguments;

  private Services(Config config, string[] args) {
    _config = config;
    _log = new();
    _arguments = new(args);
    if (!_arguments["admin"]) {
      _runState |= RunState.User;
    } else {
      _runState |= RunState.Admin;
      if (_arguments["user"]) {
        _runState |= RunState.User;
      }
    }
  }

  internal static bool IsConfigNull() {
    if (_instance is null || _instance._config is null) {
      return true;
    }
    return false;
  }

  internal static void Init(Config config, string[] args) {
    _instance ??= new(config, args);
  }
}
