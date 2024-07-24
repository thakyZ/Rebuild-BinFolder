using Newtonsoft.Json;

namespace Rebuild_BinFolder.Exceptions;
public class InvalidPathException : JsonReaderException {
  public InvalidPathException() {
  }

  public InvalidPathException(string message) : base(message) {
  }

  public InvalidPathException(string message, Exception innerException) : base(message, innerException) {
  }

  public InvalidPathException(string message, string path, int lineNumber, int linePosition, Exception? innerException) : base(message, path, lineNumber, linePosition, innerException) {
  }
}
