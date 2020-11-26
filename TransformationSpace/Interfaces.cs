namespace TransformationSpace {
  using System;
  using System.Collections.ObjectModel;
  using System.Numerics;

  /// <summary>
  /// 提供TRS与Matrix相关内容
  /// </summary>
  public interface ITransform {
    /// <summary>
    /// 世界空间偏移
    /// </summary>
    Vector3 Position { get; set; }
    /// <summary>
    /// 世界空间旋转
    /// </summary>
    Quaternion Rotation { get; set; }
    /// <summary>
    /// 相对空间缩放
    /// with out test and func
    /// </summary>
    Vector3 LocalScale { get; set; }
    /// <summary>
    /// 相对空间偏移
    /// </summary>
    Vector3 LocalPosition { get; set; }
    /// <summary>
    /// 相对空间旋转
    /// </summary>
    Quaternion LocalRotation { get; set; }
    /// <summary>
    /// 世界空间->相对空间
    /// </summary>
    Matrix4x4 ToLocalMatrix { get; }
    /// <summary>
    /// 相对空间->世界空间
    /// </summary>
    Matrix4x4 ToWorldMatrix { get; }

    /// <summary>
    /// 世界空间旋转(Euler)
    /// </summary>
    Vector3 RotationEuler { get; set; }
    /// <summary>
    /// 相对空间旋转(Euler)
    /// </summary>
    Vector3 LocalRotationEuler { get; set; }

    /// <summary>
    /// 朝向
    /// </summary>
    /// <param name="Position">世界空间坐标</param>
    void LookAt(in Vector3 Position);
    /// <summary>
    /// 世界空间矢量转换到相对空间矢量
    /// </summary>
    /// <param name="Position">世界空间矢量</param>
    /// <returns></returns>
    Vector3 ConvertWorldPositionToLocal(in Vector3 Position);
    /// <summary>
    /// 世界空间矢量转换到相对空间矢量
    /// </summary>
    /// <param name="Position">世界空间矢量</param>
    void WorldToLocalPosition(ref Vector3 Position);
    /// <summary>
    /// 相对空间矢量转换到世界空间矢量
    /// </summary>
    /// <param name="Position">相对空间矢量</param>
    /// <returns></returns>
    Vector3 ConvertLocalPositionToWorld(in Vector3 Position);
    /// <summary>
    /// 相对空间矢量转换到世界空间矢量
    /// </summary>
    /// <param name="Position">相对空间矢量</param>
    void LocalToWorldPosition(ref Vector3 Position);

  }
  /// <summary>
  /// 或许有新功能，之后脏标记更新方案
  /// </summary>
  public interface IRender {
    /// <summary>
    /// set 0,do comparer,no func
    /// </summary>
    int Order { get; }
    //bool HasChanged { get; set; }
  }
  /// <summary>
  /// 层级与层级事件相关
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public interface ITransformHieraryEntity<T> : IRender, ITransform, IComparable<T> where T : ITransformHieraryEntity<T> {
    /// <summary>
    /// 父级
    /// </summary>
    T Parent { get; set; }
    /// <summary>
    /// 子级
    /// </summary>
    ObservableCollection<T> Children { get; }
    /// <summary>
    /// need this?
    /// </summary>
    string Name { get; set; }
    /// <summary>
    /// 通过相对空间更新自身TRS和Matrix
    /// </summary>
    void UpdateSelfFromLocal();
    /// <summary>
    /// 通过世界空间更新自身TRS和Matrix
    /// </summary>
    void UpdateSelfFromWorld();
    /// <summary>
    /// 通过相对空间更新子级TRS和Matrix
    /// </summary>
    void UpdateChildrenFromLocal();
    /// <summary>
    /// 通过世界空间更新子级TRS和Matrix
    /// </summary>
    void UpdateChildrenFromWorld();
    /// <summary>
    /// 子物体索引
    /// </summary>
    /// <param name="Index"></param>
    /// <returns></returns>
    T this[int Index] { get; set; }
    /// <summary>
    /// 子级元素数量
    /// </summary>
    int Count { get; }
  }



}
