using System;
using System.Collections; 
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using System.Collections.Concurrent;

public class Octree : MonoBehaviour
{
    [SerializeField]
    private bool drawGizmo;
    [SerializeField]
    private bool VisualDebugger;
    [SerializeField]
    private GameObject player;
    private float3 playerCurrentPosition;
    [SerializeField]
    private int minLodLevel = 2;
    [SerializeField]
    private int maxLodLevel = 5;
    [SerializeField]
    private float radius = 1000;
    private bool isChecking;
    private ConcurrentDictionary<long,OctreeNode> nodes;

    /* ObjectPool */
    [SerializeField]
    private GameObject poolPrefab;
    private int poolSize = 5000;
    private Queue<GameObject> poolQueue = new Queue<GameObject>();

    private static readonly int3[] voxelBasePosition = new int3[8]{
        new int3(0, 0, 0),
        new int3(0, 0, 1),
        new int3(1, 0, 0),
        new int3(1, 0, 1),
        new int3(0, 1, 0),
        new int3(0, 1, 1),
        new int3(1, 1, 0),
        new int3(1, 1, 1)
    };

    private static readonly int3[] deltaSigns = new int3[8]{
        new int3(-1, -1, -1),
        new int3(-1, -1, 1),
        new int3(1, -1, -1),
        new int3(1, -1, 1),
        new int3(-1, 1, -1),
        new int3(-1, 1, 1),
        new int3(1, 1, -1),
        new int3(1, 1, 1)
    };

    void Start()
    {
        nodes = new ConcurrentDictionary<long, OctreeNode>();
        // poolSize = CalculateObjectPoolSize(maxLodLevel);
        InitGameObjectPool();
        InitOctree();
    }

    void LateUpdate()
    {
        playerCurrentPosition = new float3(player.transform.position.x, player.transform.position.y, player.transform.position.z);

        if (!isChecking) 
        {
            DistanceChecker();
        }
    }

    private void InitOctree()
    {
        // Init first Node data (Level 0)
        float3 startPosition = new float3(transform.position.x, transform.position.y, transform.position.z);
        float3 position = startPosition + new float3(radius / 2, radius / 2, radius / 2);
        float3 voxelPosition = startPosition;

        // Init other nodes of min lod level
        InitMinLodLevel(position, voxelPosition, radius, 0);
    }

    private OctreeNode InitMinLodLevel(float3 position, float3 voxelPosition, float size, byte lodLevel)
    {
        // Create Node
        long id = GenerateUniqueId(position);
        OctreeNode node = new OctreeNode(id, position, voxelPosition, size, lodLevel, false);

        // After check the level, and create children nodes if necessary
        if (lodLevel < minLodLevel)
        {
            node.hasChildren = true;

            float delta = size / 4;

            for (int i = 0; i < 8; i++)
            {
                float3 childPosition = GetPosition(i, position, delta);
                float3 childVoxelPosition = voxelPosition + (new float3(voxelBasePosition[i].x, voxelBasePosition[i].y, voxelBasePosition[i].z) * size / 2);
                var child = InitMinLodLevel(childPosition, childVoxelPosition, size / 2, (byte)(lodLevel + 1));
                node.children[i] = child.guid;
            }
        }

        nodes.TryAdd(node.guid, node);
        AddNodeToGameObject(node);
        return node;
    }

