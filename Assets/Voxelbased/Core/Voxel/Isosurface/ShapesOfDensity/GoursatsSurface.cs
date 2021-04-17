using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VoxelbasedCom
{
	public class GoursatsSurface : Density
	{
		private Vector3 center;
		private float radius;

		public GoursatsSurface(Vector3 center, float rad)
		{
			this.center = center;
			this.radius = rad;
		}

		public override float GetDensity(float x, float y, float z)
		{
			x -= center.x;
			y -= center.y;
			z -= center.z;

			y /= radius - 4.5f;
			x /= radius - 4.5f;
			z /= radius - 4.5f;

			return Mathf.Pow(x, 4) + Mathf.Pow(y, 4) + Mathf.Pow(z, 4) - 1.5f * (x * x + y * y + z * z) + 1.0f;
		}
	}
}
