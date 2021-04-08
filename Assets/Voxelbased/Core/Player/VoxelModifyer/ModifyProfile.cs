using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VoxelbasedCom
{
    public class ModifyProfile
    {
        public RaycastHit hit;
        public Vector3 hitPos;
        public string progressType;
        public Shape shapeType;
        public float shapeSize;
        public bool finish;

        public ModifyProfile(RaycastHit hit, Vector3 hitPos, string progressType, Shape shapeType, float shapeSize, bool finish)
        {
            this.hit = hit;
            this.hitPos = hitPos;
            this.progressType = progressType;
            this.shapeType = shapeType;
            this.shapeSize = shapeSize;
            this.finish = finish;
        }

    }
}