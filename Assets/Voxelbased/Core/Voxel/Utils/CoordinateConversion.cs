using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace VoxelbasedCom
{
    public static class CoordinateConversion
    {
        public static int Flatten(int3 pos, int size)
        {
            return Flatten(pos.x, pos.y, pos.z, size);
        }
        public static int Flatten(int x, int y, int z, int size)
        {
            return x + y * size + z * size * size;
        }

        public static int3 Expand(int index, int size)
        {
            return new int3(index / (size * size), index / size % size, index % size);
        }
    }
}
