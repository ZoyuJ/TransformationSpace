namespace TransformationSpace.Extension {
  using System;
  using System.Linq;
  using System.Text;

  public static class TransformationSpaceExtension {
    /// <summary>
    /// 向下查找
    /// </summary>
    /// <param name="This"></param>
    /// <param name="Match"></param>
    /// <returns></returns>
    public static SpaceObject FindTransform(this SpaceObject This, Predicate<SpaceObject> Match) {
      if (Match(This)) return This;
      if (This.Count > 0) {
        for (int i = 0; i < This.Children.Count; i++) {
          var Item= This.Children[i].FindTransform(Match);
          if (Item != null) return Item;
        }
      }
      return null;

    }
    /// <summary>
    /// 按Name向下查找
    /// </summary>
    /// <param name="This">查找起点</param>
    /// <param name="NamePath">不包含起始/顶级节点</param>
    /// <returns></returns>
    public static SpaceObject FindTransform(this SpaceObject This, string NamePath) {
      return FindTransform(This, NamePath.Trim('/'), 0);
    }
    private static SpaceObject FindTransform(this SpaceObject This, string NamePath, int Offset) {
      var Start = Offset;
      string Name = null;
      Offset = NamePath.IndexOf('/', Start);
      if (Offset == -1) {
        Name = NamePath.Substring(Start, NamePath.Length - Start);
      }
      else {
        Name = NamePath.Substring(Start, Offset - Start);
      }
      var Item = This.Children.FirstOrDefault(E => E.Name == Name);
      if (Offset == -1) return Item;
      else if (Item != null) return Item.FindTransform(NamePath, Offset + 1);
      else return null;
    }
    /// <summary>
    /// 生成当前节点路径，不包含顶级节点
    /// </summary>
    /// <param name="This"></param>
    /// <returns></returns>
    public static string GetSpaceNamePath(this SpaceObject This) {
      if (This == null) return null;
      StringBuilder _StrB = new StringBuilder(This.Name);
      var _Cur = This.Parent;
      while (_Cur != null) {
        if (_Cur.Parent != null) {
          _StrB.Insert(0, '/');
          _StrB.Insert(0, _Cur.Name);
        }
        _Cur = _Cur.Parent;
      }
      return _StrB.ToString();
    }



  }
}
