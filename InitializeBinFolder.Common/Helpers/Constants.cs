using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using InitializeBinFolder.Common.Attributes;
using InitializeBinFolder.Common.Extensions;

namespace InitializeBinFolder.Common.Helpers;

/// <summary>
/// Static class of constants for the rebuilding of the bin folder.
/// </summary>
internal static class Constants {
  /// <summary>
  /// Static class of default values for the constants.
  /// </summary>
  private static class Default;

  /// <summary>
  /// List of valid config file names.
  /// </summary>
  internal static string[] ValidConfigNames => ["config.json", "config.jsonc", "config.json5"];

  /// <summary>
  /// List of valid config file paths.
  /// </summary>
  internal static string[] ValidConfigPaths => [..Constants.ValidConfigNames.Select(name => Path.Combine(Constants.GetRoamingPath().FullName, name))];

  /// <summary>
  /// Gets the <see cref="DirectoryInfo" /> for the config file's location.
  /// <para>
  /// PowerShell config files will be in <c>%AppData%\Initialize-BinFolder</c>
  /// Where as the standalone binary will use the same location as the binary.
  /// </para>
  /// </summary>
  /// <returns>An instance of the <see cref="DirectoryInfo" /> for the config file's location.</returns>
  /// <exception cref="DirectoryNotFoundException">Thrown if the parent directory of the assembly file is not found. (Edge-case)</exception>
  private static DirectoryInfo GetRoamingPath() {
    var assembly = Assembly.GetExecutingAssembly();
    if (assembly.GetRootNamespace()?.Equals($"{nameof(InitializeBinFolder)}.PowerShell", StringComparison.Ordinal) == true) {
      return new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)));
    }
    return new DirectoryInfo(assembly.Location).Parent ?? throw new DirectoryNotFoundException("Parent directory of the assembly not found.");
  }

  /// <summary>
  /// Gets the config's <see cref="FileInfo" />, and if it does not exist, it will create a new blank file.
  /// </summary>
  /// <returns>The <see cref="FileInfo" /> of the config file.</returns>
  internal static FileInfo GetConfigFile() {
    if (ValidConfigPaths.Where(File.Exists).Select(path => new FileInfo(path)).FirstOrDefault() is not FileInfo fileInfo) {
      var newPath = ValidConfigPaths[0];
      using var fs = File.Open(newPath, FileMode.CreateNew, FileAccess.Write, FileShare.ReadWrite);
      fileInfo = new FileInfo(newPath);
    }
    return fileInfo;
  }

  /// <summary>
  /// Gets the <see cref="Uri" /> of the associated repository for this assembly.
  /// </summary>
  /// <returns>The <see cref="Uri" /> of the associated repository.</returns>
  internal static Uri? GetRepositoryUri() {
    var assembly = Assembly.GetExecutingAssembly();
    var name     = assembly.GetCustomAttribute<AssemblyRepositoryNameAttribute>()?.Value;
    var owner    = assembly.GetCustomAttribute<AssemblyRepositoryOwnerAttribute>()?.Value;

    if (name is null || owner is null) {
      return null;
    }

    return new Uri($"https://github.com/{owner}/{name}", UriKind.Absolute);
  }

  /// <summary>
  /// Gets the <see cref="Uri" /> of the schema for the config file.
  /// </summary>
  /// <returns>The <see cref="Uri" /> of the schema for the config file.</returns>
  internal static Uri? GetSchemaUri() {
    return GetRepositoryUri() is Uri uri ? new Uri($"{uri}/raw/main/config.schema.json", UriKind.Absolute) : null;
  }
}
