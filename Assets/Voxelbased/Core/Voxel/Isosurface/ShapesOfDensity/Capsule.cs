using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VoxelbasedCom
{
	/// <summary>
	/// Capsule density.
	/// </summary>
	public class Capsule : Density
	{
		private Vector3 center;
		private float radius;

		public Capsule(Vector3 center, float radius)
		{
			this.center = center;
			this.radius = radius;
		}

		public override float GetDensity(float x, float y, float z)
		{
			x -= center.x;
			y -= center.y;
			z -= center.z;

			y = y / (radius - 6.0f);
			x = x / (radius - 6.0f);
			z = z / (radius - 6.0f);

			Vector3 s = new Vector3(x, y, z);
			Vector3 p1 = Vector3.zero + Vector3.up * (0.4f - 1.0f);
			Vector3 p2 = Vector3.zero - Vector3.up * (0.4f - 1.0f);
			float t = Vector3.Dot((s - p1), (p2 - p1).normalized);
			t = Mathf.Clamp01(t);
			Vector3 closestPoint = p1 + (p2 - p1).normalized * t;
			return (s - closestPoint).magnitude - 1.0f;

		}
	}
}
