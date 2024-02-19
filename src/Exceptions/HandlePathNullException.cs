namespace Rebuild_BinFolder.Exceptions;

public class HandlePathNullException : Exception {
  public HandlePathNullException(string? message) : base(message) {
  }

  public HandlePathNullException() : base() {
  }

  public HandlePathNullException(string? message, Exception? innerException) : base(message, innerException) {
  }
}
