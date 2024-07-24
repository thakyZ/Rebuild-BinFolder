namespace Rebuild_BinFolder.Exceptions;
internal class NullVariableException : NullReferenceException {
  public NullVariableException() {
  }

  public NullVariableException(string? message) : base(message) {
  }

  public NullVariableException(string? message, Exception? innerException) : base(message, innerException) {
  }
}
