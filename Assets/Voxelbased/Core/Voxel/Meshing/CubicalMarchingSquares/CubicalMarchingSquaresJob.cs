using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;

namespace VoxelbasedCom 
{
    /// <summary>
    /// Job to create the hermite data
    /// </summary>
    public struct CMSHermiteDataJob : IJobParallelFor 
    {
        //Parameters for hermite data generation
        public static float normalEpsilon = 0.01f;
        public static int steps = 8;
        private HermiteDataCMS hermiteData;
        public void Execute(int index) 
        {
        
        }
        //Static Arrays
        #region Static Arrays
        private static int[,] faceToEdges =
        {
            { 0, 1, 2, 3 },
            { 2, 4, 5, 6 },
            { 4, 1, 7, 8 },
            { 5, 8, 9, 10 },
            { 6, 10, 11, 3 },
            { 9, 7, 0, 11 }
        };//Transformation from cubeFaces to edgeIndices    
        private static int[,] faceToCorners =
        {
            { 1, 6, 5, 0 },
            { 0, 5, 4, 3 },
            { 4, 5, 6, 7 },
            { 3, 4, 7, 2 },
            { 0, 3, 2, 1 },
            { 2, 7, 6, 1 }
        };//Transformation from cubeFaces to cornerIndices        
        private static int[,] edgeToCorners =
        {
            { 1, 6 },//0
            { 6, 5 },//1
            { 5, 0 },//2
            { 1, 0 },//3
            { 5, 4 },//4
            { 4, 3 },//5
            { 0, 3 },//6
            { 7, 6 },//7
            { 4, 7 },//8
            { 7, 2 },//9
            { 3, 2 },//10
            { 2, 1 },//11
        };//Transformation from edgeIndices to cornerIndices
        /// <summary>
        /// Since we wrap the marching squares algorithm (2D) around a 3D cube, there are some cases where we need to flip some information for it to match up locally (2D)
        /// This table is used to flip the axis when checking for overlapping sharp features (I think?).
        /// </summary>
        private static bool[,] faceFlipAmbiguousFix =
        {
            { true, false, true },//0 Y axis
            { false, true, true },//1 X axis
            { false, false, false },//2 Z axis
            { true, false, true },//3 Y axis
            { true, true, false },//4 Z axis
            { false, true, true },//5 X axis
        };
        private static Vector3[] corners =
        {
            Vector3.zero,//0
            new Vector3(1, 0, 0),//1
            new Vector3(1, 1, 0),//2
            new Vector3(0, 1, 0),//3
            new Vector3(0, 1, 1),//4
            new Vector3(0, 0, 1),//5
            new Vector3(1, 0, 1),//6
            new Vector3(1, 1, 1),//7
        };//Some corner offsets    
        private static Vector3[] facesAxis =
        {
            new Vector3(0, 1, 0),//0
            new Vector3(1, 0, 0),//1
            new Vector3(0, 0, 1),//2
            new Vector3(0, 1, 0),//3
            new Vector3(0, 0, 1),//4
            new Vector3(1, 0, 0),//5
        };//Axis of the faces   
        public static Vector3[] faceNormals =
        {
            new Vector3(0, -1, 0),
            new Vector3(-1, 0, 0),
            new Vector3(0, 0, 1),
            new Vector3(0, 1, 0),
            new Vector3(0, 0, -1),
            new Vector3(1, 0, 1)
        };//Normals of the faces
        private static Vector3[] facesPositions =
        {
            new Vector3(0.5f, 0, 0.5f),
            new Vector3(0, 0.5f, 0.5f),
            new Vector3(0.5f, 0.5f, 1),
            new Vector3(0.5f, 1, 0.5f),
            new Vector3(0.5f, 0.5f, 0),
            new Vector3(1, 0.5f, 0.5f)
        };
        private static Vector3[] edgePositions =
        {
            new Vector3(1, 0, 0.5f),//0
            new Vector3(0.5f, 0, 1),//1
            new Vector3(0, 0, 0.5f),//2
            new Vector3(0.5f, 0, 0),//3
            new Vector3(0, 0.5f, 1),//4
            new Vector3(0, 1, 0.5f),//5
            new Vector3(0, 0.5f, 0),//6
            new Vector3(1, 0.5f, 1),//7
            new Vector3(0.5f, 1, 1),//8
            new Vector3(1, 1, 0.5f),//9
            new Vector3(0.5f, 1, 0),//10
            new Vector3(1, 0.5f, 0)//11
        };//The position of the edges
        private static Vector3Int[] edgePositions2 =
        {
            new Vector3Int(2, 0, 1),//0
            new Vector3Int(1, 0, 2),//1
            new Vector3Int(0, 0, 1),//2
            new Vector3Int(1, 0, 0),//3
            new Vector3Int(0, 1, 2),//4
            new Vector3Int(0, 2, 1),//5
            new Vector3Int(0, 1, 0),//6
            new Vector3Int(2, 1, 2),//7
            new Vector3Int(1, 2, 2),//8
            new Vector3Int(2, 2, 1),//9
            new Vector3Int(1, 2, 0),//10
            new Vector3Int(2, 1, 0)//11
        };//The position of the edges
        private int[] cornerIndices;//A way to calculate index offsets without needing to flatten a position    
        private static int[,] msTable =
        {
            //Without sharp features
            { -1, -1, -1, -1, -1, -1, -1, -1 },//0
            { 3, 0, -1, -1, -1, -1, -1, -1 },//1c
            { 0, 1, -1, -1, -1, -1, -1, -1 },//2c
            { 3, 1, -1, -1, -1, -1, -1, -1 },//3
            { 1, 2, -1, -1, -1, -1, -1, -1 },//4c
            { 1, 2, 3, 0, -1, -1, -1, -1 },//5d
            { 0, 2, -1, -1, -1, -1, -1, -1 },//6
            { 3, 2, -1, -1, -1, -1, -1, -1 },//7c
            { 2, 3, -1, -1, -1, -1, -1, -1 },//8c
            { 2, 0, -1, -1, -1, -1, -1, -1 },//9
            { 0, 1, 2, 3, -1, -1, -1, -1 },//10d
            { 2, 1, -1, -1, -1, -1, -1, -1 },//11c
            { 1, 3, -1, -1, -1, -1, -1, -1 },//12
            { 1, 0, -1, -1, -1, -1, -1, -1 },//13c
            { 0, 3, -1, -1, -1, -1, -1, -1 },//14c
            { -1, -1, -1, -1, -1, -1, -1, -1 },//15
            //Ambiguous cases
            { 1, 0, 3, 2, -1, -1, -1, -1 },//5a
            { 2, 1, 0, 3, -1, -1, -1, -1 },//10a

            { -1, -1, -1, -1, -1, -1, -1, -1 },//0s
            { 3, 4, 4, 0, -1, -1, -1, -1 },//1s
            { 0, 4, 4, 1, -1, -1, -1, -1 },//2s
            { 3, 4, 4, 1, -1, -1, -1, -1 },//3s
            { 1, 4, 4, 2, -1, -1, -1, -1 },//4s
            { 1, 5, 5, 2, 3, 4, 4, 0 },//5sd
            { 0, 4, 4, 2, -1, -1, -1, -1 },//6s
            { 3, 4, 4, 2, -1, -1, -1, -1 },//7s
            { 2, 4, 4, 3, -1, -1, -1, -1 },//8s
            { 2, 4, 4, 0, -1, -1, -1, -1 },//9s
            { 0, 5, 5, 1, 2, 4, 4, 3 },//10sd
            { 2, 4, 4, 1, -1, -1, -1, -1 },//11s
            { 1, 4, 4, 3, -1, -1, -1, -1 },//12s
            { 1, 4, 4, 0, -1, -1, -1, -1 },//13s
            { 0, 4, 4, 3, -1, -1, -1, -1 },//14s
            { -1, -1, -1, -1, -1, -1, -1, -1 },//15s
            //Ambiguous cases
            { 1, 4, 4, 0, 3, 5, 5, 2 },//5as
            { 2, 4, 4, 1, 0, 5, 5, 3 },//10as

        };//The marching squares edge table wich tells us what edges to connect
        private static int[,] msCaseToEdgeGroups =
        {
            //First two numbers is the first pair of edges, followed by their shared vertex.
            //Then in the following numbers, the first two are the second pair of edges, followed by their shared vertex.
            { -1, -1, -1, -1, -1, -1 },//0
            { 3, 0, 0, -1, -1, -1 },//1
            { 0, 1, 0, -1, -1, -1 },//2
            { 3, 1, 0, -1, -1, -1 },//3
            { 1, 2, 0, -1, -1, -1 },//4
            { 1, 2, 1, 3, 0, 0 },//5
            { 0, 2, 0, -1, -1, -1 },//6
            { 3, 2, 0, -1, -1, -1},//7
            { 2, 3, 0, -1, -1, -1 },//8
            { 2, 0, 0, -1, -1, -1 },//9
            { 2, 3, 0, 0, 1, 1 },//10
            { 2, 1, 0, -1, -1, -1 },//11
            { 1, 3, 0, -1, -1, -1 },//12
            { 1, 0, 0, -1, -1, -1 },//13
            { 0, 3, 0, -1, -1, -1 },//14
            { -1, -1, -1, -1, -1, -1 },//15
            //Ambiguous cases
            { 1, 0, 0, 3, 2, 1 },//5a
            { 2, 1, 0, 0, 3, 1 },//10a
        };//Transforms the marching squares case into two pairs of edges, with their corresponding shared vertex
        //Flatten a position into an index
        public static int FlattenIndex(Vector3Int position, int gridSize)
        {
            return (position.z * gridSize * gridSize) + (position.y * gridSize) + position.x;
        }
        #endregion
        //-----Hermite Data Generation-----\\
        #region Hermite Data Generation

