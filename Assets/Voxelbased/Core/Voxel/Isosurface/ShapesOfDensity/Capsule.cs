using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using static Unity.Mathematics.math;

namespace VoxelbasedCom
{
	/// <summary>
	/// Capsule density.
	/// </summary>
	public class Capsule : IDensity
	{
		private float3 center;
		private float radius;

		public Capsule(float3 center, float radius)
		{
			this.center = center;
			this.radius = radius;
		}

        public float GetDensity(float3 pos)
        {
			pos -= center;
			pos = pos / (radius - 6.0f);

			float3 p1 = float3(0, 1, 0) * (0.4f - 1.0f);
			float3 p2 = 0.0f - float3(0, 1, 0) * (0.4f - 1.0f);
			float t = dot((pos - p1), normalize(p2 - p1));
			t = saturate(t);
			float3 closestPoint = p1 + normalize(p2 - p1) * t;
			return length(pos - closestPoint) - 1.0f;

		}
	}
}
