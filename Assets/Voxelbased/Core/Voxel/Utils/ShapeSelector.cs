using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VoxelbasedCom
{
    public class ShapeSelector
    {
        public Density GetShapeDensity(Shape shapeType, Vector3 centerPoint, float radius)
        {
            Density density;

            switch (shapeType)
            {
                case Shape.Sphere:
                    density = new Sphere(centerPoint, radius - 1f);
                    break;
                case Shape.Cube:
                    density = new Cube(centerPoint, radius - 1f, Quaternion.identity);
                    break;
                case Shape.Capsule:
                    density = new Capsule(centerPoint, radius);
                    break;
                case Shape.Torus:
                    density = new Torus(centerPoint, radius - 5f);
                    break;
                case Shape.Heart:
                    density = new Heart(centerPoint, radius);
                    break;
                case Shape.Pyramid:
                    density = new Pyramid(centerPoint, radius);
                    break;
                case Shape.Rubin:
                    density = new Rubin(centerPoint, radius);
                    break;
                case Shape.GoursatsSurface:
                    density = new GoursatsSurface(centerPoint, radius);
                    break;
                case Shape.Plane:
                    density = new Plane(centerPoint.y / 2);
                    break;
                default:
                    density = new Rubin(centerPoint, radius);
                    break;
            }
            return density;
        }
    }

    public enum Shape
    {
        Sphere,
        Cube,
        Capsule,
        Torus,
        Heart,
        Pyramid,
        Rubin,
        GoursatsSurface,
        Plane,
    }
}
