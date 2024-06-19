using Newtonsoft.Json;

using Rebuild_BinFolder.Exceptions;
using Rebuild_BinFolder.Extensions;

namespace Rebuild_BinFolder.Configuration.Converters;

public class ProgramPathConverter : JsonConverter<ProgramPath> {
  public override ProgramPath ReadJson(JsonReader reader, Type objectType, ProgramPath? existingValue, bool hasExistingValue, JsonSerializer serializer) {
    if (reader.Value is not string value) {
      goto _throw;
    }

    return new(value);

    _throw:
      (int LineNumber, int LinePosition) = reader.GetInformation();
      throw new InvalidPathException($"Failed to parse Program Path entry at {LineNumber}:{LinePosition}");
  }

  public override void WriteJson(JsonWriter writer, ProgramPath? value, JsonSerializer serializer) {
    value ??= ProgramPath.Empty;
    writer.WriteValue(value.ToRawString());
  }
}
