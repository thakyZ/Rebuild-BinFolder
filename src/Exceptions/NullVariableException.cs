namespace Rebuild_BinFolder.Exceptions;
internal class NullVariableException : NullReferenceException {
  public NullVariableException() {
  }

  public NullVariableException(string? message) : base(message) {
  }
}
