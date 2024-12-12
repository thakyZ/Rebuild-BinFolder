using System.Collections.Generic;
using CommandLine;

namespace InitializeBinFolder.Standalone;

/// <summary>
/// The entry point <see langword="class" /> for the <see cref="Standalone" /> tool.
/// </summary>
internal static class Program {
  /// <summary>
  /// The entry point <see langword="method" /> for the <see cref="Standalone" /> tool.
  /// </summary>
  /// <param name="args">The arguments passed to by the environment.</param>
  internal static void Main(string[] args) {
    // Parse the command line arguments.
    Parser.Default.ParseArguments<CommandLineOptions>(args)
      // Handle if arguments successfully parsed.
      .WithParsed((CommandLineOptions options) => {
        Logger.Information("Hello, {0}!", options.Name);
      })
      // Handle if any arguments failed to be parsed.
      .WithNotParsed((IEnumerable<Error> errors) => {
        foreach (var error in errors) {
          Logger.HandleError(error);
        }
      });
  }
}
