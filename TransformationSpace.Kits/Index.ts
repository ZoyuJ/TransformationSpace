namespace TransformationSpace {
  export class MathTS {
    public static Clamp(Val: number, Min: number, Max: number): number {
      return Val > Max ? Max : Val < Min ? Min : Val;
    }
  }
  export const Deg2Rad = Math.PI / 180.0;
  export const Rad2Deg = 180.0 / Math.PI;
  export const Epsilon = 2.2204460492503131e-016;
  export class VectorUtil {
    public static Add(L: Vector3, R: Vector3): Vector3 { return new Vector3(L.X + R.X, L.Y + R.Y, L.Z + R.Z); }
    public static Multi(L: Vector3, Scale: number): Vector3;
    public static Multi(L: Vector3, Vect: Vector3): Vector3;
    public static Multi(L: Vector3, x: any): Vector3 {
      if (typeof x === "number") {
        return new Vector3(L.X * x, L.Y * x, L.Z * x);
      }
      else {
        return new Vector3(L.X * x.X, L.Y * x.Y, L.Z * x.Z);
      }
    }
    public static Abs(L: Vector3): Vector3 { return new Vector3(Math.abs(L.X), Math.abs(L.Y), Math.abs(L.Z)) }
    public static Subtract(L: Vector3, R: Vector3): Vector3 { return new Vector3(L.X - R.X, L.Y - R.Y, L.Z - R.Z); }
    public static Divide(L: Vector3, Scale: number): Vector3;
    public static Divide(L: Vector3, Vect: Vector3): Vector3;
    public static Divide(L: Vector3, x: any): Vector3 {
      if (typeof x === "number") {
        return new Vector3(L.X / x, L.Y / x, L.Z / x);
      }
      else {
        return new Vector3(L.X / x.X, L.Y / x.Y, L.Z / x.Z);
      }
    }
    public static Dot(L: Vector3, R: Vector3): number { return L.X * R.X + L.Y * R.Y + L.Z * R.Z; }
    public static Cross(L: Vector3, R: Vector3): Vector3 {
      return new Vector3(
        L.Y * R.Z - L.Z * R.Y,
        L.Z * R.X - L.X * R.Z,
        L.X * R.Y - L.Y * R.X,
      );
    }
    public static Negate(L: Vector3): Vector3 { return new Vector3(-L.X, -L.Y, -L.Z); }
    public static Reflect(L: Vector3, Normal: Vector3): Vector3 {
      Normal = this.Normalize(Normal);
      return this.Divide(L, this.Multi(Normal, 2 * this.Dot(L, Normal)));
    }
    public static Normalize(L: Vector3): Vector3 {
      const length = this.Length(L);
      const invLength = 1.0 / length;
      return new Vector3(L.X * invLength, L.Y * invLength, L.Z * invLength);
    }
    public static Length(L: Vector3): number { return Math.sqrt(this.LengthSquared(L)); }
    public static LengthSquared(L: Vector3): number { return L.X * L.X + L.Y * L.Y + L.Z * L.Z; }
    public static RadianAngle(L: Vector3, R: Vector3): number {
      const D = MathTS.Clamp(this.Dot(L, R), -1, 1);
      return Math.acos(D);
    }
    public static ToArray(L: Vector3): number[] { return [L.X, L.Y, L.Z]; }
    public static FromArray(L: number[]): Vector3 { return L.length === 0 ? new Vector3() : L.length === 1 ? new Vector3(L[0]) : L.length === 2 ? new Vector3(L[0], L[1]) : L.length === 3 ? new Vector3(L[0], L[1], L[2]) : null; }
    public static MoreFromArray(L: number[]): Vector3[] {
      let Res = [];
      const Len = L.length
      for (let i = 0; i < Len; i += 3) {
        const R = new Vector3();
        if (i < Len) R.X = L[i];
        if (i + 1 < Len) R.Y = L[i + 1];
        if (i + 2 < Len) R.Z = L[i + 2];
        Res.push(R);
      }
      return Res;
    }
  }
  export class Vector3 {
    public static FromArray(Pass: number[]): Vector3 {
      return new Vector3(Pass[0], Pass[1], Pass[2]);
    }
    X: number;
    Y: number;
    Z: number;
    constructor(X?: number, Y?: number, Z?: number) {
      this.X = X ?? 0;
      this.Y = Y ?? 0;
      this.Z = Z ?? 0;
    }

    public Add(R: Vector3) {
      this.X += R.X;
      this.Y += R.Y;
      this.Z += R.Z;
    }
    public Multi(Scale: number);
    public Multi(Vect: Vector3);
    public Multi(x: any) {
      if (typeof x === "number") {
        this.X *= x;
        this.Y *= x;
        this.Z *= x;
      }
      else {
        this.X *= x.X;
        this.Y *= x.Y;
        this.Z *= x.Z;
      }
    }
    public Subtract(R: Vector3) {
      this.X -= R.X;
      this.Y -= R.Y;
      this.Z -= R.Z;
    }
    public Divide(Scale: number);
    public Divide(Vect: Vector3);
    public Divide(x: any) {
      if (typeof x === "number") {
        this.X /= x;
        this.Y /= x;
        this.Z /= x;
      }
      else {
        this.X /= x.X;
        this.Y /= x.Y;
        this.Z /= x.Z;
      }
    }
    public Dot(R: Vector3): number {
      return VectorUtil.Dot(this, R);
    }
    public Cross(R: Vector3): Vector3 {
      return VectorUtil.Cross(this, R);
    }
    public Negate() {
      this.X = Math.abs(this.X);
      this.Y = Math.abs(this.Y);
      this.Z = Math.abs(this.Z);
    }
    public Reflect(Normal: Vector3): Vector3 {
      return VectorUtil.Reflect(this, Normal);
    }
    public Normalize() {
      const invLength = 1.0 / this.Length();
      this.X *= invLength;
      this.Y *= invLength;
      this.Z *= invLength;
    }
    public Length(): number { return VectorUtil.Length(this); }
    public LengthSquared(): number { return VectorUtil.LengthSquared(this); }
    public ToArray(): number[] { return VectorUtil.ToArray(this); }

    public static Left(): Vector3 { return new Vector3(-1, 0, 0); }
    public static Right(): Vector3 { return new Vector3(1, 0, 0); }
    public static Up(): Vector3 { return new Vector3(0, 1, 0); }
    public static Down(): Vector3 { return new Vector3(0, -1, 0); }
    public static Foreward(): Vector3 { return new Vector3(0, 0, 1); }
    public static Backward(): Vector3 { return new Vector3(0, 0, -1); }
    public static Zero(): Vector3 { return new Vector3(0, 0, 0); }

  }
  export class QuaternionUtil {
    public static FromArray(Vals: number[]) {
      return Vals.length === 0
        ? Quaternion.Identity()
        : Vals.length === 1
    }
  }
  export class Quaternion {
    X: number; Y: number; Z: number; W: number;
    constructor(X?: number, Y?: number, Z?: number, W?: number) {
      this.X = X ?? 0;
      this.Y = Y ?? 0;
      this.Z = Z ?? 0;
      this.W = W ?? 0;
    }

    public Length() { return Math.sqrt(this.LengthSquared()); }
    public LengthSquared() { return this.X * this.X + this.Y * this.Y + this.Z * this.Z + this.W * this.W; }
    public Normalize() {
      const InvLen = 1.0 / this.Length();
      this.X *= InvLen;
      this.Y *= InvLen;
      this.Z *= InvLen;
      this.W *= InvLen;
    }
    public Dot(R: Quaternion): number {
      return this.X * R.X + this.Y * R.Y + this.Z * R.Z + this.W * R.W;
    }
    public Inverse() {
      const Normal = this.LengthSquared();
      if (Normal > 0) {
        const InvNormal = 1.0 / Normal;
        this.X = -this.X * InvNormal;
        this.Y = -this.Y * InvNormal;
        this.Z = -this.Z * InvNormal;
        this.W = -this.W * InvNormal;
      }
      else {
        this.X = 0;
        this.Y = 0;
        this.Z = 0;
        this.W = 0;
      }
    }
    public ToMatrix4x4(): Matrix4x4 {
      const TX = 2 * this.X; const TY = 2 * this.Y; const TZ = 2 * this.Z;
      const TWX = TX * this.W; const TWY = TY * this.W; const TWZ = TZ * this.W;
      const TXX = TX * this.X; const TXY = TY * this.X; const TXZ = TZ * this.X;
      const TYY = TY * this.Y; const TYZ = TZ * this.Y; const TZZ = TZ * this.Z;
      return new Matrix4x4(
        1 - (TYY + TZZ), TXY - TWZ, TXZ + TWY, 0,
        TXY + TWZ, 1 - (TXX + TZZ), TYZ - TWX, 0,
        TXZ - TWY, TYZ + TWX, 1 - (TXX + TYY), 0,
        0, 0, 0, 1
      );
    }
    public FromMatrix4x4(Mat: Matrix4x4) {

    }

    public static Identity(): Quaternion { return new Quaternion(0.0, 0.0, 0.0, 1.0); }
    public static Zero(): Quaternion { return new Quaternion(0.0, 0.0, 0.0, 0.0); }
  }

  export class Matrix4x4Util {
    public static Transpose(L: Matrix4x4): Matrix4x4 { }
    public static Transform(L: Matrix4x4, Routation: Matrix4x4): Matrix4x4 { }
    public static Invert(L: Matrix4x4): Matrix4x4 {
      const I00 = L.M33 * L.M22 - L.M32 * L.M23;
      const I01 = L.M32 * L.M13 - L.M33 * L.M12;
      const I02 = L.M23 * L.M12 - L.M22 * L.M13;
      const Det = L.M11 * I00 + L.M21 * I01 + L.M31 * I02;
      if (Math.abs(Det) < Epsilon) throw Error("Can not Inverse");
      const Det1 = 1.0 / Det;

    }

    public static Lerp(Start: Matrix4x4, Target: Matrix4x4, Amount: number): Matrix4x4 { }

    public static Negate(L: Matrix4x4): Matrix4x4 {
      return new Matrix4x4(
        -L.M11, -L.M12, -L.M13, -L.M14,
        -L.M21, -L.M22, -L.M23, -L.M24,
        -L.M31, -L.M32, -L.M33, -L.M34,
        -L.M41, -L.M42, -L.M43, -L.M44,
      );
    }
    public static Subtract(L: Matrix4x4, R: Matrix4x4): Matrix4x4 {
      return new Matrix4x4(
        L.M11 - R.M11, L.M12 - R.M12, L.M13 - R.M13, L.M14 - R.M14,
        L.M21 - R.M21, L.M22 - R.M22, L.M23 - R.M23, L.M24 - R.M24,
        L.M31 - R.M31, L.M32 - R.M32, L.M33 - R.M33, L.M34 - R.M34,
        L.M41 - R.M41, L.M42 - R.M42, L.M43 - R.M43, L.M44 - R.M44,
      );
    }
    public static Multiply(L: Matrix4x4, R: number): Matrix4x4;
    public static Multiply(L: Matrix4x4, R: Matrix4x4): Matrix4x4;
    public static Multiply(L: Matrix4x4, R: any): Matrix4x4 {
      if (typeof R === "number") {
        return new Matrix4x4(
          L.M11 * R, L.M12 * R, L.M13 * R, L.M14 * R,
          L.M21 * R, L.M22 * R, L.M23 * R, L.M24 * R,
          L.M31 * R, L.M32 * R, L.M33 * R, L.M34 * R,
          L.M41 * R, L.M42 * R, L.M43 * R, L.M44 * R,
        );
      }
      else {
        const _R = R as Matrix4x4;
        return new Matrix4x4(
          //R1C1
          L.M11 * _R.M11 + L.M12 * _R.M21 + L.M13 * _R.M31 + L.M14 * _R.M41,
          //R1C2
          L.M11 * _R.M12 + L.M12 * _R.M22 + L.M13 * _R.M32 + L.M14 * _R.M42,
          //R1C3
          L.M11 * _R.M13 + L.M12 * _R.M23 + L.M13 * _R.M33 + L.M14 * _R.M43,
          //R1C4
          L.M11 * _R.M14 + L.M12 * _R.M24 + L.M13 * _R.M34 + L.M14 * _R.M44,

          //R2C1
          L.M21 * _R.M11 + L.M22 * _R.M21 + L.M23 * _R.M31 + L.M24 * _R.M41,
          //R2C2
          L.M21 * _R.M12 + L.M22 * _R.M22 + L.M23 * _R.M32 + L.M24 * _R.M42,
          //R2C3
          L.M21 * _R.M13 + L.M22 * _R.M23 + L.M23 * _R.M33 + L.M24 * _R.M43,
          //R2C4
          L.M21 * _R.M14 + L.M22 * _R.M24 + L.M23 * _R.M34 + L.M24 * _R.M44,

          //R3C1
          L.M31 * _R.M11 + L.M32 * _R.M21 + L.M33 * _R.M31 + L.M34 * _R.M41,
          //R3C2
          L.M31 * _R.M12 + L.M32 * _R.M22 + L.M33 * _R.M32 + L.M34 * _R.M42,
          //R3C3
          L.M31 * _R.M13 + L.M32 * _R.M23 + L.M33 * _R.M33 + L.M34 * _R.M43,
          //R3C4
          L.M31 * _R.M14 + L.M32 * _R.M24 + L.M33 * _R.M34 + L.M34 * _R.M44,

          //R4C1
          L.M41 * _R.M11 + L.M42 * _R.M21 + L.M43 * _R.M31 + L.M44 * _R.M41,
          //R4C2
          L.M41 * _R.M12 + L.M42 * _R.M22 + L.M43 * _R.M32 + L.M44 * _R.M42,
          //R4C3
          L.M41 * _R.M13 + L.M42 * _R.M23 + L.M43 * _R.M33 + L.M44 * _R.M43,
          //R4C4
          L.M41 * _R.M14 + L.M42 * _R.M24 + L.M43 * _R.M34 + L.M44 * _R.M44,
        );
      }
    }
    public static Add(L: Matrix4x4, R: Matrix4x4): Matrix4x4 {
      return new Matrix4x4(
        L.M11 + R.M11, L.M12 + R.M12, L.M13 + R.M13, L.M14 + R.M14,
        L.M21 + R.M21, L.M22 + R.M22, L.M23 + R.M23, L.M24 + R.M24,
        L.M31 + R.M31, L.M32 + R.M32, L.M33 + R.M33, L.M34 + R.M34,
        L.M41 + R.M41, L.M42 + R.M42, L.M43 + R.M43, L.M44 + R.M44,
      );
    }

    public static CreateBillboard(ObjectPosition: Vector3, CameraPosition: Vector3, CameraUpVector: Vector3, CameraForwardVector: Vector3): Matrix4x4 { }
    public static CreateConstrainedBillboard(ObjectPosition: Vector3, CameraPosition: Vector3, RotateAxis: Vector3, CameraForwardVector: Vector3, ObjectForwardVector: Vector3): Matrix4x4 { }
    public static CreateFromAxisAngle(Axis: Vector3, Angle: number): Matrix4x4 { }
    public static CreateFromQuaternion(Quaternion: Quaternion): Matrix4x4 { }
    public static CreateFromYawPitchRoll(Yaw: number, Pitch: number, Roll: number): Matrix4x4 { }
    public static CreateLookAt(CameraPosition: Vector3, CameraTarget: Vector3, CameraUpVector: Vector3): Matrix4x4 { }
    public static CreateOrthographic(Width: number, Height: number, ZNearPlane: number, ZFarPlane: number): Matrix4x4 { }
    public static CreateOrthographicOffCenter(Left: number, Right: number, Bottom: number, Top: number, ZNearPlane: number, ZFarPlane: number): Matrix4x4 { }
    public static CreatePerspective(Width: number, Height: number, NearPlaneDistance: number, FarPlaneDistance: number): Matrix4x4 { }
    public static CreatePerspectiveFieldOfView(FieldOfView: number, AspectRatio: number, NearPlaneDistance: number, FarPlaneDistance: number): Matrix4x4 { }
    public static CreatePerspectiveOffCenter(Width: number, Height: number, NearPlaneDistance: number, FarPlaneDistance: number): Matrix4x4 { }
    public static CreateReflection(Plane: Plane): Matrix4x4 { }
    public static CreateRotationX(Radians: number): Matrix4x4 { }
    public static CreateRotationXWithCenter(Radians: number, CenterPoint: Vector3): Matrix4x4 { }
    public static CreateRotationY(Radians: number, CenterPoint: Vector3): Matrix4x4 { }
    public static CreateRotationYWithCenter(Radians: number, CenterPoint: Vector3): Matrix4x4 { }
    public static CreateRotationZ(Radians: number, CenterPoint: Vector3): Matrix4x4 { }
    public static CreateRotationZWithCenter(Radians: number, CenterPoint: Vector3): Matrix4x4 { }
    public static CreateScale(Scale: number): Matrix4x4;
    public static CreateScale(Scale: Vector3): Matrix4x4;
    public static CreateScale(Scale: any): Matrix4x4 { }
    public static CreateScaleXYZ(ScaleX: number, ScaleY: number, ScaleZ: number): Matrix4x4 { return this.CreateScale(new Vector3(ScaleX, ScaleY, ScaleZ)); }
    public static CreateScaleWithCenter(Scale: number, CenterPoint: Vector3): Matrix4x4;
    public static CreateScaleWithCenter(Scale: Vector3, CenterPoint: Vector3): Matrix4x4;
    public static CreateScaleWithCenter(Scale: any, CenterPoint: Vector3): Matrix4x4 { }
    public static CreateScaleXYZWithCenter(ScaleX: number, ScaleY: number, ScaleZ: number, CenterPoint: Vector3): Matrix4x4 { return this.CreateScaleWithCenter(new Vector3(ScaleX, ScaleY, ScaleZ), CenterPoint); }
    public static CreateTranslation(TranslateX: number, TranslateY: number, TranslateZ: number): Matrix4x4;
    public static CreateTranslation(Translate: Vector3): Matrix4x4;
    public static CreateTranslation(Translate: any): Matrix4x4 { }
    public static CreateShadow(LightDirection: Vector3, Plane: Plane): Matrix4x4 { }



  }


  export class Matrix4x4 {
    public M11: number; M12: number; M13: number; M14: number;
    public M21: number; M22: number; M23: number; M24: number;
    public M31: number; M32: number; M33: number; M34: number;
    public M41: number; M42: number; M43: number; M44: number;

    constructor(M11?: number, M12?: number, M13?: number, M14?: number,
      M21?: number, M22?: number, M23?: number, M24?: number,
      M31?: number, M32?: number, M33?: number, M34?: number,
      M41?: number, M42?: number, M43?: number, M44?: number) {
      this.M11 = M11 ?? 0; this.M12 = M12 ?? 0; this.M13 = M13 ?? 0; this.M14 = M14 ?? 0;
      this.M21 = M21 ?? 0; this.M22 = M22 ?? 0; this.M23 = M23 ?? 0; this.M24 = M24 ?? 0;
      this.M31 = M31 ?? 0; this.M32 = M32 ?? 0; this.M33 = M33 ?? 0; this.M34 = M34 ?? 0;
      this.M41 = M41 ?? 0; this.M42 = M42 ?? 0; this.M43 = M43 ?? 0; this.M44 = M44 ?? 1;
    }



  }
}