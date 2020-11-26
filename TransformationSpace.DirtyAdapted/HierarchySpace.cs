namespace TransformationSpace.DirtyAdapted {
  using System;
  using System.Collections;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.Diagnostics.CodeAnalysis;
  using System.Numerics;

  /// <summary>
  /// TramsformationSpace节点
  /// </summary>
  public class SpaceObject : ITransformHieraryEntity<SpaceObject>, IEnumerable<SpaceObject> {
    protected static readonly Matrix4x4 WorldBase;
    static SpaceObject() {
      WorldBase = Matrix4x4.CreateWorld(Vector3.Zero, Vector3.UnitZ, Vector3.UnitY); //Matrix4x4.Identity;/* Matrix4x4.CreateWorld(Vector3.Zero, Vector3.UnitZ, Vector3.UnitX);*/

    }
    /// <summary>
    /// default
    /// </summary>
    public SpaceObject() {
      _LocalScale.Data = Vector3.One;
      _LocalRotation.Data = Quaternion.Identity;
      _LocalPosition.Data = Vector3.Zero;
      Children = new ObservableCollection<SpaceObject>();
    }
    protected DataStorage<SpaceObject> _Parent;
    /// <summary>
    /// 父级
    /// </summary>
    public SpaceObject Parent {
      get {
        if (_Parent.Dirty) {
          _Parent.Dirty = false;
        }
        return _Parent.Data;
      }
      set {
        _Parent.Data = value;
        _LocalPosition.Dirty = true;
        _Position.Dirty = true;
        _LocalRotation.Dirty = true;
        _Rotation.Dirty = true;
        _LocalScale.Dirty = true;
      }
    }
    /// <summary>
    /// 子级
    /// </summary>
    public ObservableCollection<SpaceObject> Children { get; protected set; }
    /// <summary>
    /// need this?
    /// </summary>
    public string Name { get; set; }

    #region Obsolete
    [Obsolete("Refresh By Property")]
    public void UpdateSelfFromLocal() {

    }

    [Obsolete("Refresh By Property")]
    public void UpdateSelfFromWorld() {

    }

    [Obsolete("Refresh By Property")]
    public void UpdateChildrenFromLocal() {

    }

    [Obsolete("Refresh By Property")]
    public void UpdateChildrenFromWorld() {

    }
    #endregion

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public int Order { get; protected set; }

    /// <summary>
    /// 世界空间偏移
    /// </summary>
    public Vector3 Position {
      get {
        if (_Position.Dirty) {
          _Position.Data = (Parent == null ? LocalPosition : Parent.Position + Vector3.Transform(LocalPosition, Parent.Rotation));
        }
        return _Position.Data;
      }
      set {
        _Position.Data = (value);
        _LocalPosition.Dirty = true;
      }
    }
    protected DataStorage<Vector3> _Position;
    /// <summary>
    /// 世界空间旋转
    /// </summary>
    public Quaternion Rotation {
      get {
        if (_Rotation.Dirty) {
          _Rotation.Data = (Parent == null ? LocalRotation : Parent.Rotation * LocalRotation);
        }
        return _Rotation.Data;
      }
      set {
        _Rotation.Data = (value);
        _LocalRotation.Dirty = true;
      }
    }
    protected DataStorage<Quaternion> _Rotation;
    /// <summary>
    /// 相对空间缩放
    /// with out test and func
    /// </summary>
    public Vector3 LocalScale {
      get {
        if (_LocalScale.Dirty) _LocalScale.Dirty = false; return _LocalScale.Data;
      }
      set => _LocalScale.Data = value;
    }
    protected DataStorage<Vector3> _LocalScale;
    /// <summary>
    /// 相对空间偏移
    /// </summary>
    public Vector3 LocalPosition {
      get {
        if (_LocalPosition.Dirty) {
          _LocalPosition.Data = (Parent == null ? Position : Vector3.Transform(Position, Parent.ToLocalMatrix));
        }
        return _LocalPosition.Data;
      }
      set {
        _LocalPosition.Data = (value);
        _Position.Dirty = true;
      }
    }
    protected DataStorage<Vector3> _LocalPosition;
    /// <summary>
    /// 相对空间旋转
    /// </summary>
    public Quaternion LocalRotation {
      get {
        if (_LocalRotation.Dirty) {
          _LocalRotation.Data = (Parent == null ? _LocalRotation.Data : Parent.Rotation * _LocalRotation.Data);
        }
        return _LocalRotation.Data;
      }
      set {
        _LocalRotation.Data = value;
        _Rotation.Dirty = true;
      }
    }
    protected DataStorage<Quaternion> _LocalRotation;

    /// <summary>
    /// 世界空间->相对空间
    /// </summary>
    public Matrix4x4 ToLocalMatrix {
      get =>
        Parent == null
          ? Kits.FromTRS(LocalPosition, LocalRotation, LocalScale)
          : Matrix4x4.Multiply(Parent.ToLocalMatrix, Kits.FromTRS(LocalPosition, LocalRotation, LocalScale));
    }
    /// <summary>
    /// 相对空间->世界空间
    /// </summary>
    public Matrix4x4 ToWorldMatrix { get { Matrix4x4.Invert(ToLocalMatrix, out var Mat); return Mat; } }
    /// <summary>
    /// 世界空间旋转(Euler)
    /// </summary>
    public Vector3 RotationEuler { get => LocalRotation.ToEuler(); set => LocalRotation = Kits.FromEuler(value); }
    /// <summary>
    /// 相对空间旋转(Euler)
    /// </summary>
    public Vector3 LocalRotationEuler { get => Rotation.ToEuler(); set => Rotation = Kits.FromEuler(value); }


    #region TransExten

    /// <summary>
    /// 朝向
    /// </summary>
    /// <param name="Position">世界空间坐标</param>
    public void LookAt(in Vector3 Position) {
      if (this.Position.X == Position.X && this.Position.Z == Position.Z) return;
      var To2From = Vector3.Normalize(Position - this.Position);
      var side = Vector3.Normalize(Vector3.Cross(Vector3.Normalize(Vector3.UnitY), To2From));
      var up = Vector3.Normalize(Vector3.Cross(To2From, side));
      var LMat = new Matrix4x4(
        side.X, side.Y, side.Z, /*-Vector3.Dot(side, this.Position)*/0f,
        up.X, up.Y, up.Z, /*-Vector3.Dot(up, this.Position)*/0f,
        To2From.X, To2From.Y, To2From.Z, /*-Vector3.Dot(To2From, this.Position)*/0f,
        0.0f, 0.0f, 0.0f, 1.0f
        );
      this.Rotation = Quaternion.CreateFromRotationMatrix(LMat);// Kits.FromRotationMatrix2(LMat);
    }
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="Target"></param>
    public void LookAt(in SpaceObject Target) {
      LookAt(Target.Position);
    }

    /// <summary>
    /// 世界空间矢量转换到相对空间矢量
    /// </summary>
    /// <param name="Position">世界空间矢量</param>
    /// <returns></returns>
    public Vector3 ConvertWorldPositionToLocal(in Vector3 Position) {
      return Vector3.Transform(Position, ToLocalMatrix);
    }
    /// <summary>
    /// 世界空间矢量转换到相对空间矢量
    /// </summary>
    /// <param name="Position">世界空间矢量</param>
    public void WorldToLocalPosition(ref Vector3 Position) {
      Position = Vector3.Transform(Position, ToLocalMatrix);
    }
    /// <summary>
    /// 相对空间矢量转换到世界空间矢量
    /// </summary>
    /// <param name="Position">相对空间矢量</param>
    /// <returns></returns>
    public Vector3 ConvertLocalPositionToWorld(in Vector3 Position) {
      return Vector3.Transform(Position, ToWorldMatrix);
    }
    /// <summary>
    /// 相对空间矢量转换到世界空间矢量
    /// </summary>
    /// <param name="Position">相对空间矢量</param>
    public void LocalToWorldPosition(ref Vector3 Position) {
      Position = Vector3.Transform(Position, ToWorldMatrix);
    }

    #endregion


    public int CompareTo([AllowNull] SpaceObject other) => this.GetHashCode().CompareTo(other.GetHashCode());


    public IEnumerator<SpaceObject> GetEnumerator() => Children.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => Children.GetEnumerator();
    public SpaceObject this[int Index] { get => Children[Index]; set => Children[Index] = value; }
    public int Count { get => Children.Count; }

    /// <summary>
    /// 默认世界空间
    /// </summary>
    public static SpaceObject World {
      get {
        var W = new SpaceObject() {
          Name = "0",
        };
        return W;
      }
    }
  }
  /// <summary>
  /// data with dirty flag,if data is dirty,means need refresh first.
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
        Kits.CompareAndSet(value, ref _Data);
        Dirty = false;
      }
    }
    /// <summary>
    /// data must be refresh before use it flag
    /// </summary>
    public bool Dirty { get; set; }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="Data">default data</param>
    /// <param name="SetDirty">defaut flag</param>
    public DataStorage(in T Data, in bool SetDirty = false) {
      _Data = Data;
      Dirty = SetDirty;
    }



  }

}
