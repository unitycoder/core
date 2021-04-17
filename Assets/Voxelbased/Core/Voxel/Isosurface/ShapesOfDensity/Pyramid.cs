using System.Collections;
using System.Collections.Generic;
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

			return -Mathf.Min(radius * (0.5f - y / radius) - Mathf.Max(Mathf.Abs(x), Mathf.Abs(z)), radius / 2f - Mathf.Abs(y));
		}
	}
}
