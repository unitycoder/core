using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

namespace VoxelbasedCom
{
    public static class NativeExtensions
    {
        public static void Add<T>(this NativeArray<T> x, T t, Counter counter) where T : struct
        {
            x[counter.Increment()] = t;
        }
    }
}
