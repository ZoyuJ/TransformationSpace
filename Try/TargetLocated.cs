namespace Try {
  using System;
  using System.Collections.Generic;
  using System.Numerics;
  using System.Text;

  using TransformationSpace;

  class TargetLocated : SpaceObject {

    public TargetLocated() {
      _Transform = new SpaceObject();
      _Transform.LocalPosition = new Vector3(0, 0, 0);
      _Random = new Random();
    }

    public SpaceObject PTZ1;
    public readonly SpaceObject _Transform;
    private readonly Random _Random;

    int I = 0;
    public void NextPosition() {
      //LocalPosition = new Vector3(Convert.ToSingle(_Random.Next(0, 15) + _Random.NextDouble()), Convert.ToSingle(_Random.Next(0, 100) + _Random.NextDouble()), 0f);
      I = I % __Pos.Length;
      LocalPosition = __Pos[I];
      I++;
    }

    static readonly Vector3[] __Pos = new Vector3[] {
      //PTZ=(750,400,20000)

      //new Vector3(0,0,0),
      //new Vector3(750,0,0),
      //new Vector3(1500,0,0),

      //new Vector3(0,0,3300),
      //new Vector3(750,0,3300),
      //new Vector3(1500,0,3300),

      //new Vector3(0,0,6600),
      //new Vector3(750,0,6600),
      //new Vector3(1500,0,6600),

      //new Vector3(0,0,9900),
      //new Vector3(750,0,9900),
      //new Vector3(1500,0,9900),

      //new Vector3(0,400,20000), //X 0, Y -90 正右
      new Vector3(0,400,20750), //X 0, Y -90 右前
      //new Vector3(1500,400,20750), //X 0, Y -90 左前
      //new Vector3(1500,400,20000),//X 0, Y 90 正左

      new Vector3(1500,400,19250),//X 0, Y 90 左后
      new Vector3(0,400,19250),//X 0, Y 90 右后

      new Vector3(750,400,0),//X 0, Y 180|-180 正后
      new Vector3(750,400,40000),//X 0, Y 0 正前

      //new Vector3(750,400,20000), //X 0, Y -90 正左
      //new Vector3(750,400,20000),//X 0, Y 90 正右
      //new Vector3(750,400,20000),//X 0, Y 180|-180 正后
      //new Vector3(750,400,40000),//X 0, Y 0 正前


    };


  }
}
