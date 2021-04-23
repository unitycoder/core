using Unity.Mathematics;
using UnityEngine;

namespace VoxelbasedCom
{
    /// <summary>
    /// A single density class
    /// </summary>
    public interface IDensity
    {
        public float GetDensity(float3 pos);
    }

    /// <summary>
    /// How to change the density function based on the BaseModification densities
    /// </summary>
    public enum OperationType
    {
        Union,
        Difference,
        Intersection,
        Set
    }
}
