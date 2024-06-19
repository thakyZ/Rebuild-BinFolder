using Newtonsoft.Json;

namespace Rebuild_BinFolder.Exceptions;
public class InvalidPathException : JsonReaderException {
  public InvalidPathException() {
  }

  public InvalidPathException(string message) : base(message) {
  }

  public InvalidPathException(string message, Exception innerException) : base(message, innerException) {
  }
}
