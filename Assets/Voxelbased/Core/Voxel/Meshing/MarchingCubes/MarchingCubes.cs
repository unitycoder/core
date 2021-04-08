using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;

namespace VoxelbasedCom.MarchingCubes
{
    public class MarchingCubes : MeshBuilder
    {
        private const float IsoLevel = 0; // The value that represents the surface of mesh
        private const float NormalSmoothing = 0; // set 90 if you want to shape to be smooter

        public MarchingCubes(Isosurface isosurface, Vector3 offset, int chunkSize) : base(isosurface, offset, chunkSize)
        {
            //INSTEAD SHOULD WAIT FOR NOSIE TO BE READY
            CompleteDensityJob();
            meshData = new MeshData()
            {
                vertices = new NativeArray<Vector3>(chunkSize * chunkSize * chunkSize * 5 * 3, Allocator.Persistent, NativeArrayOptions.UninitializedMemory),
                triangles = new NativeArray<int>(chunkSize * chunkSize * chunkSize * 5 * 3, Allocator.Persistent, NativeArrayOptions.UninitializedMemory),
                counter = new Counter(Allocator.Persistent)

            };


            ScheduleMeshJob();
        }

        protected override JobHandle OnMeshJobScheduled(JobHandle inputDeps = default)
        {
            if (meshData == null) return default;
            meshData.counter.Count = 0;

            var marchingCubesJob = new MarchingCubesJob()
            {
                chunkSize = chunkSize,
                densities = DensityField,
                isolevel = IsoLevel,
                counter = meshData.counter,
                indices = meshData.triangles,
                vertices = meshData.vertices
            };
            meshingHandle = marchingCubesJob.Schedule(chunkSize * chunkSize * chunkSize, 64, inputDeps);
            return meshingHandle;
        }

        public override bool GetMeshData(out MeshData meshData)
        {
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
    }
}
