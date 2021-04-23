using UnityEngine;

namespace VoxelbasedCom
{
    public abstract class Density
    {
        public OperationType operation = OperationType.Union;
        public Vector3 Abs(Vector3 p)
        {
            return new Vector3(Mathf.Abs(p.x), Mathf.Abs(p.y), Mathf.Abs(p.z));
        }
        public Vector3 Max(Vector3 p, float v)
        {
            return new Vector3(Mathf.Max(p.x, v), Mathf.Max(p.y, v), Mathf.Max(p.z, v));
        }
        public Vector3 Min(Vector3 p, float v)
        {
            return new Vector3(Mathf.Min(p.x, v), Mathf.Min(p.y, v), Mathf.Min(p.z, v));
        }
        public Vector2 Abs(Vector2 p)
        {
            return new Vector2(Mathf.Abs(p.x), Mathf.Abs(p.y));
        }
        public Vector2 Max(Vector2 p, float v)
        {
            return new Vector2(Mathf.Max(p.x, v), Mathf.Max(p.y, v));
        }
        public Vector2 Min(Vector2 p, float v)
        {
            return new Vector2(Mathf.Min(p.x, v), Mathf.Min(p.y, v));
        }

        public float PerlinNoise3D(float x, float y, float z)
        {
            float xy = Mathf.PerlinNoise(x, y);
            float xz = Mathf.PerlinNoise(x, z);
            float yz = Mathf.PerlinNoise(y, z);
            float yx = Mathf.PerlinNoise(y, x);
            float zx = Mathf.PerlinNoise(z, x);
            float zy = Mathf.PerlinNoise(z, y);

            return (xy + xz + yz + yx + zx + zy) / 6;
        }

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
        Intersection,
        Set
    }
}
