using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VoxelbasedCom
{
    public class ShapeSelector
    {
        /// <summary>
        /// Turn a shape enum into a density function
        /// </summary>
        public Density GetShapeDensity(Shape shapeType, Vector3 centerPoint, float radius)
        {
            Density density;

            switch (shapeType)
            {
                case Shape.None:
                    density = new None();
                    break;
                case Shape.Sphere:
                    density = new Sphere(centerPoint, radius);
                    break;
                case Shape.Cube:
                    density = new Cube(centerPoint, radius);
                    break;
                case Shape.Cylinder:
                    density = new Cylinder(centerPoint, radius);
                    break;
                case Shape.Capsule:
                    density = new Capsule(centerPoint, radius);
                    break;
                case Shape.Cone:
                    density = new Cone(centerPoint, radius);
                    break;
                case Shape.Torus:
                    density = new Torus(centerPoint, radius);
                    break;
                case Shape.Wedge:
                    density = new Wedge(centerPoint, radius);
                    break;
                case Shape.TriangularPrism:
                    density = new TriangularPrism(centerPoint, radius);
                    break;
                case Shape.Heart:
                    density = new Heart(centerPoint, radius);
                    break;
                case Shape.Pyramid:
                    density = new Pyramid(centerPoint, radius);
                    break;
                case Shape.Octahedron:
                    density = new Octahedron(centerPoint, radius);
                    break;
                case Shape.GoursatsSurface:
                    density = new GoursatsSurface(centerPoint, radius);
                    break;
                case Shape.Plane:
                    density = new Plane(centerPoint, radius);
                    break;
                case Shape.Planet:
                    density = new Planet(centerPoint, radius);
                    break;
                default:
                    density = new Sphere(centerPoint, radius);
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
