using System;
using UnityEngine;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace VoxelbasedCom
{
    public class Isosurface : IDisposable
    {
        public IsosurfaceAlgorithm isosurfaceAlgorithm;
        private Dictionary<Vector3, ModifyVertex> modifyVertex;

        public JobHandle densityHandle;
        public NativeArray<float> densityField;

        DensityProperties densityProperties;

        public Isosurface(IsosurfaceAlgorithm algorithm, Dictionary<Vector3, ModifyVertex> modifyVertex, bool preGenerate, int chunkSize, int3 chunkPos, DensityProperties densityProperties)
        {
            this.isosurfaceAlgorithm = algorithm;
            this.modifyVertex = modifyVertex;
            this.densityProperties = densityProperties;

            if (preGenerate)
            {
                int chunkSizeWithBorder = chunkSize + densityProperties.borderSize;
                densityField = new NativeArray<float>(chunkSizeWithBorder * chunkSizeWithBorder * chunkSizeWithBorder, Allocator.Persistent);

                var densityJob = new DensityJob()
                {
                    chunkOffset = chunkPos,
                    chunkSize = chunkSizeWithBorder,
                    densityField = densityField,
                    shape = densityProperties.shape,
                    shapeCenter = densityProperties.centerPoint,
                    shapeRadius = densityProperties.shapeRadius,
                };
                densityHandle = densityJob.Schedule(chunkSizeWithBorder * chunkSizeWithBorder * chunkSizeWithBorder, 64);
            }
        }

        public JobHandle ScheduleDensityModification(Shape shape, float3 modificationCenter, float modificationRadius, DensityOperationType operationType, int chunkSize, int3 chunkPos)
        {
            if (!densityField.IsCreated) return default;

            int chunkSizeWithBorder = chunkSize + densityProperties.borderSize;

            var densityJob = new DensityJob()
            {
                chunkOffset = chunkPos,
                chunkSize = chunkSizeWithBorder,
                densityField = densityField,
                shape = shape,
                shapeCenter = modificationCenter,
                shapeRadius = modificationRadius,
                operationType = operationType
            };
            densityHandle = densityJob.Schedule(chunkSizeWithBorder * chunkSizeWithBorder * chunkSizeWithBorder, 64);
            return densityHandle;
        }

        public JobHandle ScheduleDensityUpdate(int chunkSize, int3 chunkPos)
        {
            if (!densityField.IsCreated) return default;

            int chunkSizeWithBorder = chunkSize + densityProperties.borderSize;

            var densityJob = new DensityJob()
            {
                chunkOffset = chunkPos,
                chunkSize = chunkSizeWithBorder,
                densityField = densityField,
                shape = densityProperties.shape,
                shapeCenter = densityProperties.centerPoint,
                shapeRadius = densityProperties.shapeRadius,
                time = Time.time * 2,
                simType = densityProperties.simulationType
            };
            densityHandle = densityJob.Schedule(chunkSizeWithBorder * chunkSizeWithBorder * chunkSizeWithBorder, 64);
            return densityHandle;
        }

        public virtual float GetDensity(float x, float y, float z, int chunkSize)
        {
            float baseDensity = densityField[CoordinateConversion.Flatten((int)x, (int)y, (int)z, chunkSize)];

            foreach (ModifyVertex item in modifyVertex.Values)
            {
                //Density d = shapeSelector.GetShapeDensity(item.shapeType, item.position, item.shapeSize);

                //float shapeDensity = d.GetDensity(x, y, z);

               /*if (item.processType == "Add")
                {
                    if (shapeDensity < baseDensity)
                        baseDensity = shapeDensity;
                }
                else if(item.processType == "Remove")
                {
                    if (-shapeDensity > baseDensity)
                        baseDensity = -shapeDensity;
                }*/

            }
            return baseDensity;
        }

        public MeshBuilder GetMeshBuilder(Vector3 offset, int chunkSize)
        {
            switch (isosurfaceAlgorithm)
            {
                case IsosurfaceAlgorithm.Boxel:
                    return new Boxel(this, offset, chunkSize);
                case IsosurfaceAlgorithm.MarchingCubes:
                    return new MarchingCubes.MarchingCubes(this, offset, chunkSize);
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

        public void Dispose()
        {
            densityHandle.Complete();
            densityField.Dispose();
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
    //Temp
    public class DensityProperties
    {
        public int borderSize;
        public Shape shape;
        public float3 centerPoint;
        public float shapeRadius;
        public SimulationType simulationType;
    }
}