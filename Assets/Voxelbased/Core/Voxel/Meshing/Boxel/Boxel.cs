// Part of this code based on Sam Hogan tutorial
// His original repo: https://github.com/samhogan/Minecraft-Unity3D

using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace VoxelbasedCom.Boxel
{
	public class Boxel : MeshBuilder
	{
        List<Vector3> vertices;
        List<Vector3> normals;
        List<int> triangles;
        
		public Boxel(Isosurface isosurface, Vector3 offset, int chunkSize) : base(isosurface, offset, chunkSize)
		{
            //INSTEAD SHOULD WAIT FOR NOSIE TO BE READY
            CompleteDensityJob();
            meshData = new MeshData()
            {
                vertices = new NativeArray<float3>(chunkSize * chunkSize * chunkSize * 5 * 3, Allocator.Persistent, NativeArrayOptions.UninitializedMemory),
                triangles = new NativeArray<int>(chunkSize * chunkSize * chunkSize * 5 * 3, Allocator.Persistent, NativeArrayOptions.UninitializedMemory),
                normals = new NativeArray<float3>(chunkSize * chunkSize * chunkSize * 5 * 3, Allocator.Persistent, NativeArrayOptions.UninitializedMemory),
                counter = new Counter(Allocator.Persistent)

            };


            ScheduleMeshJob();
        }

        public override int GetTriangleMultiplier()
        {
            return 6;
        }

        protected override JobHandle StartMeshJob(JobHandle inputDeps = default)
        {
            var boxelJob = new BoxelJob()
            {
                chunkSize = chunkSize,
                counter = meshData.counter,
                densities = DensityField,
                isoLevel = 0f,
                normals = meshData.normals,
                triangles = meshData.triangles,
                vertices = meshData.vertices
            };
            meshingHandle = boxelJob.Schedule(chunkSize * chunkSize * chunkSize, 64, inputDeps);
            return meshingHandle;
        }
	}
}