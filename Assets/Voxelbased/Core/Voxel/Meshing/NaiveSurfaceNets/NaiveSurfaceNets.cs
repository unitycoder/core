//-----------------------------------------------------------------------
// <copyright file="NativeSurfaceNets.cs" company="Tomasz Foster, Andrew Gotow">
//     Copyright (c) Tomasz Foster and Andrew Gotow. See LICENSE.md.
//     original repo: https://github.com/TomaszFoster/NaiveSurfaceNets
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;

namespace VoxelbasedCom
{
	public class NaiveSurfaceNets : MeshBuilder
	{
		private List<Vector3> vertices = new List<Vector3>();
		private List<int> triangles = new List<int>();
		private List<int[]> faces = new List<int[]>();
		private List<Vector3> normals = new List<Vector3>();
		private const float NormalSmoothing = 0;

		public float threshold = 0.0f;

		private int[] buffer;

		public struct Voxel2
		{
			public int index;
			public Vector3 pos;

			public int edgeFlags;
		}

        private Voxel2[,,] voxels;

		private List<Vector3> newVertices;

		private int[] edgeTable = new int[256];
		private int[] cubeEdges = new int[24];


		private Vector3[] voxelCornerOffsets = new Vector3[8] {
		new Vector3(0,0,0),		// 0
		new Vector3(1,0,0), 	// 1
		new Vector3(0,1,0), 	// 2
		new Vector3(1,1,0), 	// 3
		new Vector3(0,0,1), 	// 4
		new Vector3(1,0,1), 	// 5
		new Vector3(0,1,1), 	// 6
		new Vector3(1,1,1)  	// 7
		};

		public NaiveSurfaceNets(Isosurface isosurface, Vector3 offset, int chunkSize) : base(isosurface, offset, chunkSize + 1)
		{
		}

		/*public override bool GetMeshData(out MeshData meshData)
		{
			GenerateCubeEdgesTable();
			GenerateIntersectionTable();
			GenerateVertex();
			ComputeMesh();

			List<Vector3> normals = NormalSolver.RecalculateNormals(triangles, newVertices, NormalSmoothing);

			meshData = new MeshData
			{
				//vertices = newVertices,
				//triangles = triangles,
				normals = normals
			};
            return true;
		}*/

		void GenerateIntersectionTable()
		{

			for (int i = 0; i < 256; ++i)
			{
				int em = 0;
				for (int j = 0; j < 24; j += 2)
				{
					var a = Convert.ToBoolean(i & (1 << cubeEdges[j]));
					var b = Convert.ToBoolean(i & (1 << cubeEdges[j + 1]));
					em |= a != b ? (1 << (j >> 1)) : 0;
				}
				edgeTable[i] = em;
			}
		}

		void GenerateCubeEdgesTable()
		{
			int k = 0;
			for (int i = 0; i < 8; ++i)
			{
				for (int j = 1; j <= 4; j <<= 1)
				{
					int p = i ^ j;
					if (i <= p)
					{
						cubeEdges[k++] = i;
						cubeEdges[k++] = p;
					}
				}
			}
		}

		private void GenerateVertex()
		{

			voxels = new Voxel2[chunkSize, chunkSize, chunkSize];

			for (int x = 0; x < chunkSize; x++)
			{
				for (int y = 0; y < chunkSize; y++)
				{
					for (int z = 0; z < chunkSize; z++)
					{

						voxels[x, y, z].edgeFlags = 0;

						int cornerMask = 0;

						Voxel[] samples = new Voxel[8];
						for (int i = 0; i < 8; i++)
						{
							var offset2 = voxelCornerOffsets[i];
							var pos = new Vector3((x + (int)offset2.x - 0.5f) * 1.0f, (y + (int)offset2.y - 0.5f) * 1.0f, (z + (int)offset2.z - 0.5f)* 1.0f);
							var pos2 = new Vector3(pos.x + offset.x, pos.y + offset.y, pos.z + offset.z);
							var density = GetDensity(pos2.x, pos2.y, pos2.z);
							samples[i].density = density;
							samples[i].pos = pos;
							cornerMask |= ((density > threshold) ? (1 << i) : 0);
						}

						if (cornerMask == 0 || cornerMask == 0xff)
						{
							continue;
						}

						int edgeMask = edgeTable[cornerMask];
						int edgeCrossings = 0;
						var vertPos = Vector3.zero;

						for (int i = 0; i < 12; ++i)
						{

							if (!((edgeMask & (1 << i)) > 0))
							{
								continue;
							}

							++edgeCrossings;

							int e0 = cubeEdges[i << 1];
							int e1 = cubeEdges[(i << 1) + 1];
							float g0 = samples[e0].density;
							float g1 = samples[e1].density;
							float t = (threshold - g0) / (g1 - g0);

							vertPos += Vector3.Lerp(samples[e0].pos, samples[e1].pos, t);
						}
						vertPos /= edgeCrossings;

						voxels[x, y, z].pos = vertPos;
						voxels[x, y, z].edgeFlags = cornerMask;

					}
				}
			}
		}


