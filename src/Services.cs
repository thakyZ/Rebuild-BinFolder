using Rebuild_BinFolder.Configuration;
using Rebuild_BinFolder.Helpers;

namespace Rebuild_BinFolder;
internal class Services {
  internal static Config Config { get; private set; }
  internal static Arguments Arguments { get; }
  internal static Log Log { get; }

#pragma warning disable S1118 // Utility classes should not have public constructors
  internal Services(Config config) {
    Config = config;
  }
#pragma warning restore S1118 // Utility classes should not have public constructors

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
  static Services() {
    Arguments = new();
    Log = new();
  }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}
