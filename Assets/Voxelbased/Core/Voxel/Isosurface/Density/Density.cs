using UnityEngine;

namespace VoxelbasedCom
{
    public abstract class Density
    {
        public Operation operation = Operation.Union;

        public enum Operation
        {
            Union,
            Difference,
            Intersection
        }

        public abstract float GetDensity(float x, float y, float z);

        public float GetDensity(Vector3 pos)
        {
            return GetDensity(pos.x, pos.y, pos.z);
        }

        public float distanceFromConvexPlanes(float[,] planes, float[,] planeOffsets, float x, float y, float z)
        {
            var maxDistance = -Mathf.Infinity;
            for (var i = 0; i < planes.GetLength(0); i++)
            {
                var x_ = x - planeOffsets[i, 0];
                var y_ = y - planeOffsets[i, 1];
                var z_ = z - planeOffsets[i, 2];

                var dotProduct = planes[i, 0] * x_ + planes[i, 1] * y_ + planes[i, 2] * z_;

                maxDistance = Mathf.Max(maxDistance, dotProduct);
            }

            return maxDistance;
        }
    }
}
