//-----------------------------------------------------------------------
// <copyright file="MarchingTetrahedra.cs" company="alia">
//     Copyright (c) alia. See LICENSE.md.
//     Conversion by mgear @ http://unitycoder.com/blog/2012/11/12/marching-tetrahedrons/
//     Bug fixes by alia
//
//     Just attach to a renderable game object.
//     Go grab testdata.js from
//     https://github.com/mikolalysenko/mikolalysenko.github.com/tree/master/Isosurface/js
//     for more interesting shapes.
//
//     original website: https://sites.google.com/site/aliadevlog/marching-tetrahedrons-for-unity3d-javascript
//	   original code: https://docs.google.com/viewer?a=v&pid=sites&srcid=ZGVmYXVsdGRvbWFpbnxhbGlhZGV2bG9nfGd4OjI0OTlkZDYzZGRmMGY4OA
// </copyright>
//-----------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;

namespace VoxelbasedCom
{
	public class MarchingTetrahedra : MeshBuilder
	{
        //private float[] res = new float[3];
        private float[] grid = new float[8];
        private int[] mtvectror = new int[3];
        private List<Vector3> vertices = new List<Vector3>();
        private List<int> triangles = new List<int>();
        private List<int[]> faces = new List<int[]>();
        private List<Vector3> normals = new List<Vector3>();
        private const float NormalSmoothing = 0;

        public MarchingTetrahedra(Isosurface isosurface, Vector3 offset, int chunkSize) : base(isosurface, offset, chunkSize + 1)
		{
		}

		public override bool GetMeshData(out MeshData meshData)
		{

			float[] value = CalculateDensities();
			MarchTetrahedra(value);

			for (var n = 0; n < faces.Count; n++)
			{
				if (faces[n].Length == 3)
				{
					// Triangle 1
					triangles.AddRange(faces[n]);

				}
				else
				{
					// Triangle 1
					triangles.Add(faces[n][0]);
					triangles.Add(faces[n][1]);
					triangles.Add(faces[n][2]);

					// Triangle 2
					triangles.Add(faces[n][2]);
					triangles.Add(faces[n][3]);
					triangles.Add(faces[n][0]);
				}
			}

            normals = NormalSolver.RecalculateNormals(triangles, vertices, NormalSmoothing);

            meshData = new MeshData
			{
				//vertices = vertices,
				//triangles = triangles,
				normals = normals
			};
            return true;
		}

		int interp(int i0, int i1)
		{
			var g0 = grid[i0];
			var g1 = grid[i1];
			int x0 = mtvectror[0];
			int x1 = mtvectror[1];
			int x2 = mtvectror[2];
			float[] v = new float[3] { x0, x1, x2 };
			float t = g0 - g1;

			if (Mathf.Abs(t) > 1e-6)
			{
				t = g0 / t;
			}
			for (var i = 0; i < 3; ++i)
			{
				v[i] += Tables.VertexOffset[i0, i] + t * (Tables.VertexOffset[i1, i] - Tables.VertexOffset[i0, i]);
			}

            vertices.Add(new Vector3(v[0], v[1], v[2]));
			return vertices.Count - 1;
		}

        private float[] CalculateDensities()
        {
            var voxels = new float[chunkSize, chunkSize, chunkSize];
            float[] volume = new float[chunkSize * chunkSize * chunkSize];
            int n = 0;

            for (int z = 0; z < chunkSize; ++z) //z
            {
                for (int y = 0; y < chunkSize; ++y) // y
                {
                    for (int x = 0; x < chunkSize; ++x) // x
                    {

                        Vector3 pos = new Vector3(x, y, z);
                        pos += offset;
                        volume[n] = GetDensity(pos.x, pos.y, pos.z);

                        ++n;
                    }
                }
            }
            return volume;
        }

