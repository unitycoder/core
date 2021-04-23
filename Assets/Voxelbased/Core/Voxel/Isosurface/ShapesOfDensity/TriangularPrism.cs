using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using static Unity.Mathematics.math;

namespace VoxelbasedCom
{
	/// <summary>
	/// Triangular Prism density.
	/// </summary>
	public class TriangularPrism : IDensity
	{
		private float3 center;
		private float radius;

		public TriangularPrism(float3 center, float radius)
		{
			this.center = center;
			this.radius = radius;
		}

		public float GetDensity(float3 pos)
		{
			pos -= center;

			Vector3 q = abs(pos);
			return Mathf.Max(q.z - radius, Mathf.Max(q.x * 0.866025f + pos.y * 0.5f, -pos.y) - radius * 0.5f);
		}
	}
}
