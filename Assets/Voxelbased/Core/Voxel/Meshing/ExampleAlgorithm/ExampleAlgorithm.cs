using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VoxelbasedCom
{
	public class ExampleAlgorithm : MeshBuilder
	{
		// you can declaration the variable here or on the GenerateMeshData() method depends on how many methods have to use
		// if you work with these only in GenerateMeshData() then enough to the declaration in this method
		List<Vector3> vertices;
		List<Vector3> normals;
		List<int> triangles;

		public ExampleAlgorithm(Isosurface isosurface, Vector3 offset, int chunkSize) : base(isosurface, offset, chunkSize)
		{
		}

		public override MeshData GenerateMeshData()
		{
			vertices = new List<Vector3>();
			normals = new List<Vector3>();
			triangles = new List<int>();

			// if our code short we do not need a different method,
			// but if we work lots of code we create another method like below:
			CreateISOSurface();

			// this will be return with calculated vvertices, normals and triangles
			return new MeshData
			{
				vertices = vertices,
				normals = normals,
				triangles = triangles
			};
		}

		private void CreateISOSurface()
        {
			//something here calculate which point in our shape
			// for example
			/*
			 
			for (int x = 0; x < chunkSize; x++)
			{
				for (int y = 0; y < chunkSize; y++)
				{
					for (int z = 0; z < chunkSize; z++)
					{
						Vector3 pos = new Vector3(x, y, z);
						pos += offset;
						float currentBlockDensity = GetDensity(pos.x, pos.y, pos.z);
					}
				}
			}

			*/

			// more caculation here

		}
	}
}
