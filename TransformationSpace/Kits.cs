namespace TransformationSpace {
  using System;
  using System.Collections.Generic;
  using System.Numerics;

  public static class Kits {
    public const float Deg2Rad = (float)(Math.PI / 180.0);
    public const float Rad2Deg = (float)(180.0 / Math.PI);
    public const float Epsilon = 0.0001f;

    /// <summary>
    /// 比较并赋值
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="Value"></param>
    /// <param name="Target"></param>
    /// <returns>是否相同，true:不同并赋值，false:相同</returns>
    public static bool CompareAndSet<T>(T Value, ref T Target) {
      if (!EqualityComparer<T>.Default.Equals(Value, Target)) {
        Target = Value;
        return true;
      }
      return false;
    }

    public static bool ToClose(in this float Left, in float Right) => Math.Abs(Left - Right) <= Epsilon;
    public static bool ToClose(in this Vector3 Left, in Vector3 Right) => Math.Abs(Left.X - Right.X) <= Epsilon && Math.Abs(Left.Y - Right.Y) <= Epsilon && Math.Abs(Left.Z - Right.Z) <= Epsilon;
    public static bool ToClose(in this Quaternion Left, in Quaternion Right) => Math.Abs(Left.X - Right.X) <= Epsilon && Math.Abs(Left.Y - Right.Y) <= Epsilon && Math.Abs(Left.Z - Right.Z) <= Epsilon && Math.Abs(Left.W - Right.W) <= Epsilon;
    //public static bool ToClose(in this Matrix4x4 Left, in Matrix4x4 Right) => Math.Abs(Left - Right) <= Epsilon;

    /// <summary>
    /// Degree To Qua...
    /// </summary>
    /// <param name="Rotate"></param>
    /// <returns></returns>
    public static Quaternion FromEuler(in Vector3 Rotate) => Quaternion.CreateFromYawPitchRoll(Rotate.X * Deg2Rad, Rotate.Y * Deg2Rad, Rotate.Z * Deg2Rad);
    /// <summary>
    /// Quaternion To Degree
    /// https://stackoverflow.com/questions/1031005/is-there-an-algorithm-for-converting-quaternion-rotations-to-euler-angle-rotatio/2070899#2070899
    /// </summary>
    /// <param name="This"></param>
    /// <returns></returns>
    public static Vector3 ToEuler(this Quaternion This) {
      float LengthSqr = This.LengthSquared();
      return new Vector3(
                (float)(Math.Atan2(2.0f * (This.Y * This.Z + This.X * This.W), 1.0f - 2.0f * (This.X * This.X + This.Y * This.Y))) * Rad2Deg,
                (float)(Math.Asin(2.0f * (This.Y * This.W - This.X * This.Z) / LengthSqr)) * Rad2Deg,
                (float)(Math.Atan2(2.0f * (This.X * This.Y + This.Z * This.W), 1.0f - 2.0f * (This.Y * This.Y + This.Z * This.Z))) * Rad2Deg
              );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="Translate"></param>
    /// <param name="Rotate"></param>
    /// <param name="Scale"></param>
    /// <returns></returns>
    public static Matrix4x4 FromTRS(in Vector3 Translate, in Vector3 Rotate, in Vector3 Scale) {
      return FromTRS(Translate, Kits.FromEuler(Rotate), Scale);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="Translate"></param>
    /// <param name="Rotate"></param>
    /// <param name="Scale"></param>
    /// <returns></returns>
    public static Matrix4x4 FromTRS(in Vector3 Translate, in Quaternion Rotate, in Vector3 Scale) {
      return Matrix4x4.CreateTranslation(Translate) * Matrix4x4.CreateFromQuaternion(Rotate) * Matrix4x4.CreateScale(Scale);
    }

    public static Matrix4x4 LookAtMatrix(in Vector3 Target, in Vector3 Position, in Vector3 Up) {
      var forward = Vector3.Normalize(Position - Target);
      var side = Vector3.Normalize(Vector3.Cross(Vector3.Normalize(Up), forward));
      var up = Vector3.Normalize(Vector3.Cross(forward, side));
      return new Matrix4x4(
        side.X, up.X, forward.X, 0.0f,
        side.Y, up.Y, forward.Y, 0.0f,
        side.Z, up.Z, forward.Z, 0.0f,
        -Vector3.Dot(side, Position),
        -Vector3.Dot(up, Position),
        -Vector3.Dot(forward, Position), 1.0f);
    }

  }

  /// <summary>
  /// data with dirty flag
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public struct DataStorage<T> {
    private T _Data;
    /// <summary>
    /// target data
    /// </summary>
    public T Data {
      get => _Data;
      set {
        if (Kits.CompareAndSet(value, ref _Data)) {
          Dirty = true;
        }
      }
    }
    /// <summary>
    /// data changed flag
    /// </summary>
    public bool Dirty { get; set; }
    /// <summary>
    /// set data and dont set flag
    /// </summary>
    /// <param name="Data"></param>
    public void SetData(T Data) => _Data = Data;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="Data">default data</param>
    /// <param name="SetChanged">defaut flag</param>
    public DataStorage(in T Data, in bool SetChanged = false) {
      _Data = Data;
      Dirty = SetChanged;
    }
  }

}