        /// <summary>
        /// Calculate the normal
        /// </summary>
        private Vector3 FindNormal(Vector3 pos)
        {
            Vector3 normal;
            normal.x = GetDensity(pos.x + normalEpsilon, pos.y, pos.z) - GetDensity(pos.x - normalEpsilon, pos.y, pos.z);
            normal.y = GetDensity(pos.x, pos.y + normalEpsilon, pos.z) - GetDensity(pos.x, pos.y - normalEpsilon, pos.z);
            normal.z = GetDensity(pos.x, pos.y, pos.z + normalEpsilon) - GetDensity(pos.x, pos.y, pos.z - normalEpsilon);
            return normal;
        }

        /// <summary>
        /// Sample the isoline density multiple times through a specific edge
        /// </summary>
        public IntersectionSample EdgeTrace(float startDensity, float endDensity, Vector3 startEdge, Vector3 endEdge, Vector3 dir, out bool hit)
        {
            hit = startDensity < 0 ^ endDensity < 0;
            if (!hit) return new IntersectionSample();//Return early
            Vector3 intersectionPoint = startEdge;
            Vector3 lastIntersectionPoint = startEdge;
            float lastDensity = startDensity;
            for (int i = 1; i < steps + 1; i++)
            {
                intersectionPoint = startEdge + (dir * ((float)i / ((float)steps)));
                float currentDensity = i == steps ? endDensity : isosurface.GetDensity(intersectionPoint + offset);
                if (lastDensity < 0 ^ currentDensity < 0)
                {
                    //Intersection!!
                    Vector3 pos = Vector3.Lerp(intersectionPoint, lastIntersectionPoint, Mathf.InverseLerp(currentDensity, lastDensity, 0));
                    IntersectionSample sample = new IntersectionSample() { p = pos, n = FindNormal(pos + offset).normalized };
                    return sample;
                }
                lastDensity = currentDensity;
                lastIntersectionPoint = intersectionPoint;
            }
            return new IntersectionSample();//Oh no...
        }

