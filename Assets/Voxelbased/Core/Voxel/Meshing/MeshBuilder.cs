using System;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace VoxelbasedCom
{
    public abstract class MeshBuilder : IDisposable
    {
        private readonly Isosurface isosurface;
        protected readonly int chunkSize;

        protected MeshData meshData;
        protected JobHandle meshingHandle;

        protected readonly Vector3 offset;
        protected Triangle triangle = new Triangle();
        protected MeshBuilder(Isosurface isosurface, Vector3 offset, int chunkSize)
        {
            this.isosurface = isosurface;
            this.offset = offset;
            this.chunkSize = chunkSize;

        }

        public abstract bool GetMeshData(out MeshData meshData);
        //change to abstract when all meshing options can support this
        public JobHandle ScheduleMeshJob(bool regenerateDensity = false)
        {
            return OnMeshJobScheduled(regenerateDensity ? ScheduleDensityJob() : default);
        }
        public JobHandle ScheduleMeshJob(JobHandle inputDeps)
        {
            return OnMeshJobScheduled(inputDeps);
        }

        protected abstract JobHandle OnMeshJobScheduled(JobHandle inputDeps = default);

        /// <summary>
        /// Get density for point in world
        /// </summary>
        protected float GetDensity(float x, float y, float z)
        {
            return isosurface.GetDensity(x, y, z, chunkSize);
        }

        public void Dispose()
        {
            meshingHandle.Complete();
            meshData.Dispose();
        }

        protected NativeArray<float> DensityField
        {
            get
            {
                return isosurface.densityField;
            }
        }
        //TEMP, dont do this, instead wait for noise to be ready.
        protected void CompleteDensityJob()
        {
            isosurface.densityHandle.Complete();
        }
        protected JobHandle ScheduleDensityJob()
        {
            return isosurface.ScheduleDensityUpdate(chunkSize, new int3(offset));
        }
    }
}
