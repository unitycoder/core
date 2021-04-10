using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Profiling;
using Random = UnityEngine.Random;

namespace VoxelbasedCom
{
    public class Voxelbased: MonoBehaviour
    {
        public GameObject chunkPrefab;
        public int chunkSize = 16;
        public IsosurfaceAlgorithm isosurfaceAlgorithm;
        public Shape shape;
        public SimulationType simulation;
        public bool drawGizno;

        private int radius;
        private float centerPoint;
        public ShapeSelector shapeSelector;
        public Density density;
        private Queue<Chunk> chunksToGenerate;

        readonly Dictionary<IsosurfaceAlgorithm, int> borderSizes = new Dictionary<IsosurfaceAlgorithm, int>()
        {
            { IsosurfaceAlgorithm.MarchingCubes, 1 },
            { IsosurfaceAlgorithm.Boxel, 2 }
        };

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

                        //Profiler.BeginSample(string.Format("Chunk_{0}_{1}_{2}", x, y, z));
                        Vector3 chunkPosition = new Vector3(x, y, z);
                        chunkPosition *= chunkSize;
                        PlaceChunk(chunkPosition);
                        BuildChunkFromQueue();
                    }
                }
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
                DemoModification();
        }

        private void DemoModification()
        {
            float3 pos = new float3(Random.Range(0, chunkSize * 2), Random.Range(0, chunkSize * 2), Random.Range(0, chunkSize * 2));
            foreach (Transform chunk in transform)
            {
                
                chunk.GetComponent<Chunk>().ModifyChunk(shape, pos, centerPoint, OperationType.Union);
            }
            
        }

        private void PlaceChunk(Vector3 chunkPos)
        {
            GameObject chunkGameObject = Object.Instantiate(chunkPrefab);
            chunkGameObject.name = string.Format("Chunk_{0}_{1}_{2}", chunkPos.x, chunkPos.y, chunkPos.z);
            chunkGameObject.transform.SetParent(transform);
            chunkGameObject.transform.position = chunkPos;

            Chunk chunk = chunkGameObject.GetComponent<Chunk>();
            chunk.chunkSize = chunkSize;
            chunk.chunkPos = new int3(chunkPos);
            chunk.simType = simulation;
            chunksToGenerate.Enqueue(chunk);
        }

        private void BuildChunkFromQueue()
        {
            if (chunksToGenerate.Count > 0)
            {
                //var isosurface = new Isosurface(shapeSelector, density, isosurfaceAlgorithm, new Dictionary<Vector3, BaseModification>());

                Chunk chunk = chunksToGenerate.Dequeue();

                var isosurface = new Isosurface(shapeSelector, isosurfaceAlgorithm, new Dictionary<Vector3, BaseModification>(), true, chunk.chunkSize, chunk.chunkPos, new DensityProperties()
                {
                    borderSize = borderSizes[isosurfaceAlgorithm],
                    centerPoint = centerPoint,
                    shape = shape,
                    shapeRadius = centerPoint,
                    simulationType = simulation
                });
                
 
                chunk.CreateChunk(isosurface, chunkSize);
            }
        }

        void OnDrawGizmos()
        {
            if (drawGizno && transform.childCount > 0)
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
