using Newtonsoft.Json;

namespace Rebuild_BinFolder.Configuration.Converters;
internal class ProgramPathsConverter : JsonConverter<List<ProgramPath>> {
  public override List<ProgramPath> ReadJson(JsonReader reader, Type objectType, List<ProgramPath>? existingValue, bool hasExistingValue, JsonSerializer serializer) {
    if (reader is JsonTextReader textReader) {
      return this.ReadJson(textReader, objectType, existingValue, hasExistingValue, serializer);
    }
    return [];
  }

  [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "No need for it to be static or instanced.")]
  public List<ProgramPath> ReadJson(JsonTextReader reader, Type _, List<ProgramPath>? existingValue, bool hasExistingValue, JsonSerializer __) {
    List<ProgramPath> output = [];
    if (reader.TokenType == JsonToken.StartArray) {
      while (reader.TokenType != JsonToken.EndArray) {
        if (reader is { TokenType: JsonToken.String, Value: string val }) {
          output.Add(new ProgramPath(val));
        } else if (reader.TokenType != JsonToken.EndArray && reader.TokenType != JsonToken.StartArray) {
          throw new JsonException($"Invalid type. Expected String but got {reader.TokenType}. Position {reader.LineNumber}:{reader.LinePosition}");
        }
        if (!reader.Read()) {
          break;
        }
      }
    }
    if (output.Count == 0 && hasExistingValue && existingValue?.Count > 0) {
      return existingValue;
    }
    return output;
  }

  public override void WriteJson(JsonWriter writer, List<ProgramPath>? values, JsonSerializer serializer) {
    writer.WriteStartArray();
    if (values is not null) {
      foreach (ProgramPath value in values) {
        writer.WriteValue(value.ToRawString());
      }
    }
    writer.WriteEndArray();
  }
}
