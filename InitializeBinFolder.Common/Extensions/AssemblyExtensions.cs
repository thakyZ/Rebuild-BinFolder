using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace InitializeBinFolder.Common.Extensions;
/// <summary>
/// Extension methods for the <see cref="Assembly" /> type.
/// </summary>
internal static class AssemblyExtensions {
  public static string? GetRootNamespace(this Assembly assembly) {
    return assembly.GetTypes().FirstOrDefault()?.Namespace;
  }
}
