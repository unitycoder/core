using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VoxelbasedCom
{
	/// <summary>
	/// Octahedron density.
	/// </summary>
	public class Octahedron : Density
	{
		private Vector3 center;
		private float radius;

		public Octahedron(Vector3 center, float rad)
		{
			this.center = center;
			this.radius = rad;
		}

		public override float GetDensity(float x, float y, float z)
		{
			Vector3 p = new Vector3(x, y, z);

			p.x -= center.x;
			p.y -= center.y;
			p.z -= center.z;

			p = Abs(p);

			return (p.x + p.y + p.z - radius) * 0.57735027f;
		}
	}
}
