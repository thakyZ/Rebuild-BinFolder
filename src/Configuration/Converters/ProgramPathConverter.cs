using Newtonsoft.Json;

namespace Rebuild_BinFolder.Configuration.Converters;

public class ProgramPathConverter : JsonConverter<ProgramPath> {
  public override void WriteJson(JsonWriter writer, ProgramPath? value, JsonSerializer serializer) {
    writer.WriteValue((value ?? new ProgramPath("null")).ToString());
  }

  public override ProgramPath ReadJson(JsonReader reader, Type objectType, ProgramPath? existingValue, bool hasExistingValue, JsonSerializer serializer) {
    string value = (string)(reader.Value ?? "null");

    return new ProgramPath(value);
  }
}