        /// <summary>
        /// Run through all the edges and generate the hermite data
        /// </summary>
        public void GenerateHermiteData()
        {
            bool hita, hitb, hitc;
            float[] densities = new float[(chunkSize + 2) * (chunkSize + 2) * (chunkSize + 2)];
            IntersectionSample a, b, c;
            Vector3 pos;
            for (int z = 0, i = 0; z < chunkSize + 2; z++)
            {
                for (int y = 0; y < chunkSize + 2; y++)
                {
                    for (int x = 0; x < chunkSize + 2; x++, i++)
                    {
                        pos = new Vector3(x, y, z);
                        float density = isosurface.GetDensity(pos + offset);
                        densities[i] = density;
                        if (i < ((chunkSize + 1) + 1) * ((chunkSize + 1) + 1) * ((chunkSize + 1) + 1))
                        {
                            int i2 = FlattenIndex(new Vector3Int(x, y, z), (chunkSize + 1));
                            hermiteData.grid[i2] = new Voxel() { density = density, pos = new Vector3Int(x, y, z) };
                        }
                    }
                }
            }

            for (int z = 0, i = 0; z < (chunkSize + 1); z++)
            {
                for (int y = 0; y < (chunkSize + 1); y++)
                {
                    for (int x = 0; x < (chunkSize + 1); x++, i++)
                    {
                        pos = new Vector3(x, y, z);
                        i = FlattenIndex(new Vector3Int(x, y, z), chunkSize + 2);
                        //Gizmos.DrawWireSphere(new Vector3(x, y, z), hermiteData.grid[i].density);
                        //The edge intersection stuff
                        a = EdgeTrace(densities[i], densities[i + 1], new Vector3(x, y, z), pos + Vector3.right, Vector3.right, out hita);
                        b = EdgeTrace(densities[i], densities[i + ((chunkSize + 2))], new Vector3(x, y, z), pos + Vector3.up, Vector3.up, out hitb);
                        c = EdgeTrace(densities[i], densities[i + ((chunkSize + 2)) * ((chunkSize + 2))], new Vector3(x, y, z), pos + Vector3.forward, Vector3.forward, out hitc);

                        if (hita) hermiteData.hermiteData.Add(FlattenIndex(new Vector3Int(x * 2 + 1, y * 2, z * 2), (chunkSize) * 2 + 3), a);
                        if (hitb) hermiteData.hermiteData.Add(FlattenIndex(new Vector3Int(x * 2, y * 2 + 1, z * 2), (chunkSize) * 2 + 3), b);
                        if (hitc) hermiteData.hermiteData.Add(FlattenIndex(new Vector3Int(x * 2, y * 2, z * 2 + 1), (chunkSize) * 2 + 3), c);
                    }
                }
            }
        }
        #endregion
        #endregion
    }
}