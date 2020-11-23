namespace TransformationSpace.Serializa {
  using System;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.Diagnostics.CodeAnalysis;
  using System.Numerics;
  using System.Text;
  using System.Text.RegularExpressions;

  using Newtonsoft.Json;
  using Newtonsoft.Json.Linq;

  public class Vector3JsonConverter : JsonConverter {
    public override bool CanConvert(Type objectType) {
      return (objectType == typeof(Vector3));
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
      var V = (Vector3)value;
      serializer.Serialize(writer, new float[] { V.X, V.Y, V.Z });
    }
    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
      var Passes = serializer.Deserialize<float[]>(reader);
      return new Vector3((Passes[0]), (Passes[1]), (Passes[2]));
    }
  }
  public class QuaternionJsonConverter : JsonConverter {
    public override bool CanConvert(Type objectType) {
      return (objectType == typeof(Quaternion));
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
      var Q = (Quaternion)value;
      serializer.Serialize(writer, new float[] { Q.X, Q.Y, Q.Z, Q.W });
    }
    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
      var V = serializer.Deserialize<float[]>(reader);
      return new Quaternion(V[0], V[1], V[2], V[3]);
    }
  }

  public class SpaceObjectJsonConverter : JsonConverter<SpaceObject> {
    public override void WriteJson(JsonWriter writer, [AllowNull] SpaceObject value, JsonSerializer serializer) {
      writer.WriteStartObject();
      writer.WritePropertyName(nameof(value.Name));
      writer.WriteValue(value.Name);
      writer.WritePropertyName(nameof(value.Order));
      writer.WriteValue(value.Order);
      writer.WritePropertyName(nameof(value.LocalPosition));
      serializer.Serialize(writer, value.LocalPosition);
      writer.WritePropertyName(nameof(value.LocalRotation));
      serializer.Serialize(writer, value.LocalRotation);
      writer.WritePropertyName(nameof(value.LocalScale));
      serializer.Serialize(writer, value.LocalScale);
      writer.WritePropertyName(nameof(value.Parent));
      writer.WriteNull();
      writer.WritePropertyName(nameof(value.Children));
      serializer.Serialize(writer, value.Children);
      writer.WriteEndObject();
    }
    public override SpaceObject ReadJson(JsonReader reader, Type objectType, [AllowNull] SpaceObject existingValue, bool hasExistingValue, JsonSerializer serializer) {
      var Target = new SpaceObject();
      do {
        reader.Read();
        if (reader.TokenType == JsonToken.PropertyName) {
          switch (reader.Value) {
            case nameof(SpaceObject.Name):
              Target.Name = reader.ReadAsString();
              break;
            case nameof(SpaceObject.Order):
              Target.Order = reader.ReadAsInt32().Value;
              break;
            case nameof(SpaceObject.LocalPosition):
              reader.Read();
              Target.LocalPosition = serializer.Deserialize<Vector3>(reader);
              break;
            case nameof(SpaceObject.LocalRotation):
              reader.Read();
              Target.LocalRotation = serializer.Deserialize<Quaternion>(reader);
              break;
            case nameof(SpaceObject.LocalScale):
              reader.Read();
              Target.LocalScale = serializer.Deserialize<Vector3>(reader);
              break;
            case nameof(SpaceObject.Children):
              reader.Read();
              var P = serializer.Deserialize<SpaceObject[]>(reader);
              for (int i = 0; i < P.Length; i++) {
                Target.Children.Add(P[i]);
              }
              break;
            default:
              break;
          }
        }
        else if (reader.TokenType == JsonToken.EndObject) {
          break;
        }
      } while (true);
      return Target;
    }
  }

  public static class Serialization {
    /// <summary>
    /// Vector3 <-De/Serialize-> [X,Y,Z]
    /// </summary>
    public static readonly Vector3JsonConverter Vector3Converter;
    /// <summary>
    /// Quaternion <-De/Serialize-> [X,Y,Z,W]
    /// </summary>
    public static readonly QuaternionJsonConverter QuaternionConverter;
    /// <summary>
    /// SpaceObject <-De/Serialize-> {}
    /// </summary>
    public static readonly SpaceObjectJsonConverter SpaceObjectConverter;
    /// <summary>
    /// Default Newtonsoft Serialize Settings
    /// already added Vector3Converter、QuaternionConverter and SpaceObjectConverter
    /// </summary>
    public static readonly JsonSerializerSettings NewtonJsonSettings;

    static Serialization() {
      NewtonJsonSettings = new JsonSerializerSettings();
      Vector3Converter = new Vector3JsonConverter();
      QuaternionConverter = new QuaternionJsonConverter();
      SpaceObjectConverter = new SpaceObjectJsonConverter();
      NewtonJsonSettings.Converters.Add(Vector3Converter);
      NewtonJsonSettings.Converters.Add(QuaternionConverter);
      NewtonJsonSettings.Converters.Add(SpaceObjectConverter);
    }
    /// <summary>
    /// 序列化
    /// </summary>
    /// <param name="Object"></param>
    /// <returns></returns>
    public static string Serialize(SpaceObject Object) => JsonConvert.SerializeObject(Object, NewtonJsonSettings);
    /// <summary>
    /// 反序列化
    /// </summary>
    /// <param name="Str"></param>
    /// <returns></returns>
    public static SpaceObject Deserialize(string Str) => JsonConvert.DeserializeObject<SpaceObject>(Str, NewtonJsonSettings);

  }

}
