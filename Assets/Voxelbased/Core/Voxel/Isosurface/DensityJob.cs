using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Burst;
using VoxelbasedCom.Extensions;
using System.Runtime.CompilerServices;

namespace VoxelbasedCom
{
    public enum SimulationType { None, Expand, Sin }

    [BurstCompile]
    public struct DensityJob : IJobParallelFor
    {
        //Chunk variables
        [ReadOnly] public float3 chunkOffset;
        [ReadOnly] public int chunkSize;
        
        
        //Simulation variables
        [ReadOnly] public float time;
        [ReadOnly] public SimulationType simType;

        //Shape variables
        [ReadOnly] public Shape shape;
        [ReadOnly] public float3 shapeCenter;
        [ReadOnly] public float shapeRadius;

        //Density variables
        [NativeDisableParallelForRestriction] public NativeArray<float> densityField;
        [ReadOnly] public OperationType operationType;

        public void Execute(int index)
        {
            float3 localPos = CoordinateConversion.Expand(index, chunkSize);

            float value = DemoShapes.GetDensity(shape, localPos + chunkOffset, shapeRadius, shapeCenter);

            if(simType == SimulationType.Expand)
                value += math.sin(time);
            else if(simType == SimulationType.Sin)
                value += math.sin(time + localPos.x + chunkOffset.x) * 0.05f;

            switch (operationType)
            {
                case OperationType.Union:
                    densityField[index] = Mathf.Min(densityField[index], value);
                    break;
                case OperationType.Difference:
                    densityField[index] = Mathf.Max(densityField[index], -value);
                    break;
                case OperationType.Intersection:
                    densityField[index] = Mathf.Max(densityField[index], value);
                    break;
            }

            densityField[index] = math.clamp(densityField[index], -1f, 1f);
        }
    }

    public static class DemoShapes
    {
        public static float GetDensity(Shape shape, float3 pos, float shapeRadius, float3 shapeCenter)
        {
            float value = 0f;
            if (shape == Shape.Capsule)
                value = DemoShapes.GetDensity_Capsule(pos, shapeRadius, shapeCenter);
            else if (shape == Shape.Cube)
                value = DemoShapes.GetDensity_Cube(pos, shapeRadius, shapeCenter);
            else if (shape == Shape.GoursatsSurface)
                value = DemoShapes.GetDensity_GoursatsSurface(pos, shapeRadius, shapeCenter);
            else if (shape == Shape.Heart)
                value = DemoShapes.GetDensity_Heart(pos, shapeRadius, shapeCenter);
            else if (shape == Shape.Plane)
                value = DemoShapes.GetDensity_Plane(pos, shapeRadius, shapeCenter);
            else if (shape == Shape.Rubin)
                value = DemoShapes.GetDensity_Rubin(pos, shapeRadius, shapeCenter);
            else if (shape == Shape.Sphere)
                value = DemoShapes.GetDensity_Sphere(pos, shapeRadius, shapeCenter);
            else if (shape == Shape.Torus)
                value = DemoShapes.GetDensity_Torus(pos, shapeRadius, shapeCenter);

            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static float GetDensity_Capsule(float3 pos, float shapeRadius, float3 shapeCenter)
        {
            pos.x -= shapeCenter.x;
            pos.y -= shapeCenter.y;
            pos.z -= shapeCenter.z;

            pos.y = pos.y / (shapeRadius - 6.0f);
            pos.x = pos.x / (shapeRadius - 6.0f);
            pos.z = pos.z / (shapeRadius - 6.0f);

            float3 p1 = float3.zero + new float3(0, 1, 0) * (0.4f - 1.0f);
            float3 p2 = float3.zero - new float3(0, 1, 0) * (0.4f - 1.0f);
            float t = math.dot((pos - p1), math.normalize(p2 - p1));
            t = math.clamp(t, 0f, 1f);
            float3 closestPoint = p1 + math.normalize(p2 - p1) * t;
            return (pos - closestPoint).magnitude() - 1.0f;

        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static float GetDensity_Sphere(float3 pos, float shapeRadius, float3 shapeCenter)
        {
            pos.x -= shapeCenter.x;
            pos.y -= shapeCenter.y;
            pos.z -= shapeCenter.z;

            float sqr_dist = math.pow(pos.x, 2) + math.pow(pos.y, 2) + math.pow(pos.z, 2);
            float sqr_rad = math.pow(shapeRadius, 2);
            float d = sqr_dist - sqr_rad;
            return d;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static float GetDensity_Cube(float3 pos, float shapeRadius, float3 shapeCenter)
        {
            float xt = pos.x - shapeCenter.x;
            float yt = pos.y - shapeCenter.y;
            float zt = pos.z - shapeCenter.z;

            float xd = (xt * xt) - shapeRadius * shapeRadius;
            float yd = (yt * yt) - shapeRadius * shapeRadius;
            float zd = (zt * zt) - shapeRadius * shapeRadius;
            float d;

            if (xd > yd)
                if (xd > zd)
                    d = xd;
                else
                    d = zd;
            else if (yd > zd)
                d = yd;
            else
                d = zd;

            return d;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static float GetDensity_Torus(float3 pos, float shapeRadius, float3 shapeCenter)
        {
            pos.x -= shapeCenter.x;
            pos.y -= shapeCenter.y;
            pos.z -= shapeCenter.z;

            float sqr_dist = math.pow(shapeRadius - math.sqrt(math.pow(pos.x, 2.0f) + math.pow(pos.y, 2.0f)), 2) + math.pow(pos.z, 2.0f) - 2.0f * shapeRadius;

            return sqr_dist;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static float GetDensity_Heart(float3 pos, float shapeRadius, float3 shapeCenter)
        {
            pos.x -= shapeCenter.x;
            pos.y -= shapeCenter.y;
            pos.z -= shapeCenter.z;

            pos.y = pos.y / (shapeRadius - 0.0f);
            pos.x = pos.x / (shapeRadius - 0.0f);
            pos.z = pos.z / (shapeRadius - 0.0f);

            pos.y *= 1.4f;
            pos.z *= 1.4f;
            return math.pow(2f * pos.x * pos.x + pos.y * pos.y + 2f * pos.z * pos.z - 1, 3) - 0.1f * pos.z * pos.z * pos.y * pos.y * pos.y - pos.y * pos.y * pos.y * pos.x * pos.x;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static float GetDensity_Rubin(float3 pos, float shapeRadius, float3 shapeCenter)
        {
            pos.x -= shapeCenter.x;
            pos.y -= shapeCenter.y;
            pos.z -= shapeCenter.z;

            pos.y /= shapeRadius - 0.5f;
            pos.x /= shapeRadius - 0.5f;
            pos.z /= shapeRadius - 0.5f;

            return math.abs(pos.x) + math.abs(pos.y) + math.abs(pos.z) - 1.0f;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static float GetDensity_GoursatsSurface(float3 pos, float shapeRadius, float3 shapeCenter)
        {
            pos.x -= shapeCenter.x;
            pos.y -= shapeCenter.y;
            pos.z -= shapeCenter.z;

            pos.y /= shapeRadius - 4.5f;
            pos.x /= shapeRadius - 4.5f;
            pos.z /= shapeRadius - 4.5f;

            return math.pow(pos.x, 4) + math.pow(pos.y, 4) + math.pow(pos.z, 4) - 1.5f * (pos.x * pos.x + pos.y * pos.y + pos.z * pos.z) + 1.0f;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static float GetDensity_Plane(float3 pos, float shapeRadius, float3 shapeCenter)
        {
            float height = shapeCenter.y / 2f;
            pos.y -= pos.y / height;

            return height;
        }
    }
}
