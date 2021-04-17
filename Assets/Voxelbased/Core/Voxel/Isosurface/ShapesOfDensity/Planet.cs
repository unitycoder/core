using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VoxelbasedCom
{
	/// <summary>
	/// Planet density.
	/// </summary>
	public class Planet : Density
	{
		private Vector3 center;
		private float radius;

		public Planet( Vector3 center, float radius)
		{
			this.center = center;
			this.radius = radius;
		}

		public override float GetDensity(float x, float y, float z)
		{
			x -= center.x;
			y -= center.y;
			z -= center.z;

			float distance = x * x + y * y + z * z;
			float radiusSqrt = Mathf.Pow(radius * 0.5f, 2);
			float noise = PerlinNoise3D(x, y, z) * 200;

			return distance - radiusSqrt - noise;
		}
	}
}
