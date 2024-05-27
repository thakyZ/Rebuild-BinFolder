using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Rebuild_BinFolder.Extensions;
internal class OrderedMap<TKey, TValue> : OrderedDictionary where TKey : notnull {
  private sealed class ReverseComparer : IComparer
  {
    public int Compare(object? x, object? y) => new CaseInsensitiveComparer().Compare(y, x);
  }

  private sealed class OrderedMapMJsonConverter : System.Text.Json.Serialization.JsonConverter<OrderedMap<TKey, TValue>>
  {
    public override bool CanConvert(Type typeToConvert) {
      throw new NotImplementedException();
    }

    public override OrderedMap<TKey, TValue>? Read(ref System.Text.Json.Utf8JsonReader reader, Type typeToConvert, System.Text.Json.JsonSerializerOptions options)
    {
      return [];
    }

    public override void Write(System.Text.Json.Utf8JsonWriter writer, OrderedMap<TKey, TValue> orderedMap, System.Text.Json.JsonSerializerOptions options)
    {
      if (orderedMap is null) {
        writer.WriteNullValue();
        return;
      }
      writer.WriteStartArray();
      foreach ((int index, TKey key, TValue? value) in orderedMap.GetIterator()) {
        writer.WriteStartObject();
        writer.WriteNumber("index", index);
        writer.WritePropertyName("key");
        writer.WriteRawValue(System.Text.Json.JsonSerializer.Serialize(key, typeof(TKey), options));
        writer.WritePropertyName("value");
        writer.WriteRawValue(System.Text.Json.JsonSerializer.Serialize(key, typeof(TKey), options));
        writer.WriteEndObject();
      }
      writer.WriteEndArray();
    }
  }
  private class OrderedMapNJsonConverter : Newtonsoft.Json.JsonConverter<OrderedMap<TKey, TValue>>
  {
    public override OrderedMap<TKey, TValue>? ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, OrderedMap<TKey, TValue>? existingValue, bool hasExistingValue, Newtonsoft.Json.JsonSerializer serializer)
    {
      OrderedMap<TKey, TValue> output = [];
      var jArray = Newtonsoft.Json.Linq.JToken.ReadFrom(reader);
      for (int i = 0; i < jArray.Count(); i++) {
        if (jArray[i] is Newtonsoft.Json.Linq.JToken item) {
          int? _index = (int?)item["index"];
          int index = _index ?? throw new NullReferenceException("Json value at index is null");
          TKey? _key = item["key"];
          TKey key = _key ?? throw new NullReferenceException("Json value at index is null");
          TValue? _value = item["value"];
          TValue value = _value ?? throw new NullReferenceException("Json value at index is null");
          output.Insert(index, key, value);
        }
      }
      return [];
    }

