using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VoxelbasedCom
{
	/// <summary>
	/// Cylinder density.
	/// </summary>
	public class Cylinder : Density
	{
		private Vector3 center;
		private float radius;

		public Cylinder(Vector3 center, float rad)
		{
			this.center = center;
			this.radius = rad;
		}

		public override float GetDensity(float x, float y, float z)
		{
			x -= center.x;
			y -= center.y;
			z -= center.z;

			float l = new Vector2(x, z).magnitude;
			Vector2 v = new Vector2(l, y);
			Vector2 w = new Vector2(radius * 0.5f, radius);
			Vector2 d = Abs(v) - w;
			return Mathf.Min(Mathf.Max(d.x, d.y), 0.0f) + Max(d, 0.0f).magnitude;
		}
	}
}
