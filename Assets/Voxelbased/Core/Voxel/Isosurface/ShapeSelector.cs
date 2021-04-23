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
            float padding = 2f;

            switch (shapeType)
            {
                case Shape.None:
                    density = new None();
                    break;
                case Shape.Sphere:
                    density = new Sphere(centerPoint, radius - padding);
                    break;
                case Shape.Cube:
                    density = new Cube(centerPoint, radius - padding);
                    break;
                case Shape.Cylinder:
                    density = new Cylinder(centerPoint, radius - padding);
                    break;
                case Shape.Capsule:
                    density = new Capsule(centerPoint, radius - padding);
                    break;
                case Shape.Cone:
                    density = new Cone(centerPoint, radius - padding);
                    break;
                case Shape.Torus:
                    density = new Torus(centerPoint, radius - padding);
                    break;
                case Shape.Wedge:
                    density = new Wedge(centerPoint, radius - padding);
                    break;
                case Shape.TriangularPrism:
                    density = new TriangularPrism(centerPoint, radius - padding);
                    break;
                case Shape.Heart:
                    density = new Heart(centerPoint, radius - padding);
                    break;
                case Shape.Pyramid:
                    density = new Pyramid(centerPoint, radius - padding);
                    break;
                case Shape.Octahedron:
                    density = new Octahedron(centerPoint, radius - padding);
                    break;
                case Shape.GoursatsSurface:
                    density = new GoursatsSurface(centerPoint, radius - padding);
                    break;
                case Shape.Plane:
                    density = new Plane(centerPoint, radius - padding);
                    break;
                case Shape.Planet:
                    density = new Planet(centerPoint, radius - padding);
                    break;
                default:
                    density = new Sphere(centerPoint, radius - padding);
                    break;
            }
            return density;
        }
    }

    public enum Shape
    {
        None,
        Sphere,
        Cube,
        Cylinder,
        Capsule,
        Cone,
        Torus,
        Wedge,
        TriangularPrism,
        Heart,
        Pyramid,
        Octahedron,
        GoursatsSurface,
        Plane,
        Planet
    }
}
