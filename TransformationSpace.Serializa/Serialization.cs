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
  /// <summary>
  /// Vector3 <-De/Serialize-> [X,Y,Z]
  /// </summary>
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
  /// <summary>
  /// Quaternion <-De/Serialize-> [X,Y,Z,W]
  /// </summary>
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
  /// <summary>
  /// Quaternion <-Convert-> Euler(Vector3) <-De/Serialize-> [X,Y,Z]
  /// </summary>
  public class EulerQuaternionJsonConverter : JsonConverter {
    public override bool CanConvert(Type objectType) {
      return (objectType == typeof(Quaternion));
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
      var Q = ((Quaternion)value).ToEuler();
      serializer.Serialize(writer, new float[] { Q.X, Q.Y, Q.Z });
    }
    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
      var V = serializer.Deserialize<float[]>(reader);
      return Kits.FromEuler(new Vector3(V[0], V[1], V[2]));
    }
  }
  /// <summary>
  /// SpaceObject <-De/Serialize-> {}
  /// </summary>
  public class SpaceObjectJsonConverter : JsonConverter {
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
      var _value = (SpaceObject)(value);
      writer.WriteStartObject();
      writer.WritePropertyName(nameof(_value.Name));
      writer.WriteValue(_value.Name);
      writer.WritePropertyName(nameof(_value.Order));
      writer.WriteValue(_value.Order);
      writer.WritePropertyName(nameof(_value.LocalPosition));
      serializer.Serialize(writer, _value.LocalPosition);
      writer.WritePropertyName(nameof(_value.LocalRotation));
      serializer.Serialize(writer, _value.LocalRotation);
      writer.WritePropertyName(nameof(_value.LocalScale));
      serializer.Serialize(writer, _value.LocalScale);
      writer.WritePropertyName(nameof(_value.Parent));
      writer.WriteNull();
      writer.WritePropertyName(nameof(_value.Children));
      serializer.Serialize(writer, _value.Children);
      writer.WriteEndObject();
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
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

    public override bool CanConvert(Type objectType) {
      return objectType == typeof(SpaceObject);
    }
  }
  //public class SpaceObjectJsonConverter : JsonConverter<SpaceObject> {
  //  public override void WriteJson(JsonWriter writer, [AllowNull] SpaceObject value, JsonSerializer serializer) {
  //    writer.WriteStartObject();
  //    writer.WritePropertyName(nameof(value.Name));
  //    writer.WriteValue(value.Name);
  //    writer.WritePropertyName(nameof(value.Order));
  //    writer.WriteValue(value.Order);
  //    writer.WritePropertyName(nameof(value.LocalPosition));
  //    serializer.Serialize(writer, value.LocalPosition);
  //    writer.WritePropertyName(nameof(value.LocalRotation));
  //    serializer.Serialize(writer, value.LocalRotation);
  //    writer.WritePropertyName(nameof(value.LocalScale));
  //    serializer.Serialize(writer, value.LocalScale);
  //    writer.WritePropertyName(nameof(value.Parent));
  //    writer.WriteNull();
  //    writer.WritePropertyName(nameof(value.Children));
  //    serializer.Serialize(writer, value.Children);
  //    writer.WriteEndObject();
  //  }
  //  public override SpaceObject ReadJson(JsonReader reader, Type objectType, [AllowNull] SpaceObject existingValue, bool hasExistingValue, JsonSerializer serializer) {
  //    var Target = new SpaceObject();
  //    do {
  //      reader.Read();
  //      if (reader.TokenType == JsonToken.PropertyName) {
  //        switch (reader.Value) {
  //          case nameof(SpaceObject.Name):
  //            Target.Name = reader.ReadAsString();
  //            break;
  //          case nameof(SpaceObject.Order):
  //            Target.Order = reader.ReadAsInt32().Value;
  //            break;
  //          case nameof(SpaceObject.LocalPosition):
  //            reader.Read();
  //            Target.LocalPosition = serializer.Deserialize<Vector3>(reader);
  //            break;
  //          case nameof(SpaceObject.LocalRotation):
  //            reader.Read();
  //            Target.LocalRotation = serializer.Deserialize<Quaternion>(reader);
  //            break;
  //          case nameof(SpaceObject.LocalScale):
  //            reader.Read();
  //            Target.LocalScale = serializer.Deserialize<Vector3>(reader);
  //            break;
  //          case nameof(SpaceObject.Children):
  //            reader.Read();
  //            var P = serializer.Deserialize<SpaceObject[]>(reader);
  //            for (int i = 0; i < P.Length; i++) {
  //              Target.Children.Add(P[i]);
  //            }
  //            break;
  //          default:
  //            break;
  //        }
  //      }
  //      else if (reader.TokenType == JsonToken.EndObject) {
  //        break;
  //      }
  //    } while (true);
  //    return Target;
  //  }
  //}

  /// <summary>
  /// 
  /// </summary>
  public class DetectingQuaterionJsonConverter : JsonConverter {
    public override bool CanConvert(Type objectType) {
      return (objectType == typeof(Quaternion));
    }

    public DetectingQuaterionJsonConverter(bool SerializeUseEuler = true) {
      this.SerializeUseEuler = SerializeUseEuler;
    }

    /// <summary>
    /// true: Use Euler formatter for Serialization
    /// </summary>
    public bool SerializeUseEuler;

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
      if (SerializeUseEuler) {
        var Q = ((Quaternion)value).ToEuler();
        serializer.Serialize(writer, new float[] { Q.X, Q.Y, Q.Z });
      }
      else {
        var Q = (Quaternion)value;
        serializer.Serialize(writer, new float[] { Q.X, Q.Y, Q.Z, Q.W });
      }
    }
    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
      var V = serializer.Deserialize<float[]>(reader);
      return V.Length == 3 ? Kits.FromEuler(new Vector3(V[0], V[1], V[2])) : new Quaternion(V[0], V[1], V[2], V[3]);
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
    /// Quaternion <-Convert-> Euler(Vector3) <-De/Serialize-> [X,Y,Z]
    /// </summary>
    public static readonly EulerQuaternionJsonConverter EulerQuaternionConverter;
    /// <summary>
    /// Quaternion (<-Convert-> Euler(Vector3)) <-De/Serialize-> [X,Y,Z]/[X,Y,Z,W]
    /// </summary>
    public static readonly DetectingQuaterionJsonConverter DetectingQuaterionConverter;
    /// <summary>
    /// SpaceObject <-De/Serialize-> {}
    /// </summary>
    public static readonly SpaceObjectJsonConverter SpaceObjectConverter;

    static Serialization() {
      Vector3Converter = new Vector3JsonConverter();
      QuaternionConverter = new QuaternionJsonConverter();
      EulerQuaternionConverter = new EulerQuaternionJsonConverter();
      DetectingQuaterionConverter = new DetectingQuaterionJsonConverter(true);
      SpaceObjectConverter = new SpaceObjectJsonConverter();
    }
    /// <summary>
    /// 序列化
    /// rotation value is [X,Y,Z,W]
    /// </summary>
    /// <param name="Object"></param>
    /// <returns></returns>
    public static string Serialize(SpaceObject Object) => JsonConvert.SerializeObject(Object, new JsonSerializerSettings() { Converters = new JsonConverter[] { Vector3Converter, QuaternionConverter, SpaceObjectConverter } });
    /// <summary>
    /// 序列化
    /// rotation value is Euler[X,Y,Z]
    /// </summary>
    /// <param name="Object"></param>
    /// <returns></returns>
    public static string SerializeWithEuler(SpaceObject Object) => JsonConvert.SerializeObject(Object, new JsonSerializerSettings() { Converters = new JsonConverter[] { Vector3Converter, EulerQuaternionConverter, SpaceObjectConverter } });
    /// <summary>
    /// 反序列化
    /// rotation value is [X,Y,Z,W]
    /// </summary>
    /// <param name="Str"></param>
    /// <returns></returns>
    public static SpaceObject Deserialize(string Str) => JsonConvert.DeserializeObject<SpaceObject>(Str, new JsonSerializerSettings() { Converters = new JsonConverter[] { Vector3Converter, QuaternionConverter, SpaceObjectConverter } });
    /// <summary>
    /// 反序列化
    /// rotation value is Euler[X,Y,Z]
    /// </summary>
    /// <param name="Str"></param>
    /// <returns></returns>
    public static SpaceObject DeserializeWithEuler(string Str) => JsonConvert.DeserializeObject<SpaceObject>(Str, new JsonSerializerSettings() { Converters = new JsonConverter[] { Vector3Converter, EulerQuaternionConverter, SpaceObjectConverter } });
    /// <summary>
    /// 序列化
    /// </summary>
    /// <param name="Object"></param>
    /// <param name="UseEulerFormatte">true:use Euler formatte</param>
    /// <returns></returns>
    public static string SerializeDetected(SpaceObject Object, in bool UseEulerFormatte = true) {
      return JsonConvert.SerializeObject(Object, new JsonSerializerSettings() { Converters = new JsonConverter[] { Vector3Converter, new DetectingQuaterionJsonConverter(UseEulerFormatte), SpaceObjectConverter } });
    }
    /// <summary>
    /// 反序列化
    /// Whatever rotation value formatte is Euler or Quaternion
    /// </summary>
    /// <param name="Str"></param>
    /// <returns></returns>
    public static string DeserializeDetected(SpaceObject Object) => JsonConvert.SerializeObject(Object, new JsonSerializerSettings() { Converters = new JsonConverter[] { Vector3Converter, DetectingQuaterionConverter, SpaceObjectConverter } });

  }

}
