using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rebuild_BinFolder {
  internal static class Log {
    private static string LogFile => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "output.log");
    internal static void Error(string message) {
      var date = new DateTime().ToString("yyyy-MM-dd HH:mm:ss.ff");
      WriteLine($"[{date}][ERR] {message}");
    }
    internal static void Error(Exception e, string message) {
      var date = new DateTime().ToString("yyyy-MM-dd HH:mm:ss.ff");
      WriteLine($"[{date}][ERR] {message}");
      WriteLine($"[{date}][ERR] {e.Message}");
      WriteLine($"[{date}][ERR] {e.StackTrace}");
    }
    internal static void Warn(string message) {
      var date = new DateTime().ToString("yyyy-MM-dd HH:mm:ss.ff");
      WriteLine($"[{date}][WRN] {message}");
    }
    internal static void Info(string message) {
      var date = new DateTime().ToString("yyyy-MM-dd HH:mm:ss.ff");
      WriteLine($"[{date}][INF] {message}");
    }
    internal static void Debug(string message) {
      var date = new DateTime().ToString("yyyy-MM-dd HH:mm:ss.ff");
      WriteLine($"[{date}][DBG] {message}");
    }
    internal static void ResetLogFile() => File.WriteAllText(LogFile, string.Empty);
    private static void WriteLine(string message) {
      Console.Write($"{message}\n");
      WriteToFile($"{message}\n");
    }
    private static void WriteToFile(string message) {
      using (StreamWriter writer = File.AppendText(LogFile)) {
        writer.Write(message);
        writer.Close();
      }
    }
  }
}
