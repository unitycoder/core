using UnityEngine;

namespace VoxelbasedCom
{
    public class ModifyVertex
    {
        public Vector3 position;
        public string processType;
        public Shape shapeType;
        public float shapeSize;
        public ModifyVertex(Vector3 position, string processType, Shape shapeType, float shapeSize)
        {
            this.position = position;
            this.processType = processType;
            this.shapeType = shapeType;
            this.shapeSize = shapeSize;
        }
    }
}