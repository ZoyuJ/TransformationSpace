
namespace TransformationSpace {
  using System;
  using System.Collections;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.Collections.Specialized;
  using System.ComponentModel;
  using System.Numerics;

  class HierarchySpace {

    public readonly ITransformHieraryEntity Root;


  }

  public class SpaceObject : ITransformHieraryEntity, IEnumerable<ITransformHieraryEntity>, INotifyPropertyChanged {
    public string Name { get; set; }

    public SpaceObject() {
      _LocalScale = Vector3.One;
      _LocalRotation = Quaternion.Identity;
      _LocalPosition = Vector3.Zero;
      Children = new ObservableCollection<ITransformHieraryEntity>();
      Children.CollectionChanged += OnChildrenChanged;
    }

    protected ITransformHieraryEntity _Parent;
    public ITransformHieraryEntity Parent {
      get => _Parent; set {
        if (Kits.CompareAndSet(value, ref _Parent)) {
          UpdateSelf();
          PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Parent)));
        }
      }
    }
    public ObservableCollection<ITransformHieraryEntity> Children { get; }

    #region Transform
    protected Vector3 _Position;
    public Vector3 Position {
      get => _Position;
      set {
        if (Kits.CompareAndSet(value, ref _Position)) {
          UpdateChildren();
          PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Position)));
        }
      }
    }
    public Quaternion _Rotation;
    public Quaternion Rotation {
      get => _Rotation;
      set {
        if (Kits.CompareAndSet(value, ref _Rotation)) {
          UpdateChildren();
          PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Rotation)));
        }
      }
    }
    public Vector3 _LocalScale;
    public Vector3 LocalScale {
      get => _LocalScale;
      set {
        if (Kits.CompareAndSet(value, ref _LocalScale)) {
          UpdateChildren();
          PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LocalScale)));
        }
      }
    }
    protected Vector3 _LocalPosition;
    public Vector3 LocalPosition {
      get => _LocalPosition;
      set {
        if (Kits.CompareAndSet(value, ref _LocalPosition)) {
          UpdateSelf();
          PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LocalPosition)));
        }
      }
    }
    public Quaternion _LocalRotation;
    public Quaternion LocalRotation {
      get => _LocalRotation;
      set {
        if (Kits.CompareAndSet(value, ref _LocalRotation)) {
          UpdateSelf();
          PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LocalRotation)));
        }
      }
    }

    public Vector3 Rotation3 {
      get => Rotation.ToEuler(); set => Rotation = Kits.FromEuler(value);
    }
    public Vector3 LocalRotation3 {
      get => LocalRotation.ToEuler(); set => LocalRotation = Kits.FromEuler(value);
    }

    //public Matrix4x4 LocalMatrix {
    //  get {
    //    return Matrix4x4.CreateTranslation(LocalPosition) * Matrix4x4.CreateFromQuaternion(LocalRotation) * Matrix4x4.CreateScale(LocalScale);
    //  }
    //}
    public Matrix4x4 LocalMatrix { get; protected set; }

    public virtual void UpdateSelf() {
      if (Parent == null) {
        Position = LocalPosition;
        Rotation = LocalRotation;
        LocalMatrix = Kits.FromTRS(LocalPosition, LocalRotation, LocalScale);
      }
      else {
        Position = Parent.Position + Vector3.Transform(LocalPosition, Parent.Rotation);
        Rotation = Quaternion.Multiply(Parent.Rotation, LocalRotation);
        LocalMatrix = Parent.LocalMatrix * LocalMatrix;
      }
      if (Children.Count > 0)
        for (int i = 0; i < Children.Count; i++) {
          Children[i].UpdateSelf();
        }
    }

    public virtual void UpdateChildren() {
      if (Children.Count > 0) {
        for (int i = 0; i < Children.Count; i++) {
          Children[i].UpdateSelf();
        }
      }
    }
    protected virtual void OnChildrenChanged(object sender, NotifyCollectionChangedEventArgs Args) {
      void HandleNew() {
        if (Args.NewItems != null && Args.NewItems.Count > 0) {
          for (int i = 0; i < Args.NewItems.Count; i++) {
            var Item = Args.NewItems[i] as ITransformHieraryEntity;
            Item.Parent = this;
          }
        }
      }
      void HandleLost() {
        if (Args.NewItems != null && Args.OldItems.Count > 0) {
          for (int i = 0; i < Args.NewItems.Count; i++) {
            var Item = Args.NewItems[i] as ITransformHieraryEntity;
            Item.Parent = null;
          }
        }
      }
      void HandleFull() {
        if (Children != null && Children.Count > 0) {
          for (int i = 0; i < Children.Count; i++) {
            var Item = Children[i] as ITransformHieraryEntity;
            Item.Parent = this;
          }
        }
      }
      switch (Args.Action) {
        case NotifyCollectionChangedAction.Add:
          HandleNew();
          break;
        case NotifyCollectionChangedAction.Remove:
          HandleLost();
          break;
        case NotifyCollectionChangedAction.Replace:
          HandleNew();
          HandleLost();
          break;
        case NotifyCollectionChangedAction.Move:
          break;
        case NotifyCollectionChangedAction.Reset:
          HandleFull();
          break;
        default:
          break;
      }
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Children)));

    }
    #endregion

    #region TransExten

    public void LookAt(in Vector3 Position) {
      var LMat = Matrix4x4.CreateLookAt(this.Position, Position, Vector3.UnitZ);
      this.Rotation = Quaternion.CreateFromRotationMatrix(LMat);
    }

    public Vector3 WorldToLocalPosition(in Vector3 Position) {
      return Vector3.Transform(Position, LocalMatrix);
    }





    #endregion

    public event PropertyChangedEventHandler PropertyChanged;


    #region Itera
    public IEnumerator<ITransformHieraryEntity> GetEnumerator() {
      return Children.GetEnumerator();
    }
    IEnumerator IEnumerable.GetEnumerator() {
      return Children.GetEnumerator();
    }
    public int Count { get => (this.Children?.Count) ?? 0; }
    public ITransformHieraryEntity this[int Index] {
      get => this.Children[Index]; set => this.Children[Index] = value;
    }

    #endregion


    public static SpaceObject World {
      get {
        var W = new SpaceObject() {
          Parent = null,
          LocalMatrix = Matrix4x4.CreateWorld(Vector3.Zero, Vector3.UnitZ, Vector3.UnitY),
          Position = Vector3.Zero,
          Rotation = Quaternion.Identity,
          LocalScale = Vector3.One,
          Name = "0",
        };
        W.UpdateSelf();
        return W;
      }
    }

  }

  public static class Kits {
    public const float Deg2Rad = (float)(Math.PI / 180.0);
    public const float Rad2Deg = (float)(180.0 / Math.PI);

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
    public static Quaternion FromEuler(in Vector3 Rotate) => Quaternion.CreateFromYawPitchRoll((Rotate.X * Deg2Rad) % 180.0f, (Rotate.Y * Deg2Rad) % 180.0f, (Rotate.Z * Deg2Rad) % 180.0f);
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
