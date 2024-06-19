using Newtonsoft.Json;

namespace Rebuild_BinFolder.Configuration.Converters;
internal class VersionNumberMigrate : JsonConverter {
  public override bool CanConvert(Type objectType) {
    return true;
  }

  public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer) {
    try {
      if (reader.Value is string stringValue && int.TryParse(stringValue, out int result)) {
        return result;
      } else if (reader.Value is int intValue) {
        return intValue;
      } else if (reader.Value is long longValue) {
        return longValue;
      } else {
        throw new JsonReaderException($"Failed to parse \"version\" key, value. Value was type of {(reader.Value is null ? "null" : reader.Value.GetType())}, and value of {reader.Value ?? "null"}.");
      }
    } catch (Exception exception) {
      Log.Error(exception, "Failed to parse Version number.");
      throw;
    }
  }

  public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer) {
    try {
      if (value is long longValue) {
        writer.WriteValue(longValue);
      } else if (value is int intValue) {
        writer.WriteValue(intValue);
      } else if (value is string stringValue && int.TryParse(stringValue, out int result)) {
        writer.WriteValue(result);
      } else {
        throw new JsonWriterException($"Failed to parse given value. JSON object value was of type {(value is null ? "null" : value.GetType())}, and value of {value ?? "null"}.");
      }
    } catch (Exception exception) {
      Log.Error(exception, "Failed to parse stored Version number/");
      throw;
    }
  }
}
