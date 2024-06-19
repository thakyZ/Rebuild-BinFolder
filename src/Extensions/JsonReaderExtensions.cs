using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace Rebuild_BinFolder.Extensions;
public static class JsonReaderExtensions {
  public static (int LineNumber, int LinePosition) GetInformation(this JsonReader jsonReader) {
    if (jsonReader is not JsonTextReader jsonTextReader || !jsonTextReader.HasLineInfo()) {
      return (0, 0);
    }
    return (jsonTextReader.LineNumber, jsonTextReader.LinePosition);
  }
}
