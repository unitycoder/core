using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VoxelbasedCom
{
	/// <summary>
	/// Heart density.
	/// </summary>
	public class Heart : Density
	{
		private Vector3 center;
		private float radius;

		public Heart(Vector3 center, float radius)
		{
			this.center = center;
			this.radius = radius;
		}

		public override float GetDensity(float x, float y, float z)
		{
			x -= center.x;
			y -= center.y;
			z -= center.z;

			y = y / radius;
			x = x / radius;
			z = z / radius;

			y *= 1.4f;
			z *= 1.4f;

			return Mathf.Pow(2f * x * x + y * y + 2f * z * z - 1, 3) - 0.1f * z * z * y * y * y - y * y * y * x * x;
		}
	}
}
