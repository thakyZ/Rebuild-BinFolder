using System.Diagnostics.CodeAnalysis;

using Rebuild_BinFolder.Configuration;
using Rebuild_BinFolder.Helpers;

using static Rebuild_BinFolder.HandlePaths.Handler;

namespace Rebuild_BinFolder;
internal sealed class Services {
  private static Services? _instance;
  private readonly Config _config;
  private readonly Log _log;
  private readonly RunState _runState;

  internal static RunState RunState => _instance!._runState;
  internal static Config Config => _instance!._config;
  internal static Log Log => _instance!._log;

  private Services(Config config, string[] args) {
    this._config = config;
    this._log = new Log();
    Arguments.Init(args);
    if (Arguments.GetArgument<bool>("asAdmin").Value && Arguments.GetArgument<bool>("asUser").Value) {
      this._runState = RunState.Both;
    } else if (Arguments.GetArgument<bool>("asAdmin").Value) {
      this._runState = RunState.Admin;
    } else {
      this._runState = RunState.User;
    }
  }

  internal static bool IsConfigNull() {
    return _instance?._config is null;
  }

  internal static void Init(Config config, string[] args) {
    _instance ??= new Services(config, args);
  }
}
