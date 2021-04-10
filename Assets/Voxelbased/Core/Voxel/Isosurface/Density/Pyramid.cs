using UnityEngine;

namespace VoxelbasedCom
{
	/// <summary>
	/// Pyramid density.
	/// </summary>
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

		public float distanceFromConvexPlanes(float[,] planes, float[,] planeOffsets, float x, float y, float z)
		{
			var maxDistance = -Mathf.Infinity;
			for (var i = 0; i < planes.GetLength(0); i++)
			{
				var x_ = x - planeOffsets[i, 0];
				var y_ = y - planeOffsets[i, 1];
				var z_ = z - planeOffsets[i, 2];

				var dotProduct = planes[i, 0] * x_ + planes[i, 1] * y_ + planes[i, 2] * z_;

				maxDistance = Mathf.Max(maxDistance, dotProduct);
			}

			return maxDistance;
		}
	}
}
