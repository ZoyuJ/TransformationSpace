# TransformationSpace
  NO RENDERING
  NO CAMERA

  1.manage transform only,
  2.scale value just a (number,number,number),
  3.
        +y
        ©§ 
        ©§
        ©ï©¥©¥©¥©¥©¥©¥ +x
       ¨u
      ¨u 
    +z

  4.redefined lookat matrix generator,it gonna look at the point by 'eye' not 'ass eye'
  5.redefined Quaternion to Euler,
      if rotate 2 axises 180¡ã or -180¡ã gonna change to rotate other axis:
        for example£º
        in Numerics.Vectors Quertaion2Euler (0,1,0,0) => (180,0,180)
        in this dll Quertaion2Euler (0,1,0,0) => (0,90,0)
      in order to easier to control PTZ by Euler
  
  6.update children object by event/notification,include Rotation¡¢LocalRotation¡¢Position¡¢LocalPosition¡¢ToLocalMatrix¡¢ToWorldMatrix.

# TransformationSpace.Serializa
  1.provide some implementations base on [Newtonsoft.Json.JsonConverter](https://www.newtonsoft.com/json/help/html/CustomJsonConverter.htm) 
    for System.Numerics.Vectors.Vector3,System.Numerics.Vectors.Quaternion,TransformationSpace.SpaceObject.
    for example:
      Vector3:X=1.1,Y=2.2,Z=3.3 => [1.1,2.2,3.3] type=Array
      Quaternion:X=1.1,Y=2.2,Z=3.3,W=4.4 => [1.1,2.2,3.3,4.4] type=Array
      SpaceObject: => {"Name":"","Parent":null,"Order":<int>,"LocalPosition":<Vector3>,"LocalRotation":<Quaternion>,"LocalScale":<Vector3>,"Children":[<SpaceObject>]} type=Object
    ¡ù Newtonsoft.Json provides good way to serialization/desrialization.