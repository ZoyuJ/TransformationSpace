namespace TransformationSpace {
  using System;
  using System.Collections.Generic;
  using System.Numerics;

  public static class Kits {
    public const float Deg2Rad = (float)(Math.PI / 180.0);
    public const float Rad2Deg = (float)(180.0 / Math.PI);
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

    /// <summary>
    /// Degree To Qua...
    /// </summary>
    /// <param name="Rotate"></param>
    /// <returns></returns>
    public static Quaternion FromEuler(in Vector3 Rotate) => Quaternion.CreateFromYawPitchRoll((Rotate.X * Deg2Rad) % 360.0f, (Rotate.Y * Deg2Rad) % 360.0f, (Rotate.Z * Deg2Rad) % 360.0f);
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



  }
}
