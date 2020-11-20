namespace Try {
  using System;
  using System.Drawing.Drawing2D;
  using System.Linq;
  using System.Numerics;

  using TransformationSpace;

  class Program {
    static void Main(string[] args) {
      Console.WriteLine("Hello World!");

      SpaceObject World = SpaceObject.World;
      World.Children.Add(new SpaceObject() { Name = "0-0", LocalPosition = Vector3.One, LocalRotation = Quaternion.Identity, LocalScale = Vector3.One });
      World.Children.Add(new SpaceObject() { Name = "0-1", LocalPosition = new Vector3(0.0F, 0.0F, 100.0F), LocalRotation = Kits.FromEuler(new Vector3(0.0F, 0F, 0.0F)), LocalScale = Vector3.One });
      World.Children.Add(new SpaceObject() { Name = "0-2", LocalPosition = Vector3.One * 4.0f, LocalRotation = Quaternion.Identity, LocalScale = Vector3.One });
      World.Children[1].Children.Add(new SpaceObject() { Name = "0-1-0", LocalPosition = new Vector3(0.0F, 100.0F, 0.0F), LocalRotation = Kits.FromEuler(new Vector3(0.0F, 0F, 90.0F)), LocalScale = Vector3.One });

      World.Children.Add(new SpaceObject() { Name = "0-3", LocalPosition = Vector3.One * 5.0f });
      World.Children.Last().

      Console.ReadKey();

    }
    public const float Deg2Rad = (float)(Math.PI / 180.0);
    public const float Rad2Deg = (float)(180.0 / Math.PI);




  }
}
