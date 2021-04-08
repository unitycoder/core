using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace VoxelbasedCom
{
	public class Torus : Density
	{
		private Vector3 center;
		private float radius;

		public Torus(Vector3 center, float rad)
		{
			this.center = center;
			this.radius = rad;
		}

		public override float GetDensity(float x, float y, float z)
		{
			x -= center.x;
			y -= center.y;
			z -= center.z;

			float sqr_dist = Mathf.Pow(radius - Mathf.Sqrt(Mathf.Pow(x, 2.0f) + Mathf.Pow(y, 2.0f)), 2) + Mathf.Pow(z, 2.0f) - 2.0f * radius;

			return sqr_dist;
		}
	}
}
