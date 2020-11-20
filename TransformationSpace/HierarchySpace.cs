namespace TransformationSpace {
  using System;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.Numerics;
  using System;
  using System.Collections.Generic;
  using System.Collections.Specialized;
  using System.ComponentModel;
  using System.Collections;

  class HierarchySpace {

    public readonly ITransformHieraryEntity Root;


  }

  public class SpaceObject : ITransformHieraryEntity, IEnumerable<ITransformHieraryEntity>, INotifyPropertyChanged {
    public string Name { get; set; }

    public SpaceObject() {
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
    public Vector3 _Scale;
    public Vector3 Scale {
      get => _Scale;
      set {
        if (Kits.CompareAndSet(value, ref _Scale)) {
          UpdateChildren();
          PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Scale)));
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

    //private Vector3 _Rotation3;
    public Vector3 Rotation3 {
      get => new Vector3(Rotation.X, Rotation.Y, Rotation.Z); set {

      }
    }


    public virtual void UpdateSelf() {
      if (Parent == null) {
        Position = LocalPosition;
        Rotation = LocalRotation;
      }
      else {
        Position = Parent.Position + Vector3.Transform(LocalPosition, Parent.Rotation);
        Rotation = Parent.Rotation * LocalRotation;
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
      PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(Children)));

    }

    public event PropertyChangedEventHandler PropertyChanged;

    public IEnumerator<ITransformHieraryEntity> GetEnumerator() {
      return Children.GetEnumerator();
    }
    IEnumerator IEnumerable.GetEnumerator() {
      return Children.GetEnumerator();
    }
  }

  public static class Kits {

    public static bool CompareAndSet<T>(T Value, ref T Target) {
      if (!EqualityComparer<T>.Default.Equals(Value, Target)) {
        Target = Value;
        return true;
      }
      return false;
    }

    public static Vector3 TransformWithRotate(in this Vector3 This, in Quaternion Rotate) {
      float x2 = Rotate.X + Rotate.X;
      float y2 = Rotate.Y + Rotate.Y;
      float z2 = Rotate.Z + Rotate.Z;
      float xx2 = Rotate.X * x2;
      float xy2 = Rotate.X * y2;
      float xz2 = Rotate.X * z2;
      float yy2 = Rotate.Y * y2;
      float yz2 = Rotate.Y * z2;
      float zz2 = Rotate.Z * z2;
      float wx2 = Rotate.W * x2;
      float wy2 = Rotate.W * y2;
      float wz2 = Rotate.W * z2;
      return new Vector3(
        This.X * (1.0f - yy2 - zz2) + This.X * (xy2 - wz2) + This.Z * (xz2 + wy2),
        This.X * (xy2 + wz2) + This.Y * (1.0f - xx2 - zz2) + This.Z * (yz2 - wx2),
        This.X * (xz2 - wy2) + This.Y * (yz2 + wx2) + This.Z * (1.0f - xx2 - yy2));
    }
  }

}
