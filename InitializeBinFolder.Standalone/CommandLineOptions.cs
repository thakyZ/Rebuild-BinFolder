using CommandLine;

namespace InitializeBinFolder.Standalone;

internal class CommandLineOptions {
  [Option(Default = null, Required = true, HelpText = "Placeholder parameter")]
  public string? Name { get; set; }
}
