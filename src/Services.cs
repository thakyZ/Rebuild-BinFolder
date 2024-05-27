using System.Diagnostics.CodeAnalysis;

using Microsoft.VisualStudio.TestPlatform.CoreUtilities.Helpers;

using Rebuild_BinFolder.Configuration;
using Rebuild_BinFolder.Helpers;

namespace Rebuild_BinFolder;
internal class Services {
  [NotNull, AllowNull]
  private static Services? _instance;
  private readonly Config _config;
  private readonly Log _log;
  private readonly Arguments _arguments;

  internal static Config Config => _instance._config;
  internal static Log Log => _instance._log;
  internal static Arguments Arguments => _instance._arguments;

  private Services(Config config, string[] args) {
    _config = config;
    _log = new();
    _arguments = new(args);
  }

  internal static void Init(Config config, string[] args) {
    _instance ??= new(config, args);
  }
}
