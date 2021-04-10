using UnityEngine;

namespace VoxelbasedCom
{
    /// <summary>
    /// A base class for modifications
    /// </summary>
    public class BaseModification
    {
        public Vector3 position;
        public OperationType operationType;
        public Shape shapeType;
        public float shapeSize;
        public BaseModification(Vector3 position, OperationType operationType, Shape shapeType, float shapeSize)
        {
            this.position = position;
            this.operationType = operationType;
            this.shapeType = shapeType;
            this.shapeSize = shapeSize;
        }
    }
}
