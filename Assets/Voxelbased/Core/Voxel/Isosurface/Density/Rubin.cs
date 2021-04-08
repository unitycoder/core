using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace VoxelbasedCom
{
	public class Rubin : Density
	{
		private Vector3 center;
		private float radius;

		public Rubin(Vector3 center, float rad)
		{
			this.center = center;
			this.radius = rad;
		}

		public override float GetDensity(float x, float y, float z)
		{
			x -= center.x;
			y -= center.y;
			z -= center.z;

			y /= radius - 0.5f;
			x /= radius - 0.5f;
			z /= radius - 0.5f;

			return Mathf.Abs(x) + Mathf.Abs(y) + Mathf.Abs(z) - 1.0f;
		}
	}
}
