using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace VoxelbasedCom
{
    public class Voxelbased: MonoBehaviour
    {
        public GameObject chunkPrefab;
        public int chunkSize = 16;
        public IsosurfaceAlgorithm isosurfaceAlgorithm;
        public Shape shape;
        public bool drawGizmo;

        private int radius;
        private float centerPoint;
        public ShapeSelector shapeSelector;
        public Density density;
        private Queue<Chunk> chunksToGenerate;

        void Awake()
        {
            radius = chunkSize * 2;
            centerPoint = radius / 2f;

            chunksToGenerate = new Queue<Chunk>();
            shapeSelector = new ShapeSelector();

            density = shapeSelector.GetShapeDensity(shape, transform.position + new Vector3(centerPoint, centerPoint, centerPoint), centerPoint);
        }

        void Start()
        {
            GenerateChunks();
        }

        public void GenerateChunks()
        {
            for (int x = 0; x < 2; x++)
            {
                for (int y = 0; y < 2; y++)
                {
                    for (int z = 0; z < 2; z++)
                    {
                        Vector3 chunkPosition = new Vector3(x, y, z);
                        chunkPosition *= chunkSize;
                        PlaceChunk(chunkPosition);
                        BuildChunkFromQueue();
                    }
                }
            }
        }

        private void PlaceChunk(Vector3 chunkPos)
        {
            GameObject chunkGameObject = Object.Instantiate(chunkPrefab);
            chunkGameObject.name = string.Format("Chunk_{0}_{1}_{2}", chunkPos.x, chunkPos.y, chunkPos.z);
            chunkGameObject.transform.SetParent(transform);
            chunkGameObject.transform.position = chunkPos;

            Chunk chunk = chunkGameObject.GetComponent<Chunk>();
            chunksToGenerate.Enqueue(chunk);
        }

        private void BuildChunkFromQueue()
        {
            if (chunksToGenerate.Count > 0)
            {
                var isosurface = new Isosurface(shapeSelector, density, isosurfaceAlgorithm, new Dictionary<Vector3, BaseModification>());
                Chunk chunk = chunksToGenerate.Dequeue();
                chunk.CreateChunk(isosurface, chunkSize);
            }
        }

        void OnDrawGizmos()
        {
            if (drawGizmo && transform.childCount > 0)
            {
                foreach (Transform child in transform)
                {
                    Gizmos.color = Color.green;

                    Vector3 chunkDrawSize = new Vector3(chunkSize, chunkSize, chunkSize);
                    Vector3 chunkDrawCenterPostion = new Vector3(centerPoint /2, centerPoint / 2, centerPoint / 2) + child.position;
                    Gizmos.DrawWireCube(chunkDrawCenterPostion, chunkDrawSize);
                }
            }
        }
    }
}