		public void ComputeMesh()
		{
			newVertices = new List<Vector3>();
			// set the size of our buffer
			buffer = new int[4096];

			// get the width, height, and depth of the sample space for our nested for loops
			int width = chunkSize + 1;
			int height = chunkSize + 1;
			int depth = chunkSize + 1;

			int n = 0;
			int[] pos = new int[3];
			int[] R = new int[] { 1, width + 1, (width + 1) * (height + 1) };
			float[] grid = new float[8];
			int bufferNumber = 1;

			// resize the buffer if it's not big enough
			if (R[2] * 2 > buffer.Length)
				buffer = new int[R[2] * 2];

			for (pos[2] = 0; pos[2] < chunkSize; pos[2]++, n += width, bufferNumber ^= 1, R[2] = -R[2])
			{
				var bufferIndex = 1 + (width + 1) * (1 + bufferNumber * (height + 1));

				for (pos[1] = 0; pos[1] < chunkSize; pos[1]++, n++, bufferIndex += 2)
				{
					for (pos[0] = 0; pos[0] < chunkSize; pos[0]++, n++, bufferIndex++)
					{
						// get the corner mask we calculated earlier

						var mask = voxels[pos[0], pos[1], pos[2]].edgeFlags;
						
						// Early Termination Check
						if (mask == 0 || mask == 0xff)
						{
							continue;
						}

						// get edge mask
						var edgeMask = edgeTable[mask];

						var vertex = new Vector3();
						var edgeIndex = 0;

						//For Every Cube Edge
						for (var i = 0; i < 12; i++)
						{
							//Use Edge Mask to Check if Crossed
							if (!Convert.ToBoolean(edgeMask & (1 << i)))
							{
								continue;
							}

							//If So, Increment Edge Crossing #
							edgeIndex++;

							//Find Intersection Point
							var e0 = cubeEdges[i << 1];
							var e1 = cubeEdges[(i << 1) + 1];
							var g0 = grid[e0];
							var g1 = grid[e1];
							var t = g0 - g1;
							if (Math.Abs(t) > 1e-16)
								t = g0 / t;
							else
								continue;

							//Interpolate Vertices, Add Intersections
							for (int j = 0, k = 1; j < 3; j++, k <<= 1)
							{
								var a = e0 & k;
								var b = e1 & k;
								if (a != b)
									vertex[j] += Convert.ToBoolean(a) ? 1f - t : t;
								else
									vertex[j] += Convert.ToBoolean(a) ? 1f : 0;
							}
						}

						//Average Edge Intersections, Add to Coordinate
						var s = 1f / edgeIndex;
						for (var i = 0; i < 3; i++)
						{
							vertex[i] = pos[i] + s * vertex[i];
						}

						vertex = voxels[pos[0], pos[1], pos[2]].pos;

						//Add Vertex to Buffer, Store Pointer to Vertex Index
						buffer[bufferIndex] = newVertices.Count;
						newVertices.Add(vertex);

						//Add Faces (Loop Over 3 Base Components)
						for (var i = 0; i < 3; i++)
						{
							//First 3 Entries Indicate Crossings on Edge
							if (!Convert.ToBoolean(edgeMask & (1 << i)))
							{
								continue;
							}

							//i - Axes, iu, iv - Ortho Axes
							var iu = (i + 1) % 3;
							var iv = (i + 2) % 3;

							//Skip if on Boundary
							if (pos[iu] == 0 || pos[iv] == 0)
								continue;

							//Otherwise, Look Up Adjacent Edges in Buffer
							var du = R[iu];
							var dv = R[iv];

							//Flip Orientation Depending on Corner Sign
							if (Convert.ToBoolean(mask & 1))
							{
								triangles.Add(buffer[bufferIndex]);
								triangles.Add(buffer[bufferIndex - du - dv]);
								triangles.Add(buffer[bufferIndex - du]);
								triangles.Add(buffer[bufferIndex]);
								triangles.Add(buffer[bufferIndex - dv]);
								triangles.Add(buffer[bufferIndex - du - dv]);
							}
							else
							{
								triangles.Add(buffer[bufferIndex]);
								triangles.Add(buffer[bufferIndex - du - dv]);
								triangles.Add(buffer[bufferIndex - dv]);
								triangles.Add(buffer[bufferIndex]);
								triangles.Add(buffer[bufferIndex - du]);
								triangles.Add(buffer[bufferIndex - du - dv]);
							}
						}
					}
				}
			}
		}

        protected override JobHandle StartMeshJob(JobHandle inputDeps = default)
        {
            throw new NotImplementedException();
        }
    }
}
