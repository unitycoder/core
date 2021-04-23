using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using VoxelbasedCom.Extensions;

namespace VoxelbasedCom.Boxel
{
    public struct BoxelJob : IJobParallelFor
    {
        [ReadOnly] public int chunkSize;
        [ReadOnly] public float isoLevel;

        [WriteOnly, NativeDisableParallelForRestriction] public NativeArray<float3> vertices;
        [WriteOnly, NativeDisableParallelForRestriction] public NativeArray<float3> normals;
        [WriteOnly, NativeDisableParallelForRestriction] public NativeArray<int> triangles;

        [ReadOnly, NativeDisableParallelForRestriction] public NativeArray<float> densities;

        public Counter counter;

        float GetDensity(float3 pos)
        {
            int borderedChunkSize = chunkSize + 2;

            return densities[CoordinateConversion.Flatten((int3)pos, borderedChunkSize)];
        }

        public void Execute(int index)
        {
            float3 pos = CoordinateConversion.Expand(index, chunkSize);

            float currentBlockDensity = GetDensity(pos + new float3(1, 1, 1));

            float nextxBlockDensity = GetDensity(pos + new float3(1, 1, 1) + Float3.right);
            float nextyBlockDensity = GetDensity(pos + new float3(1, 1, 1) + Float3.up);
            float nextzBlockDensity = GetDensity(pos + new float3(1, 1, 1) + Float3.forward);

            float prevxBlockDensity = GetDensity(pos + new float3(1, 1, 1) + Float3.left);
            float prevyBlockDensity = GetDensity(pos + new float3(1, 1, 1) + Float3.down);
            float prevzBlockDensity = GetDensity(pos + new float3(1, 1, 1) + Float3.back);
            
            if (currentBlockDensity <= isoLevel)
            {
                //X right
                if (nextxBlockDensity > isoLevel)
                {
                    int triangleIndex = counter.Increment();

                    vertices[triangleIndex * 4 + 0] = pos + new float3(1, 0, 0);
                    vertices[triangleIndex * 4 + 1] = pos + new float3(1, 1, 0);
                    vertices[triangleIndex * 4 + 2] = pos + new float3(1, 1, 1);
                    vertices[triangleIndex * 4 + 3] = pos + new float3(1, 0, 1);

                    normals[triangleIndex * 4 + 0] = Float3.right;
                    normals[triangleIndex * 4 + 1] = Float3.right;
                    normals[triangleIndex * 4 + 2] = Float3.right;
                    normals[triangleIndex * 4 + 3] = Float3.right;

                    triangles[triangleIndex * 6 + 0] = triangleIndex * 4 + 0;
                    triangles[triangleIndex * 6 + 1] = triangleIndex * 4 + 1;
                    triangles[triangleIndex * 6 + 2] = triangleIndex * 4 + 2;
                    triangles[triangleIndex * 6 + 3] = triangleIndex * 4 + 0;
                    triangles[triangleIndex * 6 + 4] = triangleIndex * 4 + 2;
                    triangles[triangleIndex * 6 + 5] = triangleIndex * 4 + 3;
                }

                //Y top
                if (nextyBlockDensity > isoLevel)
                {
                    int triangleIndex = counter.Increment();

                    vertices[triangleIndex * 4 + 0] = pos + new float3(0, 1, 0);
                    vertices[triangleIndex * 4 + 1] = pos + new float3(0, 1, 1);
                    vertices[triangleIndex * 4 + 2] = pos + new float3(1, 1, 1);
                    vertices[triangleIndex * 4 + 3] = pos + new float3(1, 1, 0);

                    normals[triangleIndex * 4 + 0] = Float3.up;
                    normals[triangleIndex * 4 + 1] = Float3.up;
                    normals[triangleIndex * 4 + 2] = Float3.up;
                    normals[triangleIndex * 4 + 3] = Float3.up;

                    triangles[triangleIndex * 6 + 0] = triangleIndex * 4 + 0;
                    triangles[triangleIndex * 6 + 1] = triangleIndex * 4 + 1;
                    triangles[triangleIndex * 6 + 2] = triangleIndex * 4 + 2;
                    triangles[triangleIndex * 6 + 3] = triangleIndex * 4 + 0;
                    triangles[triangleIndex * 6 + 4] = triangleIndex * 4 + 2;
                    triangles[triangleIndex * 6 + 5] = triangleIndex * 4 + 3;
                }

                //Z back
                if (nextzBlockDensity > isoLevel)
                {
                    int triangleIndex = counter.Increment();

                    vertices[triangleIndex * 4 + 0] = pos + new float3(1, 0, 1);
                    vertices[triangleIndex * 4 + 1] = pos + new float3(1, 1, 1);
                    vertices[triangleIndex * 4 + 2] = pos + new float3(0, 1, 1);
                    vertices[triangleIndex * 4 + 3] = pos + new float3(0, 0, 1);

                    normals[triangleIndex * 4 + 0] = Float3.forward;
                    normals[triangleIndex * 4 + 1] = Float3.forward;
                    normals[triangleIndex * 4 + 2] = Float3.forward;
                    normals[triangleIndex * 4 + 3] = Float3.forward;

                    triangles[triangleIndex * 6 + 0] = triangleIndex * 4 + 0;
                    triangles[triangleIndex * 6 + 1] = triangleIndex * 4 + 1;
                    triangles[triangleIndex * 6 + 2] = triangleIndex * 4 + 2;
                    triangles[triangleIndex * 6 + 3] = triangleIndex * 4 + 0;
                    triangles[triangleIndex * 6 + 4] = triangleIndex * 4 + 2;
                    triangles[triangleIndex * 6 + 5] = triangleIndex * 4 + 3;
                }

                //X left
                if (prevxBlockDensity > isoLevel)
                {
                    int triangleIndex = counter.Increment();

                    vertices[triangleIndex * 4 + 0] = pos + new float3(0, 0, 1);
                    vertices[triangleIndex * 4 + 1] = pos + new float3(0, 1, 1);
                    vertices[triangleIndex * 4 + 2] = pos + new float3(0, 1, 0);
                    vertices[triangleIndex * 4 + 3] = pos + new float3(0, 0, 0);

                    normals[triangleIndex * 4 + 0] = Float3.left;
                    normals[triangleIndex * 4 + 1] = Float3.left;
                    normals[triangleIndex * 4 + 2] = Float3.left;
                    normals[triangleIndex * 4 + 3] = Float3.left;

                    triangles[triangleIndex * 6 + 0] = triangleIndex * 4 + 0;
                    triangles[triangleIndex * 6 + 1] = triangleIndex * 4 + 1;
                    triangles[triangleIndex * 6 + 2] = triangleIndex * 4 + 2;
                    triangles[triangleIndex * 6 + 3] = triangleIndex * 4 + 0;
                    triangles[triangleIndex * 6 + 4] = triangleIndex * 4 + 2;
                    triangles[triangleIndex * 6 + 5] = triangleIndex * 4 + 3;
                }

                //Y bottom
                if (prevyBlockDensity > isoLevel)
                {
                    int triangleIndex = counter.Increment();

                    vertices[triangleIndex * 4 + 0] = pos + new float3(0, 0, 0);
                    vertices[triangleIndex * 4 + 1] = pos + new float3(1, 0, 0);
                    vertices[triangleIndex * 4 + 2] = pos + new float3(1, 0, 1);
                    vertices[triangleIndex * 4 + 3] = pos + new float3(0, 0, 1);

                    normals[triangleIndex * 4 + 0] = Float3.down;
                    normals[triangleIndex * 4 + 1] = Float3.down;
                    normals[triangleIndex * 4 + 2] = Float3.down;
                    normals[triangleIndex * 4 + 3] = Float3.down;

                    triangles[triangleIndex * 6 + 0] = triangleIndex * 4 + 0;
                    triangles[triangleIndex * 6 + 1] = triangleIndex * 4 + 1;
                    triangles[triangleIndex * 6 + 2] = triangleIndex * 4 + 2;
                    triangles[triangleIndex * 6 + 3] = triangleIndex * 4 + 0;
                    triangles[triangleIndex * 6 + 4] = triangleIndex * 4 + 2;
                    triangles[triangleIndex * 6 + 5] = triangleIndex * 4 + 3;
                }

                //Z front
                if (prevzBlockDensity > isoLevel)
                {
                    int triangleIndex = counter.Increment();

                    vertices[triangleIndex * 4 + 0] = pos + new float3(0, 0, 0);
                    vertices[triangleIndex * 4 + 1] = pos + new float3(0, 1, 0);
                    vertices[triangleIndex * 4 + 2] = pos + new float3(1, 1, 0);
                    vertices[triangleIndex * 4 + 3] = pos + new float3(1, 0, 0);

                    normals[triangleIndex * 4 + 0] = Float3.back;
                    normals[triangleIndex * 4 + 1] = Float3.back;
                    normals[triangleIndex * 4 + 2] = Float3.back;
                    normals[triangleIndex * 4 + 3] = Float3.back;

                    triangles[triangleIndex * 6 + 0] = triangleIndex * 4 + 0;
                    triangles[triangleIndex * 6 + 1] = triangleIndex * 4 + 1;
                    triangles[triangleIndex * 6 + 2] = triangleIndex * 4 + 2;
                    triangles[triangleIndex * 6 + 3] = triangleIndex * 4 + 0;
                    triangles[triangleIndex * 6 + 4] = triangleIndex * 4 + 2;
                    triangles[triangleIndex * 6 + 5] = triangleIndex * 4 + 3;
                }
            }
        }
    }
}
