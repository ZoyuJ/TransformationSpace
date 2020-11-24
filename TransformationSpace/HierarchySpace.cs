﻿
namespace TransformationSpace {
  using System.Collections;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.Collections.Specialized;
  using System.ComponentModel;
  using System.Diagnostics.CodeAnalysis;
  using System.Numerics;

  /// <summary>
  /// TramsformationSpace节点
  /// </summary>
  public class SpaceObject : ITransformHieraryEntity<SpaceObject>, IEnumerable<SpaceObject>, INotifyPropertyChanged {
    protected static readonly Matrix4x4 WorldBase;
    static SpaceObject() {
      WorldBase = Matrix4x4.CreateWorld(Vector3.Zero, Vector3.UnitZ, Vector3.UnitY);

    }
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// default
    /// </summary>
    public SpaceObject() {
      _LocalScale = Vector3.One;
      _LocalRotation = Quaternion.Identity;
      _LocalPosition = Vector3.Zero;
      _ToLocalMatrix = WorldBase;
      Matrix4x4.Invert(_ToLocalMatrix, out _ToWorldMatrix);
      Children = new ObservableCollection<SpaceObject>();
      Children.CollectionChanged += OnChildrenChanged;
    }
    /// <summary>
    /// 父级
    /// </summary>
    protected SpaceObject _Parent;
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public SpaceObject Parent {
      get => _Parent; set {
        if (Kits.CompareAndSet(value, ref _Parent)) {
          UpdateSelfFromLocal();
          PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Parent)));
        }
      }
    }
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public ObservableCollection<SpaceObject> Children { get; }
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public int Order { get; set; } = 0;

    #region Transform
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public Vector3 Position {
      get => _Position;
      set {
        if (Kits.CompareAndSet(value, ref _Position)) {
          UpdateSelfFromWorld();
          //UpdateChildren();
          //LocalPosition = Vector3.Transform(value, _ToLocalMatrix);
          PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Position)));
        }
      }
    }
    internal Vector3 _Position;
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public Quaternion Rotation {
      get => _Rotation;
      set {
        if (Kits.CompareAndSet(value, ref _Rotation)) {
          UpdateSelfFromWorld();
          //UpdateChildren();
          //LocalRotation = Quaternion.Multiply(value, Quaternion.CreateFromRotationMatrix(_ToLocalMatrix));
          PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Rotation)));
        }
      }
    }
    internal Quaternion _Rotation;
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public Vector3 LocalScale {
      get => _LocalScale;
      set {
        if (Kits.CompareAndSet(value, ref _LocalScale)) {
          UpdateSelfFromLocal();
          PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LocalScale)));
        }
      }
    }
    internal Vector3 _LocalScale;
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public Vector3 LocalPosition {
      get => _LocalPosition;
      set {
        if (Kits.CompareAndSet(value, ref _LocalPosition)) {
          UpdateSelfFromLocal();
          PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LocalPosition)));
        }
      }
    }
    internal Vector3 _LocalPosition;
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public Quaternion LocalRotation {
      get => _LocalRotation;
      set {
        if (Kits.CompareAndSet(value, ref _LocalRotation)) {
          UpdateSelfFromLocal();
          PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LocalRotation)));
        }
      }
    }
    internal Quaternion _LocalRotation;
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

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public Matrix4x4 ToLocalMatrix { get => _ToLocalMatrix; }
    internal Matrix4x4 _ToLocalMatrix;
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public Matrix4x4 ToWorldMatrix { get => _ToWorldMatrix; }
    internal Matrix4x4 _ToWorldMatrix;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public virtual void UpdateSelfFromLocal() {
      if (Parent == null) {
        _ToLocalMatrix = Kits.FromTRS(LocalPosition, LocalRotation, LocalScale);
        _Position = LocalPosition;
        _Rotation = LocalRotation;
      }
      else {
        _ToLocalMatrix = Matrix4x4.Multiply(Parent.ToLocalMatrix, Kits.FromTRS(LocalPosition, LocalRotation, LocalScale));
        _Position = Vector3.Transform(Parent.Position, _ToLocalMatrix);
        _Rotation = Quaternion.Multiply(Parent.Rotation, Quaternion.CreateFromRotationMatrix(_ToLocalMatrix));
      }
      Matrix4x4.Invert(ToLocalMatrix, out _ToWorldMatrix);
      UpdateChildrenFromLocal();
    }
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public virtual void UpdateSelfFromWorld() {
      if (Parent == null) {
        _ToLocalMatrix = Kits.FromTRS(Position, Rotation, LocalScale);
        _LocalPosition = Position;
        _LocalRotation = LocalRotation;
      }
      else {
        _LocalPosition = Vector3.Transform(Position, Parent.ToWorldMatrix);
        _LocalRotation = Quaternion.Multiply(Rotation, Quaternion.CreateFromRotationMatrix(Parent.ToLocalMatrix));
        _ToLocalMatrix = Matrix4x4.Multiply(Parent.ToLocalMatrix, Kits.FromTRS(LocalPosition, LocalRotation, LocalScale));
      }
      Matrix4x4.Invert(ToLocalMatrix, out _ToWorldMatrix);
      UpdateChildrenFromWorld();
    }
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public virtual void UpdateChildrenFromLocal() {
      if (Children.Count > 0) {
        for (int i = 0; i < Children.Count; i++) {
          Children[i].UpdateSelfFromLocal();
        }
      }
    }
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public virtual void UpdateChildrenFromWorld() {
      if (Children.Count > 0) {
        for (int i = 0; i < Children.Count; i++) {
          Children[i].UpdateSelfFromWorld();
        }
      }
    }
    /// <summary>
    /// 当子级节点改变
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="Args"></param>
    protected virtual void OnChildrenChanged(object sender, NotifyCollectionChangedEventArgs Args) {
      void HandleNew() {
        if (Args.NewItems != null && Args.NewItems.Count > 0) {
          for (int i = 0; i < Args.NewItems.Count; i++) {
            var Item = Args.NewItems[i] as SpaceObject;
            if (Item.Parent != null) {
              Item.Parent.Children.Remove(Item);
            }
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

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public void LookAt(in Vector3 Position) {
      if (this.Position.ToClose(Position)) return;
      var LMat = Matrix4x4.CreateLookAt(this.Position, Position,
        Vector3.Cross(Vector3.Normalize(Vector3.UnitY), Vector3.Normalize(this.Position - Position)) == Vector3.Zero ? Vector3.UnitX : Vector3.UnitY);
      this.Rotation = Quaternion.CreateFromRotationMatrix(LMat);
    }
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public Vector3 ConvertWorldPositionToLocal(in Vector3 Position) {
      return Vector3.Transform(Position, ToLocalMatrix);
    }
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public void WorldToLocalPosition(ref Vector3 Position) {
      Position = Vector3.Transform(Position, ToLocalMatrix);
    }
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public Vector3 ConvertLocalPositionToWorld(in Vector3 Position) {
      return Vector3.Transform(Position, ToWorldMatrix);
    }
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public void LocalToWorldPosition(ref Vector3 Position) {
      Position = Vector3.Transform(Position, ToWorldMatrix);
    }

    #endregion
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    #region Itera
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public IEnumerator<SpaceObject> GetEnumerator() {
      return Children.GetEnumerator();
    }
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    IEnumerator IEnumerable.GetEnumerator() {
      return Children.GetEnumerator();
    }
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public int Count { get => (this.Children?.Count) ?? 0; }
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="Index"></param>
    /// <returns></returns>
    public SpaceObject this[int Index] {
      get => this.Children[Index]; set => this.Children[Index] = value;
    }

    #endregion
    /// <summary>
    /// 比较
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public int CompareTo([AllowNull] SpaceObject other) => this.GetHashCode().CompareTo(other.GetHashCode());

    /// <summary>
    /// 默认世界空间
    /// </summary>
    public static SpaceObject World {
      get {
        var W = new SpaceObject() {
          Name = "0",
        };
        W.UpdateSelfFromLocal();
        return W;
      }
    }

  }

  //public class SpaceNode : ITransformHieraryEntity<SpaceNode>, IEnumerable<SpaceNode>, INotifyPropertyChanged {
  //  /// <summary>
  //  /// <inheritdoc/>
  //  /// </summary>
  //  public string Name { get; set; }
  //  /// <summary>
  //  /// default
  //  /// </summary>
  //  public SpaceNode() {
  //    LocalScale = new DataStorage<Vector3>(Vector3.One);
  //    LocalRotation = new DataStorage<Quaternion>(Quaternion.Identity);
  //    LocalPosition = new DataStorage<Vector3>(Vector3.Zero);
  //    LocalMatrix = Matrix4x4.CreateWorld(Vector3.Zero, Vector3.UnitZ, Vector3.UnitY);
  //    Matrix4x4.Invert(_LocalMatrix, out _WorldMatrix);
  //    Children = new ObservableCollection<SpaceNode>();
  //    Children.CollectionChanged += OnChildrenChanged;
  //  }
  //  /// <summary>
  //  /// 父级
  //  /// </summary>
  //  protected SpaceNode _Parent;
  //  /// <summary>
  //  /// <inheritdoc/>
  //  /// </summary>
  //  public SpaceNode Parent {
  //    get => _Parent; set {
  //      if (Kits.CompareAndSet(value, ref _Parent)) {
  //        UpdateSelf();
  //        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Parent)));
  //      }
  //    }
  //  }
  //  /// <summary>
  //  /// <inheritdoc/>
  //  /// </summary>
  //  public ObservableCollection<SpaceNode> Children { get; }
  //  /// <summary>
  //  /// <inheritdoc/>
  //  /// </summary>
  //  public int Order { get; set; } = 0;

  //  #region Transform
  //  /// <summary>
  //  /// <inheritdoc/>
  //  /// </summary>
  //  Vector3 ITransform.Position { get => Position.Data; set => Position.Data = value; }
  //  /// <summary>
  //  /// <inheritdoc/>
  //  /// </summary>
  //  Quaternion ITransform.Rotation { get => Rotation.Data; set => Rotation.Data = value; }
  //  /// <summary>
  //  /// <inheritdoc/>
  //  /// </summary>
  //  Vector3 ITransform.LocalScale { get => LocalScale.Data; set => LocalScale.Data = value; }
  //  /// <summary>
  //  /// <inheritdoc/>
  //  /// </summary>
  //  Vector3 ITransform.LocalPosition { get => LocalScale.Data; set => LocalScale.Data = value; }
  //  /// <summary>
  //  /// <inheritdoc/>
  //  /// </summary>
  //  Quaternion ITransform.LocalRotation { get => LocalRotation.Data; set => LocalRotation.Data = value; }
  //  /// <summary>
  //  /// <inheritdoc/>
  //  /// </summary>
  //  public DataStorage<Vector3> Position;
  //  /// <summary>
  //  /// <inheritdoc/>
  //  /// </summary>
  //  public DataStorage<Quaternion> Rotation;
  //  /// <summary>
  //  /// <inheritdoc/>
  //  /// </summary>
  //  public DataStorage<Vector3> LocalScale;
  //  /// <summary>
  //  /// <inheritdoc/>
  //  /// </summary>
  //  public DataStorage<Vector3> LocalPosition;
  //  /// <summary>
  //  /// <inheritdoc/>
  //  /// </summary>
  //  public DataStorage<Quaternion> LocalRotation;
  //  /// <summary>
  //  /// <inheritdoc/>
  //  /// </summary>
  //  public Vector3 RotationEuler {
  //    get => Rotation.Data.ToEuler(); set => Rotation.Data = Kits.FromEuler(value);
  //  }
  //  /// <summary>
  //  /// <inheritdoc/>
  //  /// </summary>
  //  public Vector3 LocalRotationEuler {
  //    get => LocalRotation.Data.ToEuler(); set => LocalRotation.Data = Kits.FromEuler(value);
  //  }

  //  /// <summary>
  //  /// <inheritdoc/>
  //  /// </summary>
  //  public Matrix4x4 LocalMatrix { get => _LocalMatrix; set => _LocalMatrix = value; }
  //  internal Matrix4x4 _LocalMatrix;
  //  /// <summary>
  //  /// <inheritdoc/>
  //  /// </summary>
  //  public Matrix4x4 WorldMatrix { get => _WorldMatrix; set => _WorldMatrix = value; }
  //  internal Matrix4x4 _WorldMatrix;
  //  /// <summary>
  //  /// <inheritdoc/>
  //  /// </summary>
  //  public virtual void UpdateSelf() {
  //    if (Parent == null) {
  //      Position = LocalPosition;
  //      Rotation = LocalRotation;
  //      _LocalMatrix = Kits.FromTRS(LocalPosition.Data, LocalRotation.Data, LocalScale.Data);
  //      Matrix4x4.Invert(LocalMatrix, out _WorldMatrix);
  //    }
  //    else {
  //      Position = Parent.Position + Vector3.Transform(LocalPosition.Data, Parent.Rotation.Data);
  //      Rotation = Quaternion.Multiply(Parent.Rotation.Data, LocalRotation.Data);
  //      _LocalMatrix = Parent.LocalMatrix * LocalMatrix;
  //      Matrix4x4.Invert(LocalMatrix, out _WorldMatrix);
  //    }
  //    if (Children.Count > 0)
  //      for (int i = 0; i < Children.Count; i++) {
  //        Children[i].UpdateSelf();
  //      }
  //  }
  //  /// <summary>
  //  /// <inheritdoc/>
  //  /// </summary>
  //  public virtual void UpdateChildren() {
  //    if (Children.Count > 0) {
  //      for (int i = 0; i < Children.Count; i++) {
  //        Children[i].UpdateSelf();
  //      }
  //    }
  //  }
  //  /// <summary>
  //  /// <inheritdoc/>
  //  /// </summary>
  //  protected virtual void OnChildrenChanged(object sender, NotifyCollectionChangedEventArgs Args) {
  //    void HandleNew() {
  //      if (Args.NewItems != null && Args.NewItems.Count > 0) {
  //        for (int i = 0; i < Args.NewItems.Count; i++) {
  //          var Item = Args.NewItems[i] as SpaceNode;
  //          if (Item.Parent != null) {
  //            Item.Parent.Children.Remove(Item);
  //          }
  //          Item.Parent = this;
  //        }
  //      }
  //    }
  //    void HandleLost() {
  //      if (Args.NewItems != null && Args.OldItems.Count > 0) {
  //        for (int i = 0; i < Args.NewItems.Count; i++) {
  //          var Item = Args.NewItems[i] as SpaceNode;
  //          Item.Parent = null;
  //        }
  //      }
  //    }
  //    void HandleFull() {
  //      if (Children != null && Children.Count > 0) {
  //        for (int i = 0; i < Children.Count; i++) {
  //          var Item = Children[i];
  //          Item.Parent = this;
  //        }
  //      }
  //    }
  //    switch (Args.Action) {
  //      case NotifyCollectionChangedAction.Add:
  //        HandleNew();
  //        break;
  //      case NotifyCollectionChangedAction.Remove:
  //        HandleLost();
  //        break;
  //      case NotifyCollectionChangedAction.Replace:
  //        HandleNew();
  //        HandleLost();
  //        break;
  //      case NotifyCollectionChangedAction.Move:
  //        break;
  //      case NotifyCollectionChangedAction.Reset:
  //        HandleFull();
  //        break;
  //      default:
  //        break;
  //    }
  //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Children)));

  //  }
  //  #endregion

  //  public virtual void Update() {
  //    UpdateSelf();
  //  }

  //  #region TransExten
  //  /// <summary>
  //  /// <inheritdoc/>
  //  /// </summary>
  //  public void LookAt(in Vector3 Position) {
  //    var LMat = Matrix4x4.CreateLookAt(this.Position.Data, Position, Vector3.UnitZ);
  //    this.LocalRotation.Data = Quaternion.CreateFromRotationMatrix(LMat);
  //  }
  //  /// <summary>
  //  /// <inheritdoc/>
  //  /// </summary>
  //  public Vector3 ConvertWorldPositionToLocal(in Vector3 Position) {
  //    return Vector3.Transform(Position, LocalMatrix);
  //  }
  //  /// <summary>
  //  /// <inheritdoc/>
  //  /// </summary>
  //  public void WorldToLocalPosition(ref Vector3 Position) {
  //    Position = Vector3.Transform(Position, LocalMatrix);
  //  }
  //  /// <summary>
  //  /// <inheritdoc/>
  //  /// </summary>
  //  public Vector3 ConvertLocalPositionToWorld(in Vector3 Position) {
  //    return Vector3.Transform(Position, WorldMatrix);
  //  }
  //  /// <summary>
  //  /// <inheritdoc/>
  //  /// </summary>
  //  public void LocalToWorldPosition(ref Vector3 Position) {
  //    Position = Vector3.Transform(Position, WorldMatrix);
  //  }

  //  #endregion
  //  /// <summary>
  //  /// <inheritdoc/>
  //  /// </summary>
  //  public event PropertyChangedEventHandler PropertyChanged;

  //  #region Itera
  //  /// <summary>
  //  /// <inheritdoc/>
  //  /// </summary>
  //  /// <returns></returns>
  //  public IEnumerator<SpaceNode> GetEnumerator() {
  //    return Children.GetEnumerator();
  //  }
  //  /// <summary>
  //  /// <inheritdoc/>
  //  /// </summary>
  //  /// <returns></returns>
  //  IEnumerator IEnumerable.GetEnumerator() {
  //    return Children.GetEnumerator();
  //  }
  //  /// <summary>
  //  /// <inheritdoc/>
  //  /// </summary>
  //  public int Count { get => (this.Children?.Count) ?? 0; }
  //  /// <summary>
  //  /// <inheritdoc/>
  //  /// </summary>
  //  /// <param name="Index"></param>
  //  /// <returns></returns>
  //  public SpaceNode this[int Index] {
  //    get => this.Children[Index]; set => this.Children[Index] = value;
  //  }

  //  #endregion
  //  /// <summary>
  //  /// 比较
  //  /// </summary>
  //  /// <param name="other"></param>
  //  /// <returns></returns>
  //  public int CompareTo([AllowNull] SpaceNode other) => this.GetHashCode().CompareTo(other.GetHashCode());

  //  /// <summary>
  //  /// 默认世界空间
  //  /// </summary>
  //  public static SpaceNode World {
  //    get {
  //      var W = new SpaceNode() {
  //        Name = "0",
  //      };
  //      W.UpdateSelf();
  //      return W;
  //    }
  //  }

  //}
}