    private void InitGameObjectPool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(poolPrefab);
            obj.SetActive(false);
            obj.name = "Chunk";
            obj.transform.parent = transform;
            // obj.hideFlags = HideFlags.HideInHierarchy;
            poolQueue.Enqueue(obj);
        }
    }

    private int CalculateObjectPoolSize(int maxodLevel)
    {
        int nodesNumber = 0;
        for (int i = 0; i <= maxodLevel; i++)
        {
            nodesNumber += (int)Mathf.Pow(8, i);
        }

        return nodesNumber;
    }

    private long GenerateUniqueId(float3 p)
    {
        long n = (nodes.Count + 1) + long.Parse(Mathf.FloorToInt(p.x) + "" + Mathf.FloorToInt(p.y) + "" + Mathf.FloorToInt(p.z)) + UnityEngine.Random.Range(0,1000000);
        return n;
    }

    private void DistanceChecker()
    {
        isChecking = true;

        List<OctreeNode> newNodeList = new List<OctreeNode>(nodes.Values);

        foreach (var node in newNodeList)
        {
            if (!nodes.ContainsKey(node.guid))
            {
                continue;
            }

            float diagonal = math.sqrt(2 * (node.size * node.size));

            if (!node.hasChildren && node.lodLevel < maxLodLevel && math.length(playerCurrentPosition - node.position) < diagonal)
            {
                var n = CreateChildren(node);
                n.hasChildren = true;
                nodes[node.guid] = n;
                
            }
            else if (node.hasChildren && node.lodLevel >= minLodLevel && math.length(playerCurrentPosition - node.position) > (diagonal * 2.0f))
            {
                RemoveChildren(node);
                var n = nodes[node.guid];
                n.hasChildren = false;
                nodes[node.guid] = n;
            }
        }

        isChecking = false;
    }

    public OctreeNode CreateChildren(OctreeNode node)
    {
        float delta = node.size / 4;

        for (int i = 0; i < 8; i++)
        {
            float3 voxelPosition = node.voxelPosition + (new float3(voxelBasePosition[i].x, voxelBasePosition[i].y, voxelBasePosition[i].z) * node.size / 2);
            float3 position = GetPosition(i, node.position, delta);
            long id = GenerateUniqueId(position);
            var n = CreateChild(position, voxelPosition, (byte)(node.lodLevel + 1), node.size / 2, id);
            node.children[i] = id;
        }

        return node;
    }

    private OctreeNode? CreateChild(float3 position, float3 voxelPosition, byte lodLevel, float size, long id)
    {
        OctreeNode node = new OctreeNode(id, position, voxelPosition, size, lodLevel, false);
            if (nodes.TryAdd(id, node))
            {
                AddNodeToGameObject(node);
                return node;
            }
            return null;
    }

    private void RemoveChildren(OctreeNode node)
    {
        for (int i = 0; i < 8; i++)
        {
            //if (node.children == null) {
            //    Debug.Log("igaz");
            //    continue; }

            OctreeNode child = nodes[node.children[i]];

            if (child.hasChildren)
            {
                RemoveChildren(child);
            }

            if (nodes.TryRemove(node.children[i], out OctreeNode oc))
            {
                RemoveNodeFromGameObject(node.children[i]);
            }
        }
    }

    float3 GetPosition(int index, float3 parentPosition, float delta)
    {
        int3 deltaSign = deltaSigns[index];
        return new float3(parentPosition.x + deltaSign.x * delta, parentPosition.y + deltaSign.y * delta, parentPosition.z + deltaSign.z * delta);
    }

    void AddNodeToGameObject(OctreeNode node)
    {
        GameObject obj = poolQueue.Dequeue();

        if (VisualDebugger)
        {
            Renderer cubeRenderer = obj.GetComponent<Renderer>();
            Color color = new Color(
                UnityEngine.Random.Range(0f, 1f),
                UnityEngine.Random.Range(0f, 1f),
                UnityEngine.Random.Range(0f, 1f),
                0.2f
            );

            cubeRenderer.material.SetColor("_Color", color);
            obj.transform.localScale = new Vector3(node.size, node.size, node.size);
        }

        obj.name = "" + node.guid;
        obj.transform.position = node.position;
        obj.SetActive(true);
    }

    void RemoveNodeFromGameObject(long id)
    {
        var obj = GameObject.Find("" + id);
        if (obj != null)
        {
            obj.name = "Chunk";
            obj.transform.position = Vector3.zero;
            poolQueue.Enqueue(obj);
            obj.SetActive(false);
        }
    }

    void OnDrawGizmos()
    {
        if (drawGizmo && nodes != null && nodes.Count > 0)
        {
            foreach (var node in nodes.Values)
            {
                switch (node.lodLevel)
                {
                    case 0:
                        Gizmos.color = Color.red;
                        break;
                    case 1:
                        Gizmos.color = Color.green;
                        break;
                    case 2:
                        Gizmos.color = Color.blue;
                        break;
                    case 3:
                        Gizmos.color = Color.yellow;
                        break;
                    case 4:
                        Gizmos.color = new Color(0.4f,0.5f,0.2f);
                        break;
                    case 5:
                        Gizmos.color = Color.red;
                        break;
                    case 6:
                        Gizmos.color = Color.cyan;
                        break;
                    case 7:
                        Gizmos.color = new Color(1.0f, 0.5f, 0.5f);
                        break;
                    case 8:
                        Gizmos.color = Color.yellow;
                        break;
                    case 9:
                        Gizmos.color = Color.blue;
                        break;
                    case 10:
                        Gizmos.color = Color.green;
                        break;
                    default:
                        Gizmos.color = Color.red;
                        break;
                }
                Vector3 chunkSize = new Vector3(node.size, node.size, node.size);
                Vector3 chunkCenterPosition = new Vector3(node.position.x, node.position.y, node.position.z);
                Gizmos.DrawWireCube(chunkCenterPosition, chunkSize);
            }
        }
    }
}
