using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using static Unity.Mathematics.math;

namespace VoxelbasedCom
{
	/// <summary>
	/// Planet density.
	/// </summary>
	public struct Planet : IDensity
	{
		private float3 center;
		private float radius;

		public Planet(float3 center, float radius)
		{
			this.center = center;
			this.radius = radius;
		}

		public float GetDensity(float3 pos)
		{
			pos -= center;

			float len = length(pos);
			float radiusSqrt = pow(radius * 0.5f, 2);

			return len - radius;
		}
	}
}
