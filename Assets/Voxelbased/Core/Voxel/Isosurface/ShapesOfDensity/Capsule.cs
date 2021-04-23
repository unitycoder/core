using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

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

			float3 p1 = math.float3(0, 1, 0) * (0.4f - 1.0f);
			float3 p2 = 0.0f - math.float3(0, 1, 0) * (0.4f - 1.0f);
			float t = math.dot((pos - p1), math.normalize(p2 - p1));
			t = math.saturate(t);
			float3 closestPoint = p1 + math.normalize(p2 - p1) * t;
			return math.length(pos - closestPoint) - 1.0f;

		}
	}
}
