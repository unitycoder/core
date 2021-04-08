using UnityEngine;

namespace VoxelbasedCom
{
    public abstract class MeshBuilder
    {
        protected readonly Isosurface isosurface;
        protected readonly int chunkSize;

        protected readonly Vector3 offset;
        protected Triangle triangle = new Triangle();
        protected MeshBuilder(Isosurface isosurface, Vector3 offset, int chunkSize)
        {
            this.isosurface = isosurface;
            this.offset = offset;
            this.chunkSize = chunkSize;
        }

        public abstract MeshData GenerateMeshData();

        /// <summary>
        /// Get density for point in world
        /// </summary>
        protected float GetDensity(float x, float y, float z)
        {
            return isosurface.GetDensity(x, y, z);
        }

    }
}
