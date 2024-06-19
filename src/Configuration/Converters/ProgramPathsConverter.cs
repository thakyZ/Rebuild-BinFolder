using Newtonsoft.Json;

namespace Rebuild_BinFolder.Configuration.Converters;
internal class ProgramPathsConverter : JsonConverter<List<ProgramPath>> {
  public override List<ProgramPath>? ReadJson(JsonReader reader, Type objectType, List<ProgramPath>? existingValue, bool hasExistingValue, JsonSerializer serializer) {
    List<ProgramPath> output = [];
    if (reader.TokenType == JsonToken.StartArray) {
      while (reader.TokenType != JsonToken.EndArray) {
        if (reader.TokenType == JsonToken.StartArray) {
          if (!reader.Read()) {
            break;
          }
        } else if (reader.TokenType == JsonToken.String && reader.Value is string val) {
          output.Add(new(val));
          if (!reader.Read()) {
            break;
          }
        }
      }
    }
    if (output.Count == 0) {
      if (existingValue is not null && existingValue.Count > 0) {
        return existingValue;
      }
      return output;
    }
    return output;
  }

  public override void WriteJson(JsonWriter writer, List<ProgramPath>? values, JsonSerializer serializer) {
    writer.WriteStartArray();
    if (values is not null) {
      foreach (var value in values) {
        writer.WriteValue(value.ToRawString());
      }
    }
    writer.WriteEndArray();
  }
}
