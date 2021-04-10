using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;

namespace VoxelbasedCom.MarchingCubes
{
    public class MarchingCubes : MeshBuilder
    {
        private const float IsoLevel = 0; // The value that represents the surface of mesh
        private const float Target = 0; // The value that represents the surface of mesh
        private const float NormalSmoothing = 90; // set 90 if you want to shape to be smooter

        public MarchingCubes(Isosurface isosurface, Vector3 offset, int chunkSize) : base(isosurface, offset, chunkSize)
        {
            //INSTEAD SHOULD WAIT FOR NOSIE TO BE READY
            CompleteDensityJob();
            meshData = new MeshData()
            {
                vertices = new NativeArray<float3>(chunkSize * chunkSize * chunkSize * 5 * 3, Allocator.Persistent, NativeArrayOptions.UninitializedMemory),
                triangles = new NativeArray<int>(chunkSize * chunkSize * chunkSize * 5 * 3, Allocator.Persistent, NativeArrayOptions.UninitializedMemory),
                counter = new Counter(Allocator.Persistent)

            };


            ScheduleMeshJob();
        }

        protected override JobHandle StartMeshJob(JobHandle inputDeps = default)
        {
            

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

        
    }
}
