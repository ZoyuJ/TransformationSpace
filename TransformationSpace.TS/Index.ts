///<reference path="../TransformationSpace.Kits/Index.ts" />

namespace TransformationSpace {

  export interface ITransform {

  }
  export class Transform implements ITransform { }
  export interface ITransformLifeTime {
    OnEngage();
    OnOnEject();
  }
  export interface ITransformHieraryEntity<T extends ITransform> {
    Parent: T;
    Children: Array<T>,
    Name: string;

  }
}