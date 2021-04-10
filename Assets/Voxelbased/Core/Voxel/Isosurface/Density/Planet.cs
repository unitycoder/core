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
			float radiusSqrt = Mathf.Pow(radius, 2);
			float noise = PerlinNoise3D(x, y, z);

			return distance - radiusSqrt - noise;
		}

		public static float PerlinNoise3D(float x, float y, float z)
		{
			float xy = Mathf.PerlinNoise(x, y);
			float xz = Mathf.PerlinNoise(x, z);
			float yz = Mathf.PerlinNoise(y, z);
			float yx = Mathf.PerlinNoise(y, x);
			float zx = Mathf.PerlinNoise(z, x);
			float zy = Mathf.PerlinNoise(z, y);

			return (xy + xz + yz + yx + zx + zy) / 6 * 200;
		}
	}
}
