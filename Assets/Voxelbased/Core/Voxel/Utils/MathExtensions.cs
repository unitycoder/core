using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace VoxelbasedCom.Extensions
{
    public static class MathExtensions
    {
        public static float magnitude(this float3 x)
        {
            return math.sqrt(math.pow(x.x, 2) + math.pow(x.y, 2) + math.pow(x.z, 2));
        }


        
    }

    public static class Float3
    {
        public static float3 up
        {
            get
            {
                return new float3(0, 1, 0);
            }
        }
        public static float3 down
        {
            get
            {
                return new float3(0, -1, 0);
            }
        }
        public static float3 right
        {
            get
            {
                return new float3(1, 0, 0);
            }
        }
        public static float3 left
        {
            get
            {
                return new float3(-1, 0, 0);
            }
        }
        public static float3 forward
        {
            get
            {
                return new float3(0, 0, 1);
            }
        }
        public static float3 back
        {
            get
            {
                return new float3(0, 0, -1);
            }
        }
    }
}
