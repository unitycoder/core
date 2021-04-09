using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace VoxelbasedCom
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshCollider))]
    public class Chunk : MonoBehaviour
    {
        private MeshFilter meshFilter;
        private MeshRenderer meshRenderer;
        private MeshCollider meshCollider;

        private Isosurface isosurface;
        private MeshBuilder meshBuilder;
      

        private void OnEnable()
        {
            meshFilter = GetComponent<MeshFilter>();
            meshRenderer = GetComponent<MeshRenderer>();
            meshCollider = GetComponent<MeshCollider>();
        }

        /// <summary>
        /// Init chunk and start mesh generation
        /// </summary>
        /// <param name="isosurface"></param>
        /// <param name="chunkSize"></param>
        public void CreateChunk(Isosurface isosurface, int chunkSize)
        {
            this.isosurface = isosurface;
            meshBuilder = isosurface.GetMeshBuilder(transform.position, chunkSize);

            BuildChunk();
        }

        public void BuildChunk()
        {
            //chunkState = ChunkState.Waiting;

            MeshData meshData = meshBuilder.GenerateMeshData();
            GenerateMesh(meshData);
        }

        /// <summary>
        /// Generate mesh from mesh data
        /// </summary>
        /// <param name="data">Contains data to build mesh</param>
        public void GenerateMesh(MeshData data)
        {
            if (data.vertices.Count == 0)
            {
                return;
            }

            Mesh mesh = new Mesh(); 

            mesh.SetVertices(data.vertices);
            mesh.SetTriangles(data.triangles, 0);
            mesh.uv = new Vector2[data.vertices.Count];
            mesh.SetNormals(data.normals);

            meshFilter.mesh = mesh;
            meshCollider.sharedMesh = mesh;
        }
    }
}
