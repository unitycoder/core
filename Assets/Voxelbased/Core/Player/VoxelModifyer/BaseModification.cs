using UnityEngine;

namespace VoxelbasedCom
{
    /// <summary>
    /// A base class for modifications
    /// </summary>
    public class BaseModification
    {
        public Vector3 position;
        public Density density;
        public OperationType operationType;
        public BaseModification(Vector3 position, Density density, OperationType operationType)
        {
            this.position = position;
            this.density = density;
            this.operationType = operationType;
        }
    }
    /// <summary>
    /// How to change the density function based on the BaseModification densities
    /// </summary>
    public enum OperationType 
    {
        Union, Subtraction, Intersection
    }
}