using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VoxelbasedCom
{
	public class Pyramid : Density
	{
		private Vector3 center;
		private float radius;

		public Pyramid(Vector3 center, float radius)
		{
			this.center = center;
			this.radius = radius;
		}

		public override float GetDensity(float x, float y, float z)
		{
			x -= center.x;
			y -= center.y;
			z -= center.z;

			y = y / (radius);
			x = x / (radius);
			z = z / (radius);

			float ROOT_3 = Mathf.Sqrt(3);

			float[,] planes = new float[,]
			{
				{-ROOT_3, ROOT_3, -ROOT_3},
				{-ROOT_3, ROOT_3, ROOT_3},
				{ROOT_3, ROOT_3, -ROOT_3},
				{ROOT_3, ROOT_3, ROOT_3}
			};

			float[,] planeOffsets = new float[,]
			{
				{0, 0, 0},
				{0, 0, 0},
				{0, 0, 0},
				{0, 0, 0}
			};

			return distanceFromConvexPlanes(planes, planeOffsets, x, y, z);

		}
	}
}
