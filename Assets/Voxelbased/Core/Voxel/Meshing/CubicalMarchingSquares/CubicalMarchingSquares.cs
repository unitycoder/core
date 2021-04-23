//-----------------------------------------------------------------------
// <copyright file="CubicalMarchingSquares.cs" company="sidit77">
//     Copyright (c) sidit77. See LICENSE.md.
//     original repo: https://github.com/sidit77/CMS/blob/master/CMS-Test/
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;

namespace VoxelbasedCom
{
    public class CubicalMarchingSquares : MeshBuilder
    {
        private List<Vector3> vertices = new List<Vector3>();
        private List<int> triangles = new List<int>();
        private List<Vector3> normals = new List<Vector3>();
        private HermiteDataCMS hermiteData;
        //Parameters for hermite data generation
        public float normalEpsilon = 0.01f;
        public int steps = 8;
        //Parameters for mesh generation
        private float angleThreshold = 25;//If the angle between two sample normals is bigger than this, the edge is then classified as a sharp edge
        private bool sharpFeatures = true;//Yknow... sharp features?
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
        public CubicalMarchingSquares(Isosurface isosurface, Vector3 offset, int chunkSize) : base(isosurface, offset, chunkSize)
        {
            hermiteData = new HermiteDataCMS((chunkSize));
            GenerateHermiteData();
            cornerIndices = new int[8] {
                0,
                1,
                ((chunkSize+1)) + 1,
                ((chunkSize+1)),
                ((chunkSize+1)) * ((chunkSize+1)) + ((chunkSize+1)),
                ((chunkSize+1)) * ((chunkSize+1)),
                ((chunkSize+1)) * ((chunkSize+1)) + 1,
                ((chunkSize+1))* ((chunkSize+1)) + ((chunkSize+1)) + 1
            };
        }

        //-----Hermite Data Generation-----\\
        #region Hermite Data Generation

