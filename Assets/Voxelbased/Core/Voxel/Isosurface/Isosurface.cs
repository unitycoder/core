using System;
using UnityEngine;
using System.Collections.Generic;

namespace VoxelbasedCom
{
    /// <summary>
    /// Actual class to get the density
    /// </summary>
    public class Isosurface
    {
        public ShapeSelector shapeSelector;
        public Density density;
        public IsosurfaceAlgorithm isosurfaceAlgorithm;
        private Dictionary<Vector3, BaseModification> modifiers;

        public Isosurface(ShapeSelector shapeSelector, Density density, IsosurfaceAlgorithm algorithm, Dictionary<Vector3, BaseModification> modifiers)
        {
            this.isosurfaceAlgorithm = algorithm;
            this.density = density;
            this.modifiers = modifiers;
            this.shapeSelector = shapeSelector;
        }
        /// <summary>
        /// Getting the density, modified by shapes
        /// </summary>
        public virtual float GetDensity(float x, float y, float z)
        {
            float baseDensity = density.GetDensity(x, y, z);
            //Modify the density
            foreach (BaseModification modification in modifiers.Values)
            {
                Density shapeDensity = shapeSelector.GetShapeDensity(modification.shapeType, modification.position, modification.shapeSize);

                float modificationDensity = shapeDensity.GetDensity(x,y,z);

                switch (modification.operationType)
                {
                    case OperationType.Union:
                        baseDensity = Mathf.Min(baseDensity, modificationDensity);
                        break;
                    case OperationType.Difference:
                        baseDensity = Mathf.Max(baseDensity, -modificationDensity);
                        break;
                    case OperationType.Intersection:
                        baseDensity = Mathf.Max(baseDensity, modificationDensity);
                        break;
                }
            }
            return baseDensity;
        }
        public virtual float GetDensity(Vector3 p) { return GetDensity(p.x, p.y, p.z); }
        public MeshBuilder GetMeshBuilder(Vector3 offset, int chunkSize)
        {
            switch (isosurfaceAlgorithm)
            {
                case IsosurfaceAlgorithm.Boxel:
                    return new Boxel(this, offset, chunkSize);
                case IsosurfaceAlgorithm.MarchingCubes:
                    return new MarchingCubes(this, offset, chunkSize);
                case IsosurfaceAlgorithm.MarchingTetrahedra:
                    return new MarchingTetrahedra(this, offset, chunkSize);
                case IsosurfaceAlgorithm.NaiveSurfaceNets:
                    return new NaiveSurfaceNets(this, offset, chunkSize);
                case IsosurfaceAlgorithm.DualContouring:
                    return new DualContouringUniform(this, offset, chunkSize);
                case IsosurfaceAlgorithm.CubicalMarchingSquares:
                    return new CubicalMarchingSquares(this, offset, chunkSize);
                default:
                    throw new NotImplementedException();
            }
        }
    }

    /// <summary>
    /// Available isosurfaces algorithms
    /// </summary>
    public enum IsosurfaceAlgorithm
    {
        Boxel,
        MarchingCubes,
        MarchingTetrahedra,
        NaiveSurfaceNets,
        DualContouring,
        CubicalMarchingSquares
    }
}
