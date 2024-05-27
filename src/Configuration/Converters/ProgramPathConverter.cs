using Newtonsoft.Json;

using Rebuild_BinFolder.Exceptions;

namespace Rebuild_BinFolder.Configuration.Converters;

public class ProgramPathConverter : JsonConverter<ProgramPath> {
  public override void WriteJson(JsonWriter writer, ProgramPath? value, JsonSerializer serializer) {
    writer.WriteValue((value ?? new ProgramPath("null")).ToString());
  }

  public override ProgramPath ReadJson(JsonReader reader, Type objectType, ProgramPath? existingValue, bool hasExistingValue, JsonSerializer serializer) {
    if (objectType != typeof(string)) goto _throw;

    if (reader.Value is not string value)
      goto _throw;

    return new ProgramPath(value.Replace(@"\\", "/").Replace("/", @"\"));

    _throw:
      throw new InvalidPathException($"Failed to parse Program Path entry at, {reader.Depth}:0");
  }
}
