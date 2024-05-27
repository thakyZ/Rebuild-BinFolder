using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Rebuild_BinFolder.Helpers;
internal static class Elevation {
  public static async Task StartElevatedAsync(string[] args, CancellationToken cancellationToken) {
    var currentProcessPath = Environment.ProcessPath ?? (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
          ? Path.ChangeExtension(typeof(Program).Assembly.Location, "exe")
          : Path.ChangeExtension(typeof(Program).Assembly.Location, null));

    var processStartInfo = CreateProcessStartInfo(currentProcessPath, args);

    using var process = Process.Start(processStartInfo)
          ?? throw new InvalidOperationException("Could not start process.");

    await process.WaitForExitAsync(cancellationToken);
  }

  private static ProcessStartInfo CreateProcessStartInfo(string processPath, string[] args) {
    var startInfo = new ProcessStartInfo
      {
      UseShellExecute = true,
    };

    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
      ConfigureProcessStartInfoForWindows(startInfo, processPath, args);
    } else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) {
      ConfigureProcessStartInfoForMacOS(startInfo, processPath, args);
    } else { // Unix platforms
      ConfigureProcessStartInfoForLinux(startInfo, processPath, args);
    }

    return startInfo;
  }

  private static void ConfigureProcessStartInfoForWindows(ProcessStartInfo startInfo, string processPath, string[] args) {
    startInfo.Verb = "runas";
    startInfo.FileName = processPath;

    foreach (var arg in args) {
      startInfo.ArgumentList.Add(arg);
    }
  }

  private static void ConfigureProcessStartInfoForLinux(ProcessStartInfo startInfo, string processPath, string[] args) {
    startInfo.FileName = "sudo";
    startInfo.ArgumentList.Add(processPath);

    foreach (var arg in args) {
      startInfo.ArgumentList.Add(arg);
    }
  }

  private static void ConfigureProcessStartInfoForMacOS(ProcessStartInfo startInfo, string processPath, string[] args) {
    startInfo.FileName = "osascript";
    startInfo.ArgumentList.Add("-e");
    startInfo.ArgumentList.Add($"do shell script \"{processPath} {string.Join(' ', args)}\" with prompt \"MyProgram\" with administrator privileges");
  }
}