        /// <summary>
        /// Calculate the normal
        /// </summary>
        private Vector3 FindNormal(Vector3 pos)
        {
            Vector3 normal;
            normal.x = isosurface.GetDensity(pos.x + normalEpsilon, pos.y, pos.z) - isosurface.GetDensity(pos.x - normalEpsilon, pos.y, pos.z);
            normal.y = isosurface.GetDensity(pos.x, pos.y + normalEpsilon, pos.z) - isosurface.GetDensity(pos.x, pos.y - normalEpsilon, pos.z);
            normal.z = isosurface.GetDensity(pos.x, pos.y, pos.z + normalEpsilon) - isosurface.GetDensity(pos.x, pos.y, pos.z - normalEpsilon);
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
        //-----Mesh Generation-----\\
        #region Mesh Generation

        /// <summary>
        /// Line-line intersection from https://stackoverflow.com/questions/59449628/check-when-two-vector3-lines-intersect-unity3d
        /// </summary>        
        public static Vector3 LineLineIntersection(Vector3 linePoint1, Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2)
        {
            Vector3 lineVec3 = linePoint2 - linePoint1;
            Vector3 crossVec1and2 = Vector3.Cross(lineVec1, lineVec2);
            Vector3 crossVec3and2 = Vector3.Cross(lineVec3, lineVec2);

            float planarFactor = Vector3.Dot(lineVec3, crossVec1and2);

            //is coplanar, and not parallel
            if (Mathf.Approximately(planarFactor, 0f) &&
                !Mathf.Approximately(crossVec1and2.sqrMagnitude, 0f))
            {
                float s = Vector3.Dot(crossVec3and2, crossVec1and2) / crossVec1and2.sqrMagnitude;
                return linePoint1 + (lineVec1 * s);
            }
            else
            {
                //return Vector3.zero;
                return (linePoint1 + linePoint2) / 2.0f;
            }
        }

        /// <summary>
        /// Check if the calculated vector is inside the volume
        /// </summary>        
        private bool InsideVolume(Vector3 a)
        {
            return (a.x >= 0 && a.y >= 0 && a.z >= 0) && (a.x <= chunkSize && a.y <= chunkSize && a.z <= chunkSize);
        }

        /// <summary>
        /// Take two intersection samples and turn them into one position
        /// </summary>        
        private Vector3 CalculateVert(IntersectionSample a, IntersectionSample b, Vector3 axis, Vector3Int cellPosition, out bool sharp)
        {
            //TODO: I gotta change this implementation so it uses 3 planes and find the intersection of those instead of rotating the normal and stuff

            sharp = Vector3.Dot(a.n, b.n) < -(angleThreshold / 90f - 1);
            if (!sharp) return ((a.p + b.p) / 2.0f);
            /*
            Gizmos.color = Color.red;
            DrawArrow.ForGizmo(a.p + cellPosition, a.n * 0.2f);
            DrawArrow.ForGizmo(b.p + cellPosition, b.n * 0.2f);
            Gizmos.color = Color.white;
            */
            //Rotate the perpendicular normal in one of the directions so it doesn't go out of bounds
            a.n = Vector3.Scale(a.n, Vector3.one - axis).normalized;
            b.n = Vector3.Scale(b.n, Vector3.one - axis).normalized;
            Vector3 testNormal1 = Quaternion.AngleAxis(90, axis) * a.n;
            Vector3 testNormal2 = Quaternion.AngleAxis(-90, axis) * a.n;
            Vector3 normala = InsideVolume(testNormal1 + a.p) ? testNormal2 : testNormal1;

            Vector3 testNormal21 = Quaternion.AngleAxis(90, axis) * b.n;
            Vector3 testNormal22 = Quaternion.AngleAxis(-90, axis) * b.n;
            Vector3 normalb = InsideVolume(testNormal21 + b.p) ? testNormal22 : testNormal21;
            //Gotta make sure it runs the line intersection in 2D, since this is per face
            normala = Vector3.Scale(normala, Vector3.one - axis).normalized;
            normalb = Vector3.Scale(normalb, Vector3.one - axis).normalized;
            Vector3 calculatedPosition = LineLineIntersection(a.p, normala, b.p, normalb);
            //Check if this a sharp feature
            return ((Vector3.Distance(calculatedPosition, cellPosition + new Vector3(0.5f, 0.5f, 0.5f)) < 1) && InsideVolume(calculatedPosition)) ? calculatedPosition : (a.p + b.p) / 2.0f;
            //return (InsideVolume(calculatedPosition)) ? calculatedPosition : (a.p + b.p) / 2.0f;
        }

        /// <summary>
        /// Calculate the two middle vertex points
        /// </summary>
        private void CalculateMiddleVertices(ref int msCase, IntersectionSample[] localIntersectionSamples, int faceIndex, Vector3Int cellPos, out Vector3 middleVertex1, out Vector3 middleVertex2)
        {
            middleVertex1 = Vector3.zero;
            middleVertex2 = Vector3.zero;
            bool switchCase1 = true;
            bool switchCase2 = true;
            if (sharpFeatures)
            {
                //Calculate the middle vertices!!
                int modCase = msCase % 18;
                int vertex = msCaseToEdgeGroups[modCase, 2];
                if (vertex == 0 || vertex == 1)
                {
                    IntersectionSample sample1 = localIntersectionSamples[msCaseToEdgeGroups[modCase, 0]];
                    IntersectionSample sample2 = localIntersectionSamples[msCaseToEdgeGroups[modCase, 1]];
                    //Switch between the two center vertices (Yes there are two of them because of ambiguous cases)
                    if (vertex == 0)
                    {
                        middleVertex1 = CalculateVert(sample1, sample2, facesAxis[faceIndex], cellPos, out switchCase1);
                    }
                    else if (vertex == 1)
                    {
                        middleVertex2 = CalculateVert(sample1, sample2, facesAxis[faceIndex], cellPos, out switchCase2);
                    }
                }

                vertex = msCaseToEdgeGroups[modCase, 5];
                if (vertex == 0 || vertex == 1)
                {
                    IntersectionSample sample1 = localIntersectionSamples[msCaseToEdgeGroups[modCase, 3]];
                    IntersectionSample sample2 = localIntersectionSamples[msCaseToEdgeGroups[modCase, 4]];
                    //Switch between the two center vertices (Yes there are two of them because of ambiguous cases)
                    if (vertex == 0)
                    {
                        middleVertex1 = CalculateVert(sample1, sample2, facesAxis[faceIndex], cellPos, out switchCase1);
                    }
                    else if (vertex == 1)
                    {
                        middleVertex2 = CalculateVert(sample1, sample2, facesAxis[faceIndex], cellPos, out switchCase2);
                    }
                }
                //When both (or only one in some cases) of our vertices are considered sharp, we switch to a sharp case
                msCase = (switchCase1 && switchCase2 && msCase < 18) ? msCase + 18 : msCase;
            }
        }

        /// <summary>
        /// Generate the whole cms mesh
        /// </summary>
        public void GetMesh()
        {
            bool[] bools = new bool[8];
            for (int z = 0; z < chunkSize; z++)
            {
                for (int y = 0; y < chunkSize; y++)
                {
                    for (int x = 0; x < chunkSize; x++)
                    {
                        int i = FlattenIndex(new Vector3Int(x, y, z), (chunkSize + 1));
                        Voxel[] localArray = new Voxel[] {
                        hermiteData.grid[i + cornerIndices[0]],
                        hermiteData.grid[i + cornerIndices[1]],
                        hermiteData.grid[i + cornerIndices[2]],
                        hermiteData.grid[i + cornerIndices[3]],
                        hermiteData.grid[i + cornerIndices[4]],
                        hermiteData.grid[i + cornerIndices[5]],
                        hermiteData.grid[i + cornerIndices[6]],
                        hermiteData.grid[i + cornerIndices[7]]};
                        bools[0] = localArray[0].density < 0;
                        bools[1] = localArray[1].density < 0;
                        bools[2] = localArray[2].density < 0;
                        bools[3] = localArray[3].density < 0;
                        bools[4] = localArray[4].density < 0;
                        bools[5] = localArray[5].density < 0;
                        bools[6] = localArray[6].density < 0;
                        bools[7] = localArray[7].density < 0;
                        if (true)
                        {
                            GenerateCMSCube(ref localArray, hermiteData.hermiteData, vertices, triangles, new Vector3Int(x, y, z));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Generate a single cms cube
        /// This is very, very, very unoptimized, though I sacrificed it for clarity since this is the first time I write a CMS implementation
        /// </summary>
        public void GenerateCMSCube(ref Voxel[] voxels, Dictionary<int, IntersectionSample> hermiteData, List<Vector3> vertices, List<int> triangles, Vector3Int cellPos)
        {
            //Check each face
            List<Segment> segments = new List<Segment>();
            Dictionary<Vector3, List<Segment>> dictSegments = new Dictionary<Vector3, List<Segment>>();//Segments stored by their starting point
            IntersectionSample[] intersectionPoints = new IntersectionSample[12];
            for (int i = 0; i < 6; i++)
            {
                int msCase = 0;
                //Calculate the marching squares case
                IntersectionSample[] localIntersectionSamples = new IntersectionSample[4];
                for (int c = 0; c < 4; c++)
                {
                    if (voxels[edgeToCorners[faceToEdges[i, c], 0]].density < 0f ^ voxels[edgeToCorners[faceToEdges[i, c], 1]].density < 0f)
                    {
                        intersectionPoints[faceToEdges[i, c]] = hermiteData[FlattenIndex((((edgePositions2[faceToEdges[i, c]] + cellPos * 2))), ((chunkSize)) * 2 + 3)];
                        localIntersectionSamples[c] = intersectionPoints[faceToEdges[i, c]];
                        //We found an intersection at this edge, gotta read the intersection sample now
                    }
                    msCase += voxels[faceToCorners[i, c]].density < 0f ? 1 << c : 0;
                }
                if (msCase == 15 || msCase == 0) continue;//This cube is either completely filled or completely empty
                                                          //The marching squares vertices picked by the vector calculation
                Vector3 middleVertex1, middleVertex2;

                //Calculate the middle vertices and detect sharp feature cases!!
                CalculateMiddleVertices(ref msCase, localIntersectionSamples, i, cellPos, out middleVertex1, out middleVertex2);


                //Gotta check if sharp features intersect, and if so we have an ambiguous case
                if ((msCase == 23 || msCase == 28))
                {
                    /*
                    //Some debug stuff
                    Debug.Log(i);
                    Debug.Log("X: " + ((middleVertex1.x <= middleVertex2.x) ^ faceFlipAmbiguousFix[i, 0]));
                    Debug.Log("Y: " + ((middleVertex1.y <= middleVertex2.y) ^ faceFlipAmbiguousFix[i, 1]));
                    Debug.Log("Z: " + ((middleVertex1.z <= middleVertex2.z) ^ faceFlipAmbiguousFix[i, 2]));
                    */

                    if (((middleVertex1.x <= middleVertex2.x) ^ faceFlipAmbiguousFix[i, 0]) && ((middleVertex1.y <= middleVertex2.y) ^ faceFlipAmbiguousFix[i, 1]) && ((middleVertex1.z <= middleVertex2.z) ^ faceFlipAmbiguousFix[i, 2]))
                    {
                        msCase = msCase == 23 ? 34 : msCase;
                        msCase = msCase == 28 ? 35 : msCase;
                        //Recalculate the vertex positions
                        CalculateMiddleVertices(ref msCase, localIntersectionSamples, i, cellPos, out middleVertex1, out middleVertex2);
                    }

                }
                //Generate the segments
                for (int c = 0; c < 8; c += 2)
                {
                    if (msTable[msCase, c] != -1)
                    {
                        Vector3 startVert = sharpFeatures ? (msTable[msCase, c] == 4 ? middleVertex1 : middleVertex2) : cellPos + facesPositions[i];
                        Vector3 endVert = sharpFeatures ? (msTable[msCase, c + 1] == 4 ? middleVertex1 : middleVertex2) : cellPos + facesPositions[i];

                        //Only use the table for the edges, the middle point is calculated
                        if (msTable[msCase, c] <= 3)
                        {
                            int edgeIndex = faceToEdges[i, msTable[msCase, c]];
                            startVert = (sharpFeatures ? intersectionPoints[edgeIndex].p : Vector3.Lerp(corners[edgeToCorners[edgeIndex, 0]], corners[edgeToCorners[edgeIndex, 1]], 0.5f) + cellPos);
                        }
                        if (msTable[msCase, c + 1] <= 3)
                        {
                            int edgeIndex1 = faceToEdges[i, msTable[msCase, c + 1]];
                            endVert = (sharpFeatures ? intersectionPoints[edgeIndex1].p : Vector3.Lerp(corners[edgeToCorners[edgeIndex1, 0]], corners[edgeToCorners[edgeIndex1, 1]], 0.5f) + cellPos);
                        }
                        /*
                        startVert += cellPos;
                        endVert += cellPos;
                        */
                        //Finally, add the 2D segment
                        Segment segment = new Segment
                        {
                            start = startVert,
                            end = endVert,
                            face = i,
                            sharp1 = msTable[msCase, c] == 4 || msTable[msCase, c] == 5,
                            sharp2 = msTable[msCase, c + 1] == 4 || msTable[msCase, c + 1] == 5,
                            vert1 = msTable[msCase, c] == 4,
                            vert2 = msTable[msCase, c + 1] == 4,
                            cell = cellPos
                        };
                        if ((startVert - endVert).magnitude > 0)
                        {
                            segments.Add(segment);
                            if (!dictSegments.ContainsKey(startVert))
                            {
                                dictSegments.Add(startVert, new List<Segment>(new Segment[] { segment }));
                            }
                            else
                            {
                                dictSegments[startVert].Add(segment);
                            }
                        }
                    }
                }
            }
            //Get the mesh for this CMS cube
            TriangulateSegments(segments, dictSegments, vertices, triangles);
        }

        /// <summary>
        /// TODO: Use SVD to get the sharp vertex from a component
        /// Generate a mesh out of the segments
        /// </summary>
        public void TriangulateSegments(List<Segment> segments, Dictionary<Vector3, List<Segment>> dictSegments, List<Vector3> vertices, List<int> triangles)
        {
            //Clean up the arrays first, making sure there aren't any loose segments
            //TODO: Reprogram this because it's actual  s h i t
            List<Component> components = new List<Component>();//All the final components
            for (int i = 0; i < segments.Count; i++)
            {
                //components.Clear();
                Vector3 startingPoint = segments[0].start;//Check if the end of the current segment is this, if it is then we reached the end
                if (!dictSegments.ContainsKey(startingPoint)) return;//Wat de fak?
                Segment currentSegment = dictSegments[startingPoint][0];//Current segment
                List<Segment> finalSegments = new List<Segment>();//The final segments that will be appended to the component
                finalSegments.Add(currentSegment);//Add the original segment
                                                  //Average of the sharp vertices axis-wise so we don't lose sharp features
                Vector3 vertexX = Vector3.zero;
                int numX = 0;
                Vector3 vertexY = Vector3.zero;
                int numY = 0;
                Vector3 vertexZ = Vector3.zero;
                int numZ = 0;
                //A simple average vertex of all the edges
                Vector3 simpleAverage = Vector3.zero;
                int simpleAverageNum = 0;

                Vector3 advancedAverage = Vector3.zero;
                int advancedAverageNum = 0;
                for (int k = 0; k < 32; k++)
                {
                    //Over here I need to check if the next segment is a valid one (A valid segment is a segment that has an end connected to another segment)
                    Vector3 newKey = currentSegment.end;
                    if (!dictSegments.ContainsKey(newKey)) return;//Wat de fak?
                    if (dictSegments[newKey].Count <= 1)
                    {
                        currentSegment = dictSegments[newKey][0];//Recursively find the next segment
                        dictSegments.Remove(newKey);
                    }
                    else
                    {
                        //Loop through all the segments that have this specific key as their starting point and use the best one
                        Segment bestSegment = new Segment();
                        for (int j = 0; j < dictSegments[newKey].Count; j++)
                        {
                            Segment segment = dictSegments[newKey][j];
                            bestSegment = segment;
                            if (segment.face == currentSegment.face && !segment.Equals(currentSegment))
                            {
                                bestSegment = segment;
                            }
                        }
                        dictSegments[newKey].Remove(bestSegment);
                        currentSegment = bestSegment;//Find the next segment who is on the same face as us
                    }
                    simpleAverageNum++;
                    simpleAverage += (currentSegment.start + currentSegment.end) / 2.0f;
                    if (currentSegment.sharp1)
                    {
                        advancedAverage += currentSegment.start;
                        advancedAverageNum++;
                        //The first vertex is sharp
                        if (currentSegment.face == 1 || currentSegment.face == 5)//X Axis
                        {
                            vertexX += currentSegment.start;
                            numX++;
                        }
                        if (currentSegment.face == 0 || currentSegment.face == 3)//Y Axis
                        {
                            vertexY += currentSegment.start;
                            numY++;
                        }
                        if (currentSegment.face == 4 || currentSegment.face == 2)//Z Axis
                        {
                            vertexZ += currentSegment.start;
                            numZ++;
                        }
                    }
                    if (currentSegment.sharp2)
                    {
                        advancedAverage += currentSegment.end;
                        advancedAverageNum++;
                        //The first vertex is sharp
                        if (currentSegment.face == 1 || currentSegment.face == 5)//X Axis
                        {
                            vertexX += currentSegment.end;
                            numX++;
                        }
                        if (currentSegment.face == 0 || currentSegment.face == 3)//Y Axis
                        {
                            vertexY += currentSegment.end;
                            numY++;
                        }
                        if (currentSegment.face == 4 || currentSegment.face == 2)//Z Axis
                        {
                            vertexZ += currentSegment.end;
                            numZ++;
                        }
                    }
                    //DrawArrow.ForGizmo(currentSegment.start * 5, currentSegment.end - currentSegment.start);
                    if (startingPoint == currentSegment.start)
                    {
                        //Yayy we found the end
                        simpleAverage = simpleAverage / (float)simpleAverageNum;
                        advancedAverage = advancedAverage / (float)advancedAverageNum;

                        if (numX > 0) vertexX = vertexX / (float)numX;
                        if (numY > 0) vertexY = vertexY / (float)numY;
                        if (numZ > 0) vertexZ = vertexZ / (float)numZ;
                        //Gizmos.DrawSphere(vertexZ, 0.2f);
                        Vector3 finalVertex;
                        finalVertex.x = (numY == 0 || numZ == 0) ? advancedAverage.x : (vertexY.x + vertexZ.x) / 2.0f;
                        finalVertex.y = (numX == 0 || numZ == 0) ? advancedAverage.y : (vertexX.y + vertexZ.y) / 2.0f;
                        finalVertex.z = (numX == 0 || numY == 0) ? advancedAverage.z : (vertexX.z + vertexY.z) / 2.0f;

                        if (numX == 0 && numY == 0 && numZ == 0) finalVertex = simpleAverage;
                        components.Add(new Component() { segments = finalSegments.ToArray(), vertex = finalVertex });
                        foreach (var item in finalSegments)
                        {
                            segments.Remove(item);
                        }
                        break;
                    }
                    finalSegments.Add(currentSegment);
                }
            }
            //Create the triangle fan for each component
            for (int i = 0; i < components.Count; i++)
            {
                CreateTriangleFan(components[i], vertices, triangles);
            }
        }

        /// <summary>
        /// Make a triangle fan out of a component
        /// </summary>
        public void CreateTriangleFan(Component component, List<Vector3> vertices, List<int> triangles)
        {
            for (int i = 0; i < component.segments.Length; i++)
            {
                //Make one triangle

                vertices.Add(component.segments[i].start);
                vertices.Add(component.segments[i].end);
                vertices.Add(component.vertex);

                triangles.Add(vertices.Count - 3);
                triangles.Add(vertices.Count - 2);
                triangles.Add(vertices.Count - 1);
            }
        }

        protected override JobHandle StartMeshJob(JobHandle inputDeps = default)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// A single edge for surface reconstruction
        /// </summary>
        public struct Segment
        {
            public int face;//Which face this segment was generated on
            public Vector3 start, end;//Start and end position    
            public bool sharp1, sharp2;//Tells us if the first vertex is vertex made from a sharp edge, same goes for the second vertex    
            public bool vert1, vert2;//Switch between using middleVertex1 and middleVertex2
            public Vector3Int cell;
        }

        /// <summary>
        /// A component, which is a loop of segments
        /// </summary>
        public struct Component
        {
            public Segment[] segments;//All the segments in this component
            public IntersectionSample[] intersectionSamples;//Samples
            public Vector3 vertex;//The sharp vertex that will be used in the triangle fan
        }
        #endregion
    }
}
