using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VoxelbasedCom
{
	/// <summary>
	/// Wedge density.
	/// </summary>
	public class Wedge : Density
	{
		private Vector3 center;
		private float radius;

		public Wedge(Vector3 center, float radius)
		{
			this.center = center;
			this.radius = radius;
		}

		public override float GetDensity(float x, float y, float z)
		{
			Vector3 p = new Vector3(x, y, z);

			p.x -= center.x;
			p.y -= center.y;
			p.z -= center.z;
			
			Vector3 q = Abs(p);
			return Mathf.Max(q.z - radius, Mathf.Max(p.x + p.y, Mathf.Max(-p.x, -p.y)) - radius * 0.5f);
		}
	}
}
