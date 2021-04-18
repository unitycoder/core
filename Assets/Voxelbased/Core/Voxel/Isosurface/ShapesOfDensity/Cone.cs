using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VoxelbasedCom
{
	/// <summary>
	/// Cone density.
	/// </summary>
	public class Cone : Density
	{
		private Vector3 center;
		private float radius;

		public Cone(Vector3 center, float radius)
		{
			this.center = center;
			this.radius = radius;
		}

		public override float GetDensity(float x, float y, float z)
		{
			x -= center.x;
			y -= center.y;
			z -= center.z;

			float height = radius;

			Vector2 q = new Vector2(new Vector2(x, z).magnitude, y);
			Vector2 k1 = new Vector2(0f, height);
			Vector2 k2 = new Vector2(-radius, 2f * height);
			Vector2 ca = new Vector2(q.x - Mathf.Min(q.x, q.y < 0f ? radius : 0f), Mathf.Abs(q.y) - height);
			Vector2 cb = q - k1 + k2 * Mathf.Clamp01(Vector2.Dot(k1 - q, k2) / k2.sqrMagnitude);
			float s = (cb.x < 0f && ca.y < 0f) ? -1f : 1f;
			return s * Mathf.Sqrt(Mathf.Min(ca.sqrMagnitude, cb.sqrMagnitude));;
		}
	}
}
