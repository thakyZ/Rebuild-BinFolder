using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Security.Principal;

namespace Rebuild_BinFolder;
internal static partial class ExternalFunctions {
  [SupportedOSPlatform("linux")]
  internal static uint GetEUid() {
    try {
      return ExternalFunctionsImpl.geteuid();
    } catch (Exception exception) {
      Log.Error(exception, "Failed to run native command \"geteuid\".");
      return uint.MaxValue;
    }
  }

  /// <summary>
  /// <see href="https://medium.com/@asimmon/programmatically-elevate-a-net-application-on-any-platform-726288b672a8" />
  /// </summary>
  /// <returns></returns>
  public static bool IsCurrentProcessElevated() {
    if (OperatingSystem.IsLinux()) {
      // https://github.com/dotnet/maintenance-packages/blob/62823150914410d43a3fd9de246d882f2a21d5ef/src/Common/tests/TestUtilities/System/PlatformDetection.Unix.cs#L58
      // 0 is the ID of the root user
      return GetEUid() == 0;
    }

    // https://github.com/dotnet/sdk/blob/v6.0.100/src/Cli/dotnet/Installer/Windows/WindowsUtils.cs#L38
    using var identity = WindowsIdentity.GetCurrent();
    var principal = new WindowsPrincipal(identity);
    return principal.IsInRole(WindowsBuiltInRole.Administrator);
  }

  private static partial class ExternalFunctionsImpl {
    [SupportedOSPlatform("linux")]
    [LibraryImport("libc")]
    internal static partial uint geteuid();
  }
}
