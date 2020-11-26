﻿namespace Try {
  using System;
  using System.Collections.ObjectModel;
  using System.Drawing.Drawing2D;
  using System.Linq;

  //using DeltaEngine.Datatypes;

  using System.Numerics;

  using Newtonsoft.Json;

  using TransformationSpace;

  class Program {
    static void Main(string[] args) {
      Console.WriteLine("Hello World!");

      //Quaternion Q = Quaternion.FromRotationMatrix(Matrix.CreateRotationZYX(new EulerAngles(90f, 0f, 0f)));
      //Console.WriteLine(Q);
      //Console.WriteLine(Q.ToEuler());

      // Y X Z
      //var M = Quaternion.CreateFromRotationMatrix(Matrix4x4.CreateFromYawPitchRoll(0f, 90.0f * Kits.Deg2Rad, 0f));
      //Console.WriteLine(M);
      //Console.WriteLine(M.ToEuler());

      //var Q = new Quaternion(0, 1, 0, 0);
      //Console.WriteLine(Q);
      //Console.WriteLine(Q.ToEuler());
      //var Q2 = new Quaternion(0, -1, 0, 0);
      //Console.WriteLine(Q2);
      //Console.WriteLine(Q2.ToEuler());
      //var MM = Kits.FromEuler(new Vector3(180F, 0F, 180F));
      //Console.WriteLine(MM);
      //Console.WriteLine(MM.ToEuler());

      //var MM2 = Kits.FromEuler(new Vector3(180F, 180F, 180F));
      //Console.WriteLine(MM2);
      //Console.WriteLine(MM2.ToEuler());
      //var MM3 = Kits.FromEuler(new Vector3(-180F, -180F, -180F));
      //Console.WriteLine(MM3);
      //Console.WriteLine(MM3.ToEuler());
      //Console.WriteLine(Math.Acos(Vector3.Dot(Vector3.UnitX, -Vector3.UnitX)) * Kits.Rad2Deg);


      SpaceObject World = SpaceObject.World;
      var B1 = new SpaceObject() { Name = "0-0", LocalPosition = Vector3.Zero, LocalRotation = Quaternion.Identity, LocalScale = Vector3.One };
      var B2 = new SpaceObject() { Name = "0-1", LocalPosition = new Vector3(10.0F, 10.0F, 10.0F), LocalRotation = Kits.FromEuler(new Vector3(0.0F, 0F, 0.0F)), LocalScale = Vector3.One };
      World.Children.Add(B1);
      World.Children.Add(B2);


      var TestPoses = new Vector3[] {
      new Vector3(0,0,0),
      new Vector3(10,0,0),
      new Vector3(0,10,0),
      new Vector3(0,0,10),
      new Vector3(10,10,0),
      new Vector3(0,10,10),
      new Vector3(10,0,10),
      new Vector3(10,10,10),
      };
      var B11 = new SpaceObject() { Name = "0-0-0", LocalPosition = new Vector3(100.0f, 0f, 0f) };
      B1.Children.Add(B11);

      //B11.Position = new Vector3(200F, 0F, 0F);
      //B1.Position = new Vector3(100f, 0f, 0f);

      //B11.LocalRotationEuler = new Vector3(45.0F, 0F, 0F);
      //B1.LocalRotationEuler = new Vector3(90.0F, 0F, 0F);
      //B11.LocalPosition = new Vector3(1F, 0F, 1F);

      //B11.Position = new Vector3(200F, 200F, 0F);
      //B11.Position = new Vector3(200F, 200F, 200F);

      for (int i = 0; i < TestPoses.Length; i++) {
        B2.LocalPosition = TestPoses[i];
        Console.WriteLine(TestPoses[i]);
        B1.LookAt(B2.Position);
        Console.WriteLine($"B1 LP= {B1.LocalPosition}");
        Console.WriteLine($"B2 LP= {B2.LocalPosition}");
        Console.WriteLine($"B1 LR= {B1.LocalRotationEuler} {B1.LocalRotation}");
        Console.WriteLine($"B1 WR= {B1.RotationEuler} {B1.Rotation}");
        Console.WriteLine("======");
      }

      Console.WriteLine("Done");
      Console.ReadKey();

    }

  }

}
