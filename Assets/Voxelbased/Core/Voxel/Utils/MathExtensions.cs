using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace VoxelbasedCom.Extensions
{
    public static class MathExtensions
    {
        public static float magnitude(this float3 i)
        {
            return math.sqrt(math.pow(i.x, 2) + math.pow(i.y, 2) + math.pow(i.z, 2));
        }
    }
}
