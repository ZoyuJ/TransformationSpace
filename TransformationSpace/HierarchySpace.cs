
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
  public class SpaceObject : ITransformHieraryEntity<SpaceObject>, ITransformLifeTime, IEnumerable<SpaceObject>, INotifyPropertyChanged {
    /// <summary>
    /// need this?
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// default
    /// </summary>
    public SpaceObject() {
      _LocalScale = Vector3.One;
      _LocalRotation = Quaternion.Identity;
      _LocalPosition = Vector3.Zero;
      _Rotation = Quaternion.Identity;
      _Position = Vector3.Zero;
      _ToLocalMatrix = Matrix4x4.Identity;
      Matrix4x4.Invert(_ToLocalMatrix, out _ToWorldMatrix);
      Children = new ObservableCollection<SpaceObject>();
      (Children as ObservableCollection<SpaceObject>).CollectionChanged += OnChildrenChanged;
    }
    /// <summary>
    /// 父级
    /// </summary>
    protected SpaceObject _Parent;
    /// <summary>
    /// 父级
    /// </summary>
    public SpaceObject Parent {
      get => _Parent;
      set {
        if (Kits.CompareAndSet(value, ref _Parent)) {
          UpdateSelfFromLocal();
          PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Parent)));
        }
      }
    }
    /// <summary>
    /// 子级
    /// </summary>
    public Collection<SpaceObject> Children { get; protected set; }

    #region Transform
    /// <summary>
    /// 世界空间偏移
    /// </summary>
    public Vector3 Position {
      get => _Position;
      set {
        if (Kits.CompareAndSet(value, ref _Position)) {
          UpdateSelfFromWorld();
          //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Position)));
        }
      }
    }
    /// <summary>
    /// 
    /// </summary>
    protected Vector3 _Position;
    /// <summary>
    /// 世界空间旋转
    /// </summary>
    public Quaternion Rotation {
      get => _Rotation;
      set {
        if (Kits.CompareAndSet(value, ref _Rotation)) {
          UpdateSelfFromWorld();
          //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Rotation)));
        }
      }
    }
    /// <summary>
    /// 
    /// </summary>
    protected Quaternion _Rotation;
    /// <summary>
    /// 相对空间缩放
    /// with out test and func
    /// </summary>
    public Vector3 LocalScale {
      get => _LocalScale;
      set {
        if (Kits.CompareAndSet(value, ref _LocalScale)) {
          UpdateSelfFromLocal();
          //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LocalScale)));
        }
      }
    }
    /// <summary>
    /// 
    /// </summary>
    protected Vector3 _LocalScale;
    /// <summary>
    /// 相对空间偏移
    /// </summary>
    public Vector3 LocalPosition {
      get => _LocalPosition;
      set {
        if (Kits.CompareAndSet(value, ref _LocalPosition)) {
          UpdateSelfFromLocal();
          //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LocalPosition)));
        }
      }
    }
    /// <summary>
    /// 
    /// </summary>
    protected Vector3 _LocalPosition;
    /// <summary>
    /// 相对空间旋转
    /// </summary>
    public Quaternion LocalRotation {
      get => _LocalRotation;
      set {
        if (Kits.CompareAndSet(value, ref _LocalRotation)) {
          UpdateSelfFromLocal();
          //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LocalRotation)));
        }
      }
    }
    /// <summary>
    /// 
    /// </summary>
    protected Quaternion _LocalRotation;
    /// <summary>
    /// 世界空间旋转(Euler)
    /// </summary>
    public Vector3 RotationEuler {
      get => Rotation.ToEuler(); set => Rotation = Kits.FromEuler(value);
    }
    /// <summary>
    /// 相对空间旋转(Euler)
    /// </summary>
    public Vector3 LocalRotationEuler {
      get => LocalRotation.ToEuler(); set => LocalRotation = Kits.FromEuler(value);
    }

    /// <summary>
    /// 世界空间->相对空间
    /// </summary>
    public Matrix4x4 ToLocalMatrix { get => _ToLocalMatrix; }
    internal Matrix4x4 _ToLocalMatrix;
    /// <summary>
    /// 相对空间->世界空间
    /// </summary>
    public Matrix4x4 ToWorldMatrix { get => _ToWorldMatrix; }
    internal Matrix4x4 _ToWorldMatrix;

    /// <summary>
    /// 通过相对空间更新自身TRS和Matrix
    /// </summary>
    public virtual void UpdateSelfFromLocal() {
      if (Parent == null) {
        _Position = LocalPosition;
        _Rotation = LocalRotation;
        _ToLocalMatrix = Kits.FromTRS(LocalPosition, LocalRotation, LocalScale);
      }
      else {
        Position = Parent.Position + Vector3.Transform(LocalPosition, Parent.Rotation);
        Rotation = Parent.Rotation * LocalRotation;
        _ToLocalMatrix = Matrix4x4.Multiply(Parent.ToLocalMatrix, Kits.FromTRS(LocalPosition, LocalRotation, LocalScale));
      }
      Matrix4x4.Invert(ToLocalMatrix, out _ToWorldMatrix);
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SpaceObject)));
      UpdateChildrenFromLocal();
    }
    /// <summary>
    /// 通过世界空间更新自身TRS和Matrix
    /// </summary>
    public virtual void UpdateSelfFromWorld() {
      if (Parent == null) {
        _ToLocalMatrix = Kits.FromTRS(Position, Rotation, LocalScale);
        _LocalPosition = Position;
        _LocalRotation = LocalRotation;
      }
      else {
        _LocalPosition = Vector3.Transform(Position, Parent.ToLocalMatrix);
        _LocalRotation = Quaternion.Multiply(Rotation, Quaternion.CreateFromRotationMatrix(Parent.ToLocalMatrix));
        _ToLocalMatrix = Matrix4x4.Multiply(Parent.ToLocalMatrix, Kits.FromTRS(LocalPosition, LocalRotation, LocalScale));
      }
      Matrix4x4.Invert(ToLocalMatrix, out _ToWorldMatrix);
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SpaceObject)));
      UpdateChildrenFromWorld();
    }
    /// <summary>
    /// 通过相对空间更新子级TRS和Matrix
    /// </summary>
    public virtual void UpdateChildrenFromLocal() {
      if (Children.Count > 0) {
        for (int i = 0; i < Children.Count; i++) {
          Children[i].UpdateSelfFromLocal();
        }
      }
    }
    /// <summary>
    /// 通过世界空间更新子级TRS和Matrix
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
            Item.OnEngage();
          }
        }
      }
      void HandleLost() {
        if (Args.NewItems != null && Args.OldItems.Count > 0) {
          for (int i = 0; i < Args.NewItems.Count; i++) {
            var Item = Args.NewItems[i] as SpaceObject;
            Item.Parent = null;
            Item.OnEject();
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
      //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Children)));
    }
    #endregion

    #region TransExten

    /// <summary>
    /// 朝向
    /// </summary>
    /// <param name="Position">世界空间坐标</param>
    public void LookAt(in Vector3 Position) {
      var To2From = Vector3.Normalize(Position - this.Position);
      var Dot = Vector3.Dot(To2From, Vector3.UnitZ);
      if (Dot == 0) return;
      var side = Vector3.Normalize(Vector3.Cross(Vector3.Normalize(Vector3.UnitY), To2From));
      var up = Vector3.Normalize(Vector3.Cross(To2From, side));
      var LMat = new Matrix4x4(
        side.X, side.Y, side.Z, /*-Vector3.Dot(side, this.Position)*/0f,
        up.X, up.Y, up.Z, /*-Vector3.Dot(up, this.Position)*/0f,
        To2From.X, To2From.Y, To2From.Z, /*-Vector3.Dot(To2From, this.Position)*/0f,
        0.0f, 0.0f, 0.0f, 1.0f
        );
      this.Rotation = Quaternion.CreateFromRotationMatrix(LMat);
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
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    #region Itera
    /// <summary>
    /// 遍历
    /// </summary>
    /// <returns></returns>
    public IEnumerator<SpaceObject> GetEnumerator() {
      return Children.GetEnumerator();
    }
    /// <summary>
    /// 遍历
    /// </summary>
    /// <returns></returns>
    IEnumerator IEnumerable.GetEnumerator() {
      return Children.GetEnumerator();
    }
    /// <summary>
    /// 子级元素数量
    /// </summary>
    public int Count { get => (this.Children?.Count) ?? 0; }
    /// <summary>
    /// 子物体索引
    /// </summary>
    /// <param name="Index"></param>
    /// <returns></returns>
    public SpaceObject this[int Index] {
      get => this.Children[Index]; set => this.Children[Index] = value;
    }

    #endregion

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

    //public int Order { get; protected set; }
    /// <summary>
    /// 当物体加入空间树
    /// </summary>
    public virtual void OnEngage() {
      if (Children.Count > 0)
        for (int i = 0; i < Children.Count; i++) {
          Children[i].OnEngage();
        }
    }
    /// <summary>
    /// 当物体移出空间树
    /// </summary>
    public virtual void OnEject() {
      PropertyChanged = null;
      if (Children.Count > 0)
        for (int i = 0; i < Children.Count; i++) {
          Children[i].OnEject();
        }
    }

    
  }

}

