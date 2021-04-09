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
        public CubicalMarchingSquares(Isosurface isosurface, Vector3 offset, int chunkSize) : base(isosurface, offset, chunkSize)
        { } 

        /*public override bool GetMeshData(out MeshData meshData)
        {
            GetMesh();

            normals = NormalSolver.RecalculateNormals(triangles, vertices, 0);

            meshData = new MeshData
            {
                //vertices = vertices,
                //triangles = triangles,
                normals = normals
            };
            return true;
        }*/

        private Vector3 GetNormal(Vector3 p)
        {
            return GetNormal(p.x, p.y, p.z);
        }


        /// <summary>
        /// Calculate normal for point
        /// </summary>
        /// <returns>Normal</returns>
        private Vector3 GetNormal(float x, float y, float z)
        {
            Vector3 grad = new Vector3
            {
                x = GetDensity(x - chunkSize, y, z) - GetDensity(x + chunkSize, y, z),
                y = GetDensity(x, y - chunkSize, z) - GetDensity(x, y + chunkSize, z),
                z = GetDensity(x, y, z - chunkSize) - GetDensity(x, y, z + chunkSize)
            };

            return Vector3.Normalize(-grad);
        }

        public Vector3[] GetMesh()
        {
            List<Vector3> polygon = new List<Vector3>();
            List<Vector3> normal = new List<Vector3>();


            float[] v = new float[8];
            int[] edgeconnection = { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };
            Vector3[,] e = new Vector3[12, 2];

            for (int x = 0; x < chunkSize; x++)
            {
                for (int y = 0; y < chunkSize; y++)
                {
                    for (int z = 0; z < chunkSize; z++)
                    {

                        //calculate the density on each corner of the cube-cell
                        for (int i = 0; i < 8; i++)
                        {
                            Vector3 p = new Vector3(x, y, z) + Vertices[i];
                            p += offset;
                            v[i] = GetDensity(p.x, p.y, p.z);
                        }

                        //foreach(edge in cube-cell)
                        for (int i = 0; i < 12; i++)
                        {
                            //if(edge intersects surface)
                            if (v[Tables.EdgeConnection[i, 0]] < 0 != v[Tables.EdgeConnection[i, 1]] < 0)
                            {
                                //calculate intersection point and normal
                                e[i, 0] = Mix(Vertices[Tables.EdgeConnection[i, 0]], Vertices[Tables.EdgeConnection[i, 1]], -v[Tables.EdgeConnection[i, 0]] / (v[Tables.EdgeConnection[i, 1]] - v[Tables.EdgeConnection[i, 0]])) + new Vector3(x, y, z);
                                e[i, 1] = GetNormal(e[i, 0]);
                                normal.Add(GetNormal(e[i, 0]));
                            }
                        }

                        //foreach(face in cell-cube)
                        for (int f = 0; f < 6; f++)
                        {

                            //calculate the marching square case of the face
                            int corners = 0;
                            for (int i = 0; i < 4; i++)
                            {
                                if (v[Faces[f, 0, i]] < 0)
                                    corners |= 1 << i;
                            }

                            //foreach(line in case)
                            for (int i = 0; i < Connections[corners, 0]; i++)
                            {

                                //get the two edges that the line connects
                                int edge1 = Faces[f, 1, Connections[corners, 1 + 2 * i]];
                                int edge2 = Faces[f, 1, Connections[corners, 2 + 2 * i]];

                                //switch edge1 and edge2
                                if (f != 0)
                                {
                                    edge1 = edge1 ^ edge2;
                                    edge2 = edge1 ^ edge2;
                                    edge1 = edge1 ^ edge2;
                                }

                                //add the connection between the two edges to the per cell connection list
                                if (edgeconnection[edge1] == -1)
                                {
                                    edgeconnection[edge1] = edge2;
                                }
                                else
                                {
                                    // Console.WriteLine("Error [" + edge1 + "/" + edge2 + "][" + edge1 + "/" + edgeconnection[edge1] + "] at " + corners);
                                }

                            }

                        }

                        //foreach(edge in cell-cube)
                        for (int i = 0; i < 12; i++)
                        {
                            //if(edge has connection)
                            if (edgeconnection[i] != -1)
                            {
                                polygon.Clear();
                                int r = i;
                                //trace the polygon
                                while (edgeconnection[r] != -1)
                                {
                                    //OLD CODE: Vector3 pos = Mix(vertices[edges[r,0]], vertices[edges[r,1]], -v[edges[r,0]] / (v[edges[r,1]] - v[edges[r,0]])) + new Vector3(x, y, z);

                                    polygon.Add(e[r, 0]);
                                    int newref = edgeconnection[r];
                                    edgeconnection[r] = -1;
                                    r = newref;
                                }

                                for (int j = 0; j < polygon.Count - 2; j++)
                                {

                                    int vertexIndex = vertices.Count;

                                    vertices.Add(new Vector3(polygon[0].x, polygon[0].y, polygon[0].z));
                                    vertices.Add(new Vector3(polygon[1 + j].x, polygon[1 + j].y, polygon[1 + j].z));
                                    vertices.Add(new Vector3(polygon[2 + j].x, polygon[2 + j].y, polygon[2 + j].z));

                                    triangles.Add(vertexIndex + 2);
                                    triangles.Add(vertexIndex + 1);
                                    triangles.Add(vertexIndex);

                                }
                            }
                        }
                    }
                }
            }

            normals = normal;
            return new Vector3[3];
        }

        private Vector3 Mix(Vector3 a, Vector3 b, float s)
        {
            return a * (1 - s) + b * s;
        }

        protected override JobHandle StartMeshJob(JobHandle inputDeps = default)
        {
            throw new NotImplementedException();
        }

        private readonly Vector3[] Vertices = {
            new Vector3(0, 0, 0),
            new Vector3(0, 1, 0),
            new Vector3(1, 1, 0),
            new Vector3(1, 0, 0),
            new Vector3(0, 0, 1),
            new Vector3(0, 1, 1),
            new Vector3(1, 1, 1),
            new Vector3(1, 0, 1),
        };

        private readonly int[,] Connections = {
            {0,-1,-1,-1,-1},
            {1, 0, 3,-1,-1},
            {1, 1, 0,-1,-1},
            {1, 1, 3,-1,-1},
            {1, 2, 1,-1,-1},
            {2, 0, 1, 2, 3},
            {1, 2, 0,-1,-1},
            {1, 2, 3,-1,-1},
            {1, 3, 2,-1,-1},
            {1, 0, 2,-1,-1},
            {2, 3, 0, 1, 2},
            {1, 1, 2,-1,-1},
            {1, 3, 1,-1,-1},
            {1, 0, 1,-1,-1},
            {1, 3, 0,-1,-1},
            {0,-1,-1,-1,-1},
        };

        private readonly int[,,] Faces = {
            {{ 0, 1, 2, 3 }, { 0,  1,  2,  3}},
            {{ 0, 1, 5, 4 }, { 0,  9,  4,  8}},
            {{ 5, 1, 2, 6 }, { 9,  1, 10,  5}},
            {{ 7, 6, 2, 3 }, { 6, 10,  2, 11}},
            {{ 0, 4, 7, 3 }, { 8,  7, 11,  3}},
            {{ 4, 5, 6, 7 }, { 4,  5,  6,  7}}
        };

        private readonly Vector3[] Facenormals = {
            new Vector3( 0, 0,-1),
            new Vector3(-1, 0, 0),
            new Vector3( 0, 1, 0),
            new Vector3( 1, 0, 0),
            new Vector3( 0,-1, 0),
            new Vector3( 0, 0, 1),
        };
    }
}
