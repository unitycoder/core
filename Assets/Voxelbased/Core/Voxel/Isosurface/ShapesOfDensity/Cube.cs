using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using static Unity.Mathematics.math;
//https://www.iquilezles.org/www/articles/distfunctions/distfunctions.htm

namespace VoxelbasedCom
{
	/// <summary>
	/// Cube density.
	/// </summary>
	public struct Cube : IDensity
	{
		private float3 center;
		private float3 bounds;

		public Cube(float3 c, float3 bounds)
		{
			this.center = c;
			this.bounds = bounds;
		}

		public float GetDensity(float3 pos)
		{
			pos -= center;
			float3 q = abs(pos) - bounds;
			return length(max(q, 0.0f)) + min(max(q.x, max(q.y, q.z)), 0.0f);
		}
    }
}
