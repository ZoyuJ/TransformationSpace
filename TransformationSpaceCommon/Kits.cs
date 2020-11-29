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

    /// <summary>
    /// 绕X轴旋转180°
    /// </summary>
    public static readonly Vector3 EulerPosX = new Vector3(180f, 0f, 0f);
    /// <summary>
    /// 绕X轴旋转-180°
    /// </summary>
    public static readonly Vector3 EulerNegX = new Vector3(-180f, 0f, 0f);
    /// <summary>
    /// 绕Y轴旋转180°
    /// </summary>
    public static readonly Vector3 EulerPosY = new Vector3(0f, 180f, 0f);
    /// <summary>
    /// 绕Y轴旋转-180°
    /// </summary>
    public static readonly Vector3 EulerNegY = new Vector3(0f, -180f, 0f);
    /// <summary>
    /// 绕Z轴旋转180°
    /// </summary>
    public static readonly Vector3 EulerPosZ = new Vector3(0f, 0f, 180f);
    /// <summary>
    /// 绕Z轴旋转-180°
    /// </summary>
    public static readonly Vector3 EulerNegZ = new Vector3(0f, 0f, -180f);
    /// <summary>
    /// 不旋转
    /// </summary>
    public static readonly Vector3 EulerIdentityZero = new Vector3(0f, 0f, 0f);
    /// <summary>
    /// 正方向旋转至不旋转
    /// </summary>
    public static readonly Vector3 EulerIdentityPos = new Vector3(180f, 180f, 180f);
    /// <summary>
    /// 负方向旋转至不旋转
    /// </summary>
    public static readonly Vector3 EulerIdentityNeg = new Vector3(-180f, -180f, -180f);

    /// <summary>
    /// 不旋转
    /// </summary>
    public static readonly Quaternion QuaternionIdentityNeg = new Quaternion(0f, 0f, 0f, -1f);
    /// <summary>
    /// 绕X轴旋转180°
    /// </summary>
    public static readonly Quaternion QuaternionPosX = new Quaternion(1f, 0f, 0f, 0f);
    /// <summary>
    /// 绕X轴旋转-180°
    /// </summary>
    public static readonly Quaternion QuaternionNegX = new Quaternion(-1f, 0f, 0f, 0f);
    /// <summary>
    /// 绕Y轴旋转180°
    /// </summary>
    public static readonly Quaternion QuaternionPosY = new Quaternion(0f, 1f, 0f, 0f);
    /// <summary>
    /// 绕Y轴旋转-180°
    /// </summary>
    public static readonly Quaternion QuaternionNegY = new Quaternion(0f, -1f, 0f, 0f);
    /// <summary>
    /// 绕Z轴旋转180°
    /// </summary>
    public static readonly Quaternion QuaternionPosZ = new Quaternion(0f, 0f, 1f, 0f);
    /// <summary>
    /// 绕Z轴旋转-180°
    /// </summary>
    public static readonly Quaternion QuaternionNegZ = new Quaternion(0f, 0f, -1f, 0f);


    public static bool TooClose(in this float Left, in float Right) => Math.Abs(Left - Right) <= Epsilon;
    public static bool ToClose(in this Vector3 Left, in Vector3 Right) => Math.Abs(Left.X - Right.X) <= Epsilon && Math.Abs(Left.Y - Right.Y) <= Epsilon && Math.Abs(Left.Z - Right.Z) <= Epsilon;
    public static bool ToClose(in this Quaternion Left, in Quaternion Right) => Math.Abs(Left.X - Right.X) <= Epsilon && Math.Abs(Left.Y - Right.Y) <= Epsilon && Math.Abs(Left.Z - Right.Z) <= Epsilon && Math.Abs(Left.W - Right.W) <= Epsilon;
    //public static bool ToClose(in this Matrix4x4 Left, in Matrix4x4 Right) => Math.Abs(Left - Right) <= Epsilon;

    /*
     Quat(0,0,0,1) = Euler(0,0,0)
     Quat(0,0,1,0) = Euler(0,0,180)
     Quat(0,0,-1,0) = Euler(0,0,-180)
     Quat(0,1,0,0) = Euler(0,180,0)
     Quat(0,-1,0,0) = Euler(0,-180,0)
     Quat(1,0,0,0) = Euler(180,0,0)
     Quat(-1,0,0,0) = Euler(-180,0,0)
     */
    /// <summary>
    /// Degree To Qua...
    /// </summary>
    /// <param name="Rotate"></param>
    /// <returns></returns>
    public static Quaternion FromEuler(in Vector3 Rotate) {
      if (Rotate.ToClose(EulerIdentityZero)) return Quaternion.Identity;
      else if (Rotate.ToClose(EulerIdentityPos)) return Quaternion.Identity;
      else if (Rotate.ToClose(EulerIdentityNeg)) return QuaternionIdentityNeg;
      else if (Rotate.ToClose(EulerPosX)) return QuaternionPosX;
      else if (Rotate.ToClose(EulerNegX)) return QuaternionNegX;
      else if (Rotate.ToClose(EulerPosY)) return QuaternionPosY;
      else if (Rotate.ToClose(EulerNegY)) return QuaternionNegY;
      else if (Rotate.ToClose(EulerPosZ)) return QuaternionPosZ;
      else if (Rotate.ToClose(EulerNegZ)) return QuaternionNegZ;
      return Quaternion.Normalize(Quaternion.CreateFromYawPitchRoll(Rotate.Y * Deg2Rad, Rotate.X * Deg2Rad, Rotate.Z * Deg2Rad));
    }
    /// <summary>
    /// Quaternion To Degree
    /// https://stackoverflow.com/questions/1031005/is-there-an-algorithm-for-converting-quaternion-rotations-to-euler-angle-rotatio/2070899#2070899
    /// </summary>
    /// <param name="This"></param>
    /// <returns></returns>
    public static Vector3 ToEuler(this Quaternion This) {
      if (This.X + This.Z == 0F) This = new Quaternion(0F, This.Y, 0F, This.W);
      else if (This.X + This.Y == 0F) This = new Quaternion(0F, 0F, This.Z, This.W);
      else if (This.Z + This.Y == 0F) This = new Quaternion(This.X, 0F, 0f, This.W);
      if (This.ToClose(Quaternion.Identity)) return EulerIdentityZero;
      else if (This.ToClose(Quaternion.Identity)) return EulerIdentityPos;
      else if (This.ToClose(QuaternionIdentityNeg)) return EulerIdentityNeg;
      else if (This.ToClose(QuaternionPosX)) return EulerPosX;
      else if (This.ToClose(QuaternionNegX)) return EulerNegX;
      else if (This.ToClose(QuaternionPosY)) return EulerPosY;
      else if (This.ToClose(QuaternionNegY)) return EulerNegY;
      else if (This.ToClose(QuaternionPosZ)) return EulerPosZ;
      else if (This.ToClose(QuaternionNegZ)) return EulerNegZ;
      This = Quaternion.Normalize(This);
      float LengthSqr = This.LengthSquared();
      var Eul = new Vector3(
             (float)(Math.Atan2(2.0f * (This.Y * This.Z + This.X * This.W), 1.0f - 2.0f * (This.X * This.X + This.Y * This.Y))) * Rad2Deg,
             (float)(Math.Asin(2.0f * (This.Y * This.W - This.X * This.Z) / LengthSqr)) * Rad2Deg,
            (float)(Math.Atan2(2.0f * (This.X * This.Y + This.Z * This.W), 1.0f - 2.0f * (This.Y * This.Y + This.Z * This.Z))) * Rad2Deg
               );
      return Eul.MinimumRotate();
    }

    private static Vector3 MinimumRotate(in this Vector3 This) {
      if (This.X.TooClose(180) && This.Z.TooClose(180)) return new Vector3(0, 180 - This.Y, 0);
      if (This.Y.TooClose(180) && This.Z.TooClose(180)) return new Vector3(180 - This.X, 0, 0);
      if (This.X.TooClose(180) && This.Y.TooClose(180)) return new Vector3(0, 0, 180 - This.Z);
      return This;
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



    /// <summary>
    /// Indexer4Matrix4x4
    /// 11 12 13 14     00 01 02 03
    /// 21 22 23 24     04 05 06 07
    /// 31 32 33 34 =>  08 09 10 11
    /// 41 42 43 44     12 13 14 15
    /// </summary>
    /// <param name="This"></param>
    /// <param name="PassIndex"></param>
    /// <returns></returns>
    public static float GetMatrix4x4Pass(in this Matrix4x4 This, in int PassIndex) {
      switch (PassIndex) {
        case 0: return This.M11;
        case 1: return This.M12;
        case 2: return This.M13;
        case 3: return This.M14;
        case 4: return This.M21;
        case 5: return This.M22;
        case 6: return This.M23;
        case 7: return This.M24;
        case 8: return This.M31;
        case 9: return This.M32;
        case 10: return This.M33;
        case 11: return This.M34;
        case 12: return This.M41;
        case 13: return This.M42;
        case 14: return This.M43;
        case 15: return This.M44;
        default:
          throw new IndexOutOfRangeException("00->15");
      }
    }
    //public static void SetMatrix4x4Pass(in this Matrix4x4 This, in int PassIndex, in float Value) {
    //  switch (PassIndex) {
    //    case 0: This.M11 = Value; return;
    //    case 1: This.M12 = Value; return;
    //    case 2: This.M13 = Value; return;
    //    case 3: This.M14 = Value; return;
    //    case 4: This.M21 = Value; return;
    //    case 5: This.M22 = Value; return;
    //    case 6: This.M23 = Value; return;
    //    case 7: This.M24 = Value; return;
    //    case 8: This.M31 = Value; return;
    //    case 9: This.M32 = Value; return;
    //    case 10: This.M33 = Value; return;
    //    case 11: This.M34 = Value; return;
    //    case 12: This.M41 = Value; return;
    //    case 13: This.M42 = Value; return;
    //    case 14: This.M43 = Value; return;
    //    case 15: This.M44 = Value; return;
    //    default:
    //      throw new IndexOutOfRangeException("00->15");
    //  }
    //}
    //public static Quaternion FromRotationMatrix2(Matrix4x4 Mat) {
    //  float t = 1.0f;
    //  if (Mat.GetMatrix4x4Pass(0) + Mat.GetMatrix4x4Pass(5) + Mat.GetMatrix4x4Pass(10) > 0.0f) {
    //    t += Mat.GetMatrix4x4Pass(0) + Mat.GetMatrix4x4Pass(5) + Mat.GetMatrix4x4Pass(10);
    //    return new Quaternion(Mat.GetMatrix4x4Pass(6) - Mat.GetMatrix4x4Pass(9), Mat.GetMatrix4x4Pass(8) - Mat.GetMatrix4x4Pass(2), Mat.GetMatrix4x4Pass(1) - Mat.GetMatrix4x4Pass(4), t) * 0.5F;
    //  }
    //  if (Mat.GetMatrix4x4Pass(0) > Mat.GetMatrix4x4Pass(5) && Mat.GetMatrix4x4Pass(0) > Mat.GetMatrix4x4Pass(10)) {
    //    t += Mat.GetMatrix4x4Pass(0) - Mat.GetMatrix4x4Pass(5) - Mat.GetMatrix4x4Pass(10);
    //    return new Quaternion(t, Mat.GetMatrix4x4Pass(1) + Mat.GetMatrix4x4Pass(4), Mat.GetMatrix4x4Pass(8) + Mat.GetMatrix4x4Pass(2), Mat.GetMatrix4x4Pass(6) - Mat.GetMatrix4x4Pass(9)) * 0.5F;
    //  }
    //  if (Mat.GetMatrix4x4Pass(5) > Mat.GetMatrix4x4Pass(10)) {
    //    t += -Mat.GetMatrix4x4Pass(0) + Mat.GetMatrix4x4Pass(5) - Mat.GetMatrix4x4Pass(10);
    //    return new Quaternion(Mat.GetMatrix4x4Pass(1) + Mat.GetMatrix4x4Pass(4), t, Mat.GetMatrix4x4Pass(6) + Mat.GetMatrix4x4Pass(9), Mat.GetMatrix4x4Pass(8) - Mat.GetMatrix4x4Pass(2)) * 0.5F;
    //  }
    //  t += -Mat.GetMatrix4x4Pass(0) - Mat.GetMatrix4x4Pass(5) + Mat.GetMatrix4x4Pass(10);
    //  return new Quaternion(Mat.GetMatrix4x4Pass(8) + Mat.GetMatrix4x4Pass(2), Mat.GetMatrix4x4Pass(6) + Mat.GetMatrix4x4Pass(9), t, Mat.GetMatrix4x4Pass(1) - Mat.GetMatrix4x4Pass(4)) * 0.5F;
    //}

  }



}
