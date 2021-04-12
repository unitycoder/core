// Part of this code based on Sam Hogan tutorial
// His original repo: https://github.com/samhogan/Minecraft-Unity3D

using System;
using System.Collections.Generic;
using UnityEngine;

namespace VoxelbasedCom
{
	public class Boxel : MeshBuilder
	{
        List<Vector3> vertices;
        List<Vector3> normals;
        List<int> triangles;

		private float voxelSize = 1.0f;
		public Boxel(Isosurface isosurface, Vector3 offset, int chunkSize) : base(isosurface, offset, chunkSize)
		{
		}

		public override MeshData GenerateMeshData()
		{
			vertices = new List<Vector3>();
			normals = new List<Vector3>();
			triangles = new List<int>();

			 CreateBlocks();

			return new MeshData
			{
				vertices = vertices,
				normals = normals,
				triangles = triangles
			};
		}

		private void CreateBlocks()
		{
			for (int x = 0; x < chunkSize; x++)
			{
				for (int y = 0; y < chunkSize; y++)
				{
					for (int z = 0; z < chunkSize; z++)
					{
						Vector3 pos = new Vector3(x, y, z);
						pos += offset;
						float currentBlockDensity = GetDensity(pos.x, pos.y, pos.z);

						float nextxBlockDensity = GetDensity(pos.x + voxelSize, pos.y, pos.z);
						float nextyBlockDensity = GetDensity(pos.x, pos.y + voxelSize, pos.z);
						float nextzBlockDensity = GetDensity(pos.x, pos.y, pos.z + voxelSize);

						float prevxBlockDensity = GetDensity(pos.x - voxelSize, pos.y, pos.z);
						float prevyBlockDensity = GetDensity(pos.x, pos.y - voxelSize, pos.z);
						float prevzBlockDensity = GetDensity(pos.x, pos.y, pos.z - voxelSize);

						Vector3 blockPosition = new Vector3(x, y, z);
						int faceCount = 0;

						if (currentBlockDensity <= 0)
						{
							//X right
							if (nextxBlockDensity > 0)
							{
								vertices.Add(blockPosition + new Vector3(1, 0, 0));
								vertices.Add(blockPosition + new Vector3(1, 1, 0));
								vertices.Add(blockPosition + new Vector3(1, 1, 1));
								vertices.Add(blockPosition + new Vector3(1, 0, 1));
								normals.Add(Vector3.right);
								normals.Add(Vector3.right);
								normals.Add(Vector3.right);
								normals.Add(Vector3.right);
								faceCount++;
							}

							//Y top
							if (nextyBlockDensity > 0)
							{
								vertices.Add(blockPosition + new Vector3(0, 1, 0));
								vertices.Add(blockPosition + new Vector3(0, 1, 1));
								vertices.Add(blockPosition + new Vector3(1, 1, 1));
								vertices.Add(blockPosition + new Vector3(1, 1, 0));
								normals.Add(Vector3.up);
								normals.Add(Vector3.up);
								normals.Add(Vector3.up);
								normals.Add(Vector3.up);
								faceCount++;
							}

							//Z back
							if (nextzBlockDensity > 0)
							{
								vertices.Add(blockPosition + new Vector3(1, 0, 1));
								vertices.Add(blockPosition + new Vector3(1, 1, 1));
								vertices.Add(blockPosition + new Vector3(0, 1, 1));
								vertices.Add(blockPosition + new Vector3(0, 0, 1));
								normals.Add(Vector3.forward);
								normals.Add(Vector3.forward);
								normals.Add(Vector3.forward);
								normals.Add(Vector3.forward);
								faceCount++;
							}

							//X left
							if (prevxBlockDensity > 0)
							{
								vertices.Add(blockPosition + new Vector3(0, 0, 1));
								vertices.Add(blockPosition + new Vector3(0, 1, 1));
								vertices.Add(blockPosition + new Vector3(0, 1, 0));
								vertices.Add(blockPosition + new Vector3(0, 0, 0));
								normals.Add(Vector3.left);
								normals.Add(Vector3.left);
								normals.Add(Vector3.left);
								normals.Add(Vector3.left);
								faceCount++;
							}

							//Y bottom
							if (prevyBlockDensity > 0)
							{
								vertices.Add(blockPosition + new Vector3(0, 0, 0));
								vertices.Add(blockPosition + new Vector3(1, 0, 0));
								vertices.Add(blockPosition + new Vector3(1, 0, 1));
								vertices.Add(blockPosition + new Vector3(0, 0, 1));
								normals.Add(Vector3.down);
								normals.Add(Vector3.down);
								normals.Add(Vector3.down);
								normals.Add(Vector3.down);
								faceCount++;
							}

							//Z front
							if (prevzBlockDensity > 0)
							{
								vertices.Add(blockPosition + new Vector3(0, 0, 0));
								vertices.Add(blockPosition + new Vector3(0, 1, 0));
								vertices.Add(blockPosition + new Vector3(1, 1, 0));
								vertices.Add(blockPosition + new Vector3(1, 0, 0));
								normals.Add(Vector3.back);
								normals.Add(Vector3.back);
								normals.Add(Vector3.back);
								normals.Add(Vector3.back);
								faceCount++;
							}

							// create triangles
							int triangle = vertices.Count - 4 * faceCount;
							for (int i = 0; i < faceCount; i++)
							{
								triangles.AddRange(new int[] {
									triangle + i * 4,
									triangle + i * 4 + 1,
									triangle + i * 4 + 2,
									triangle + i * 4,
									triangle + i * 4 + 2,
									triangle + i * 4 + 3
								});
							}
						}
                    }
                }
			}
		}
	}
}