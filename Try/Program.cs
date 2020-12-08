namespace Try {
  using System;
  using System.Collections.ObjectModel;
  using System.Drawing.Drawing2D;
  using System.Linq;

  //using DeltaEngine.Datatypes;

  using System.Numerics;
  using System.Threading.Tasks;

  using Newtonsoft.Json;

  using TransformationSpace;
  using TransformationSpace.Extension;

  class Program {
    static void Main(string[] args) {
      Console.WriteLine("Hello World!");



      //Console.WriteLine(Kits.FromEuler(new Vector3(0, -45, 0)));
      //Console.WriteLine(Kits.FromEuler(new Vector3(180, 45, 180)));
      //Console.WriteLine(Kits.FromEuler(new Vector3(0, 135, 0)));

      //var World = SpaceObject.World;

      //var A00 = new PTZCtrl_SP1510();
      //var A01 = new TargetLocated();


      //World.Children.Add(A00);
      //World.Children.Add(A01);
      //A00.PropertyChanged += (sender, args) => {
      //  switch (args.PropertyName) {
      //    case nameof(SpaceObject):
      //      Console.WriteLine($"Person1 {A01.LocalPosition} ptz1 {A00.LocalPosition}");
      //      Console.WriteLine($"Person1 ptz1 W {A00.RotationEuler} {A00.Rotation}");
      //      Console.WriteLine($"Person1 ptz1 L {A00.LocalRotationEuler} {A00.LocalRotation}");
      //      A00.TurnToPosition();
      //      Console.WriteLine($"==================================================");
      //      break;
      //    default:
      //      break;
      //  }
      //};
      //Console.WriteLine($"Person1 {A01.LocalPosition} ptz1 {A00.LocalPosition}");
      //Console.WriteLine($"Person1 ptz1 W {A00.RotationEuler} {A00.Rotation}");
      //Console.WriteLine($"Person1 ptz1 L {A00.LocalRotationEuler} {A00.LocalRotation}");
      ////Task.Run(async () => {
      //while (true) {
      //  var K = Console.ReadKey().Key;
      //  if (K == ConsoleKey.K) {
      //    Console.WriteLine("next pos");
      //    A01.NextPosition();
      //    A00.LookAt(A01);

      //  }
      //  else if (K == ConsoleKey.Q) {
      //    Console.WriteLine("Done");
      //    return;
      //  }
      //  //await Task.Delay(100);
      //}
      ////});

      Console.WriteLine("Done");
      Console.ReadKey();

    }

  }

}