        private void MarchTetrahedra(float[] data)
		{
			int[,] tetra_list = { { 0, 2, 3, 7 }, { 0, 6, 2, 7 }, { 0, 4, 6, 7 }, { 0, 6, 1, 2 }, { 0, 1, 6, 4 }, { 5, 6, 1, 4 } };

			float n = 0;
			int[] edges = new int[12];

            //March over the volume
            for (int x = 0; x < chunkSize - 1; x++)
            {
                for (int y = 0; y < chunkSize - 1; y++)
                {
                    for (int z = 0; z < chunkSize - 1; z++)
                    {
                        mtvectror[2] = x;
                        mtvectror[1] = y;
                        mtvectror[0] = z;

                        //Read in cube
                        for (var i = 0; i < 8; ++i)
						{
							//grid[i] = data[n + cube_vertices[i, 0] + dims[0] * (cube_vertices[i, 1] + dims[1] * cube_vertices[i, 2])];
							int f = (int)(n) + Tables.VertexOffset[i, 0] + (int)(chunkSize) * (Tables.VertexOffset[i, 1] + (int)(chunkSize) * Tables.VertexOffset[i, 2]);
							grid[i] = data[f];
						}

						for (int i = 0; i < tetra_list.GetLength(0); ++i)
						{
							int triindex = 0;
							if (grid[tetra_list[i, 0]] < 0) triindex |= 1;
							if (grid[tetra_list[i, 1]] < 0) triindex |= 2;
							if (grid[tetra_list[i, 2]] < 0) triindex |= 4;
							if (grid[tetra_list[i, 3]] < 0) triindex |= 8;

							//Handle each case
							switch (triindex)
							{
								case 0x00:
								case 0x0F:
									break;
								case 0x0E:
									faces.Add(new int[3] { interp(tetra_list[i, 0], tetra_list[i, 1]), interp(tetra_list[i, 0], tetra_list[i, 3]), interp(tetra_list[i, 0], tetra_list[i, 2]) });
									break;
								case 0x01:
									faces.Add(new int[3] { interp(tetra_list[i, 0], tetra_list[i, 1]), interp(tetra_list[i, 0], tetra_list[i, 2]), interp(tetra_list[i, 0], tetra_list[i, 3]) });
									break;
								case 0x0D:
									faces.Add(new int[3] { interp(tetra_list[i, 1], tetra_list[i, 0]), interp(tetra_list[i, 1], tetra_list[i, 2]), interp(tetra_list[i, 1], tetra_list[i, 3]) });
									break;
								case 0x02:
									faces.Add(new int[3] { interp(tetra_list[i, 1], tetra_list[i, 0]), interp(tetra_list[i, 1], tetra_list[i, 3]), interp(tetra_list[i, 1], tetra_list[i, 2]) });
									break;
								case 0x0C:
									faces.Add(new int[4] { interp(tetra_list[i, 1], tetra_list[i, 2]), interp(tetra_list[i, 1], tetra_list[i, 3]), interp(tetra_list[i, 0], tetra_list[i, 3]), interp(tetra_list[i, 0], tetra_list[i, 2]) });
									break;
								case 0x03:
									faces.Add(new int[4] { interp(tetra_list[i, 1], tetra_list[i, 2]), interp(tetra_list[i, 0], tetra_list[i, 2]), interp(tetra_list[i, 0], tetra_list[i, 3]), interp(tetra_list[i, 1], tetra_list[i, 3]) });
									break;
								case 0x04:
									faces.Add(new int[3] { interp(tetra_list[i, 2], tetra_list[i, 0]), interp(tetra_list[i, 2], tetra_list[i, 1]), interp(tetra_list[i, 2], tetra_list[i, 3]) });
									break;
								case 0x0B:
									faces.Add(new int[3] { interp(tetra_list[i, 2], tetra_list[i, 0]), interp(tetra_list[i, 2], tetra_list[i, 3]), interp(tetra_list[i, 2], tetra_list[i, 1]) });
									break;
								case 0x05:
									faces.Add(new int[4] { interp(tetra_list[i, 0], tetra_list[i, 1]), interp(tetra_list[i, 1], tetra_list[i, 2]), interp(tetra_list[i, 2], tetra_list[i, 3]), interp(tetra_list[i, 0], tetra_list[i, 3]) });
									break;
								case 0x0A:
									faces.Add(new int[4] { interp(tetra_list[i, 0], tetra_list[i, 1]), interp(tetra_list[i, 0], tetra_list[i, 3]), interp(tetra_list[i, 2], tetra_list[i, 3]), interp(tetra_list[i, 1], tetra_list[i, 2]) });
									break;
								case 0x06:
									faces.Add(new int[4] { interp(tetra_list[i, 2], tetra_list[i, 3]), interp(tetra_list[i, 0], tetra_list[i, 2]), interp(tetra_list[i, 0], tetra_list[i, 1]), interp(tetra_list[i, 1], tetra_list[i, 3]) });
									break;
								case 0x09:
									faces.Add(new int[4] { interp(tetra_list[i, 2], tetra_list[i, 3]), interp(tetra_list[i, 1], tetra_list[i, 3]), interp(tetra_list[i, 0], tetra_list[i, 1]), interp(tetra_list[i, 0], tetra_list[i, 2]) });
									break;
								case 0x07:
									faces.Add(new int[3] { interp(tetra_list[i, 3], tetra_list[i, 0]), interp(tetra_list[i, 3], tetra_list[i, 1]), interp(tetra_list[i, 3], tetra_list[i, 2]) });
									break;
								case 0x08:
									faces.Add(new int[3] { interp(tetra_list[i, 3], tetra_list[i, 0]), interp(tetra_list[i, 3], tetra_list[i, 2]), interp(tetra_list[i, 3], tetra_list[i, 1]) });
									break;
							}
						}
						++n;
					}
					++n;
				}
				n += chunkSize;
			}
		}

        protected override JobHandle OnMeshJobScheduled(JobHandle inputDeps = default)
        {
            throw new System.NotImplementedException();
        }
    }
}