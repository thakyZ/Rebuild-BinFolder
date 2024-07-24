using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json;

using Rebuild_BinFolder.Exceptions;
using Rebuild_BinFolder.Extensions;

namespace Rebuild_BinFolder.Configuration.Converters;

public class ProgramPathConverter : JsonConverter<ProgramPath> {
  [SuppressMessage("Major Code Smell", "S907:\"goto\" statement should not be used", Justification = "Using goto is extremely cleaner")]
  public override ProgramPath ReadJson(JsonReader reader, Type objectType, ProgramPath? existingValue, bool hasExistingValue, JsonSerializer serializer) {
    if (reader.Value is not string value) {
      goto _throw;
    }

    return new ProgramPath(value);

    _throw:
      var (lineNumber, linePosition) = reader.GetInformation();
      throw new InvalidPathException($"Failed to parse Program Path entry at {lineNumber}:{linePosition}");
  }

  public override void WriteJson(JsonWriter writer, ProgramPath? value, JsonSerializer serializer) {
    value ??= ProgramPath.Empty;
    writer.WriteValue(value.ToRawString());
  }
}
