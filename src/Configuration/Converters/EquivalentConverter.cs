using Newtonsoft.Json;

namespace Rebuild_BinFolder.Configuration.Converters;
internal class EquivalentConverter : JsonConverter<List<Equivalent>> {
  public override List<Equivalent>? ReadJson(JsonReader reader, Type objectType, List<Equivalent>? existingValue, bool hasExistingValue, JsonSerializer serializer) {
    List<Equivalent> output = [];
    JsonToken currentTokenType = reader.TokenType;
    if (currentTokenType.ToString() == "StartObject") {
      while (currentTokenType.ToString() != "EndObject") {
        if (currentTokenType.ToString() == "StartObject") {
          if (!reader.Read()) {
            break;
          }
        } else if (currentTokenType.ToString() == "PropertyName" && reader.Value is not null) {
          var key = reader.Value.ToString() ?? "<error>";
          var val = reader.ReadAsString() ?? "<error>";
          output.Add(new Equivalent(key, val));
          if (!reader.Read()) {
            break;
          }
        } else if (reader.Value is null) {
          throw new JsonReaderException("Failed to find value", reader.Path, reader.Depth, 0, null);
        }
        currentTokenType = reader.TokenType;
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

  public override void WriteJson(JsonWriter writer, List<Equivalent>? values, JsonSerializer serializer) {
    writer.WriteStartObject();
    if (values is not null) {
      foreach (var value in values) {
        writer.WritePropertyName(value.Key);
        writer.WriteValue(value.Value);
      }
    }
    writer.WriteEndObject();
  }
}
