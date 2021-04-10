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

        //How many indices there is in relation to the counters value. If you add one triangle per counter increment it's 3. if you add two triangles per counter increment, it's 6.
        public virtual int GetTriangleMultiplier()
        {
            return 3;
        }

        protected MeshBuilder(Isosurface isosurface, Vector3 offset, int chunkSize)
        {
            this.isosurface = isosurface;
            this.offset = offset;
            this.chunkSize = chunkSize;

        }

        //public abstract bool GetMeshData(out MeshData meshData);
        //change to abstract when all meshing options can support this
        public JobHandle ScheduleMeshJob(bool regenerateDensity = false)
        {
            if (meshData == null) return default;
            meshData.counter.Count = 0;

            return StartMeshJob(regenerateDensity ? ScheduleDensityJob() : default);
        }
        public JobHandle ScheduleMeshJob(JobHandle inputDeps)
        {
            if (meshData == null) return default;
            meshData.counter.Count = 0;

            return StartMeshJob(inputDeps);
        }

        protected abstract JobHandle StartMeshJob(JobHandle inputDeps = default);

        public bool TryGetMeshData(out MeshData meshData)
        {
            if (this.meshData == null) throw new Exception("MeshData not initialized! Should Initialize in the constructor of MeshBuilder derived class");
            if (meshingHandle.IsCompleted)
            {
                meshingHandle.Complete();
                meshData = this.meshData;
                //Temporary, should calculate normals in a job
                //meshData.normals = NormalSolver.RecalculateNormals(meshData.triangles.ToArray(), meshData.vertices.ToArray(), NormalSmoothing, meshData.counter.Count);
                return true;
            }
            else
            {
                meshData = null;
                return false;
            }
        }

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
