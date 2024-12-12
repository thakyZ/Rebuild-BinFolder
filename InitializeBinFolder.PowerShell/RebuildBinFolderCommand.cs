using System.Diagnostics.CodeAnalysis;
using System.Management.Automation;  // Windows PowerShell assembly.

namespace InitializeBinFolder.PowerShell;

[Cmdlet(VerbsData.Initialize, "BinFolderCommand")]
public sealed class InitializeBinFolderCommand : Cmdlet {
  /// <summary>
  /// Placeholder parameter
  /// </summary>
  [SuppressMessage("Roslynator", "RCS1085:Use auto-implemented property")]
  [Parameter(Mandatory = true, Position = 0, HelpMessage = "Placeholder parameter")]
  public string? Name {
    get => this.name;
    set => this.name = value;
  }
  private string? name;

  protected override void BeginProcessing() {
  }

  protected override void ProcessRecord() {
    WriteObject($"Hello {name}!");
  }

  protected override void StopProcessing() {
  }

  protected override void EndProcessing() {
  }
}
