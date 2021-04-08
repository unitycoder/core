using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace VoxelbasedCom
{
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

			y = y / (radius - 0.0f);
			x = x / (radius - 0.0f);
			z = z / (radius - 0.0f);

			y *= 1.4f;
			z *= 1.4f;
			return Mathf.Pow(2f * x * x + y * y + 2f * z * z - 1, 3) - 0.1f * z * z * y * y * y - y * y * y * x * x;
			//return Mathf.Pow(2.0f * Mathf.Pow(x, 2) + 2.0f * Mathf.Pow(y, 2) + Mathf.Pow(z, 2) - 1.0f, 3) - 0.1f * Mathf.Pow(x,2) * Mathf.Pow(y, 3) - Mathf.Pow(z, 2) * Mathf.Pow(y, 3);

		}
	}
}