    public override void WriteJson(Newtonsoft.Json.JsonWriter writer, OrderedMap<TKey, TValue>? orderedMap, Newtonsoft.Json.JsonSerializer serializer)
    {
      if (orderedMap is null) {
        writer.WriteNull();
        return;
      }
      writer.WriteStartArray();
      foreach ((int index, TKey key, TValue? value) in orderedMap.GetIterator()) {
        writer.WriteStartObject();
        writer.WritePropertyName("index");
        writer.WriteValue(index);
        writer.WritePropertyName("key");
        writer.WriteRawValue(Newtonsoft.Json.JsonConvert.SerializeObject(key, type: typeof(TKey), formatting: serializer.Formatting, null));
        writer.WritePropertyName("value");
        writer.WriteRawValue(Newtonsoft.Json.JsonConvert.SerializeObject(value, type: typeof(TValue?), formatting: serializer.Formatting, null));
        writer.WriteEndObject();
      }
      writer.WriteEndArray();
    }
  }

  public OrderedMap() {
  }

  public OrderedMap(Dictionary<TKey, TValue> dictionary) {
  }

  public void Add(TKey key, TValue? value) {
    base.Add(key, value);
  }

  public void Remove(TKey key) {
    base.Remove(key);
  }

  public new void RemoveAt(int index) {
    base.RemoveAt(index);
  }

  public void Deconstruct(out List<(int index, TKey key, TValue? value)> list) {
    list = this.ToList().Select((kv, index) => (index, kv.Key, kv.Value)).ToList();
  }

  private List<KeyValuePair<TKey, TValue?>> ToList() {
    List<KeyValuePair<TKey, TValue?>> output = [];
    foreach (var obj in this.Keys) {
      var key = obj;
      var value = this[obj];
      output.Add(new(key, value));
    }
    return output;
  }

  public void SetAtIndex(int index, TKey key, TValue? value) {
    this.RemoveAt(index);
    this.Insert(index, key, value);
  }

  private void FromList(List<KeyValuePair<TKey, TValue?>> dict) {
    foreach ((int index, var item) in dict.Select((item, index) => (index, item))) {
      var nullCheck1 = this.Values[index] is null && item.Value is not null;
      var nullCheck2 = this.Values[index] is not null && item.Value is null;
      var nullCheck3 = this.Values[index] is TValue value && item.Value is not null && !value.Equals(item.Value);
      if (!this.Keys[index].Equals(item.Key) || nullCheck1 || nullCheck2 || nullCheck3) this.SetAtIndex(index, item.Key, item.Value);
    }
  }

  public TValue? this[TKey key] {
    get {
      return (TValue?)base[key];
    }
    set {
      base[key] = value;
    }
  }

  public new TValue? this[int index] {
    get {
      return (TValue?)base[index];
    }
    set {
      base[index] = value;
    }
  }

  public new List<TKey> Keys {
    get {
      return [..base.Keys.Cast<TKey>()];
    }
  }

  public new List<TValue?> Values {
    get {
      return [..base.Keys.Cast<TValue?>()];
    }
  }

  public IEnumerable<TResult> Select<TResult>(Func<KeyValuePair<TKey, TValue?>, int, TResult> selector) {
    return this.ToList().Select(selector);
  }

  public List<(int index, TKey key, TValue? value)> GetIterator() {
    return [..this.Select((item, index) => (index, item.Key, item.Value))];
  }


  public KeyValuePair<TKey, TValue?> AtIndex(int index) {
    return new(this.Keys[index], this[index]);
  }

  public void Sort(Comparison<KeyValuePair<TKey, TValue?>> sortFunc) {
    var tempList = this.ToList();
    tempList.Sort(sortFunc);
    this.FromList(tempList);
  }

  public bool Equals(List<KeyValuePair<TKey, TValue?>> list) {
    if (list.Count != this.Count) return false;
    for (int i = 0; i < this.Count; i++) {
      (TKey thisKey, TValue? thisValue) = this.AtIndex(i);
      (TKey listKey, TValue? listValue) = list[i];
      if (!thisKey.Equals(listKey)) return false;
      else if ((thisValue is null && listValue is not null) || (thisValue is not null && listValue is null)) return false;
      else if (thisValue is not null && !thisValue.Equals(listValue)) return false;
    }
    return true;
  }

  public bool Equals(OrderedMap<TKey, TValue?> dictionary) {
    if (this.GetHashCode().Equals(dictionary.GetHashCode())) return true;
    if (dictionary.Count != this.Count) return false;
    for (int i = 0; i < this.Count; i++) {
      (TKey thisKey, TValue? thisValue) = this.AtIndex(i);
      (TKey dictionaryKey, TValue? dictionaryValue) = dictionary.AtIndex(i);
      if (!thisKey.Equals(dictionaryKey)) return false;
      else if ((thisValue is null && dictionaryValue is not null) || (thisValue is not null && dictionaryValue is null)) return false;
      else if (thisValue is not null && !thisValue.Equals(dictionaryValue)) return false;
    }
    return true;
  }

  public override bool Equals(object? obj) {
    if (obj is null) return false;
    else if (obj is List<KeyValuePair<TKey, TValue?>> list) return this.Equals(list);
    else if (obj is OrderedMap<TKey, TValue?> dictionary) return this.Equals(dictionary);
    return false;
  }

  public override int GetHashCode() {
    var output = 0;
    foreach (var item in this) {
      HashCode.Combine(output, item.GetHashCode());
      HashCode.Combine(output, this[item]?.GetHashCode());
    }
    return output;
  }

  public override string ToString() {
    return "";
  }
}
