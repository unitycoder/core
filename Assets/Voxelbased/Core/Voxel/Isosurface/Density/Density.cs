using UnityEngine;

namespace VoxelbasedCom
{
    public abstract class Density
    {
        public OperationType operation = OperationType.Union;

        public abstract float GetDensity(float x, float y, float z);

        public float GetDensity(Vector3 pos)
        {
            return GetDensity(pos.x, pos.y, pos.z);
        }
    }

    /// <summary>
    /// How to change the density function based on the BaseModification densities
    /// </summary>
    public enum OperationType
    {
        Union,
        Difference,
        Intersection
    }
}
