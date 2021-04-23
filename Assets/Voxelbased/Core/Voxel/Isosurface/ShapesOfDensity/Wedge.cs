using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using static Unity.Mathematics.math;

namespace VoxelbasedCom
{
	/// <summary>
	/// Wedge density.
	/// </summary>
	public class Wedge : IDensity
	{
		private float3 center;
		private float radius;

		public Wedge(float3 center, float radius)
		{
			this.center = center;
			this.radius = radius;
		}

		public float GetDensity(float3 pos)
		{
			pos -= center;
			
			float3 q = abs(pos);
			return max(q.z - radius, max(pos.x + pos.y, max(-pos.x, -pos.y)) - radius * 0.5f);
		}
	}
}
