
namespace TransformationSpace {
  using System.Collections;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.Collections.Specialized;
  using System.ComponentModel;
  using System.Diagnostics.CodeAnalysis;
  using System.Numerics;

  public class SpaceObject : ITransformHieraryEntity<SpaceObject>, IEnumerable<SpaceObject>, INotifyPropertyChanged {
    public string Name { get; set; }

    public SpaceObject() {
      _LocalScale = Vector3.One;
      _LocalRotation = Quaternion.Identity;
      _LocalPosition = Vector3.Zero;
      _LocalMatrix = Matrix4x4.CreateWorld(Vector3.Zero, Vector3.UnitZ, Vector3.UnitY);
      Matrix4x4.Invert(_LocalMatrix, out _WorldMatrix);
      Children = new ObservableCollection<SpaceObject>();
      Children.CollectionChanged += OnChildrenChanged;
    }

    protected SpaceObject _Parent;
    public SpaceObject Parent {
      get => _Parent; set {
        if (Kits.CompareAndSet(value, ref _Parent)) {
          UpdateSelf();
          PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Parent)));
        }
      }
    }
    public ObservableCollection<SpaceObject> Children { get; }

    public int Order { get; protected set; } = 0;

    #region Transform
    protected Vector3 _Position;
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
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
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
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
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
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
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
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
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public Quaternion LocalRotation {
      get => _LocalRotation;
      set {
        if (Kits.CompareAndSet(value, ref _LocalRotation)) {
          UpdateSelf();
          PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LocalRotation)));
        }
      }
    }
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public Vector3 RotationEuler {
      get => Rotation.ToEuler(); set => Rotation = Kits.FromEuler(value);
    }
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public Vector3 LocalRotationEuler {
      get => LocalRotation.ToEuler(); set => LocalRotation = Kits.FromEuler(value);
    }

    //public Matrix4x4 LocalMatrix {
    //  get {
    //    return Matrix4x4.CreateTranslation(LocalPosition) * Matrix4x4.CreateFromQuaternion(LocalRotation) * Matrix4x4.CreateScale(LocalScale);
    //  }
    //}
    public Matrix4x4 _LocalMatrix;
    public Matrix4x4 LocalMatrix { get => _LocalMatrix; }
    protected Matrix4x4 _WorldMatrix;
    public Matrix4x4 WorldMatrix { get => _WorldMatrix; }

    public virtual void UpdateSelf() {
      if (Parent == null) {
        Position = LocalPosition;
        Rotation = LocalRotation;
        _LocalMatrix = Kits.FromTRS(LocalPosition, LocalRotation, LocalScale);
        Matrix4x4.Invert(LocalMatrix, out _WorldMatrix);
      }
      else {
        Position = Parent.Position + Vector3.Transform(LocalPosition, Parent.Rotation);
        Rotation = Quaternion.Multiply(Parent.Rotation, LocalRotation);
        _LocalMatrix = Parent.LocalMatrix * LocalMatrix;
        Matrix4x4.Invert(LocalMatrix, out _WorldMatrix);
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
            var Item = Args.NewItems[i] as SpaceObject;
            Item.Parent = this;
          }
        }
      }
      void HandleLost() {
        if (Args.NewItems != null && Args.OldItems.Count > 0) {
          for (int i = 0; i < Args.NewItems.Count; i++) {
            var Item = Args.NewItems[i] as SpaceObject;
            Item.Parent = null;
          }
        }
      }
      void HandleFull() {
        if (Children != null && Children.Count > 0) {
          for (int i = 0; i < Children.Count; i++) {
            var Item = Children[i];
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
      this.LocalRotation = Quaternion.CreateFromRotationMatrix(LMat);
    }

    public Vector3 ConvertWorldPositionToLocal(in Vector3 Position) {
      return Vector3.Transform(Position, LocalMatrix);
    }
    public void WorldToLocalPosition(ref Vector3 Position) {
      Position = Vector3.Transform(Position, LocalMatrix);
    }

    public Vector3 ConvertLocalPositionToWorld(in Vector3 Position) {
      return Vector3.Transform(Position, WorldMatrix);
    }
    public void LocalToWorldPosition(ref Vector3 Position) {
      Position = Vector3.Transform(Position, WorldMatrix);
    }

    #endregion

    public event PropertyChangedEventHandler PropertyChanged;

    #region Itera
    public IEnumerator<SpaceObject> GetEnumerator() {
      return Children.GetEnumerator();
    }
    IEnumerator IEnumerable.GetEnumerator() {
      return Children.GetEnumerator();
    }
    public int Count { get => (this.Children?.Count) ?? 0; }
    public SpaceObject this[int Index] {
      get => this.Children[Index]; set => this.Children[Index] = value;
    }

    #endregion

    public int CompareTo([AllowNull] SpaceObject other) => this.Order.CompareTo(other.Order);

    public static SpaceObject World {
      get {
        var W = new SpaceObject() {
          Name = "0",
        };
        W.UpdateSelf();
        return W;
      }
    }

  }

}
