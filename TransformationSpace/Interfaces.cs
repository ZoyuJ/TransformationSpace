namespace TransformationSpace {
  using System;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.Numerics;
  using System.Text;

  public interface ITransform {
    Vector3 Position { get; set; }
    Quaternion Rotation { get; set; }
    Vector3 Scale { get; set; }
    Vector3 LocalPosition { get; set; }
    Quaternion LocalRotation { get; set; }

  }

  public interface ITransformHieraryEntity : ITransform {
    ITransformHieraryEntity Parent { get; set; }
    ObservableCollection<ITransformHieraryEntity> Children { get; }
    /// <summary>
    /// need this?
    /// </summary>
    string Name { get; set; }
    void UpdateSelf();
    void UpdateChildren();
  }

}
