using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Rebuild_BinFolder.Exceptions;
public class InvalidPathException : Exception {
  public InvalidPathException() {
  }

  public InvalidPathException(string? message) : base(message) {
  }

  public InvalidPathException(string? message, Exception? innerException) : base(message, innerException) {
  }
}
