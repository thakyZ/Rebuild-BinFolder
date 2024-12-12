using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InitializeBinFolder.Common.Exceptions;
internal class FailedOperationException : Exception {
  public FailedOperationException() : base() {
  }

  public FailedOperationException(string? message) : base(message) {
  }

  public FailedOperationException(string? message, Exception? innerException) : base(message, innerException) {
  }
}
