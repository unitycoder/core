using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using static Unity.Mathematics.math;

namespace VoxelbasedCom
{
	/// <summary>
	/// Cone density.
	/// </summary>
	public class Cone : IDensity
	{
		private float3 center;
		private float angle;
		private float height;

		public Cone(float3 center, float angle, float height)
		{
			this.center = center;
			this.angle = angle;
			this.height = height;
		}

        public float GetDensity(float3 pos)
        {
			pos -= center;
			//https://www.iquilezles.org/www/articles/distfunctions/distfunctions.htm
			float2 c = float2(sin(angle), cos(angle));
			float2 q = height * float2(c.x / c.y, -1.0f);

			float2 w = float2(length(pos.xz), pos.y);
			float2 a = w - q * clamp(dot(w, q) / dot(q, q), 0.0f, 1.0f);
			float2 b = w - q * float2(clamp(w.x / q.x, 0.0f, 1.0f), 1.0f);
			float k = sign(q.y);
			float d = min(dot(a, a), dot(b, b));
			float s = max(k * (w.x * q.y - w.y * q.x), k * (w.y - q.y));
			return sqrt(d) * sign(s);
		}
    }
}
