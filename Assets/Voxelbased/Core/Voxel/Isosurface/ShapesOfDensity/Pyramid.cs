using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using static Unity.Mathematics.math;
//https://www.iquilezles.org/www/articles/distfunctions/distfunctions.htm

namespace VoxelbasedCom
{
	/// <summary>
	/// Pyramid density.
	/// </summary>
	public struct Pyramid : IDensity
	{
		private float3 center;
		private float size;
		private float height;

		public Pyramid(float3 center, float size, float height)
		{
			this.center = center;
			this.size = size;
			this.height = height;
		}

		public float GetDensity(float3 pos)
		{
			pos -= center;

			float m2 = height * height + 0.25f;

			pos.xz = abs(pos.xz) / size;
			pos.xz = (pos.z > pos.x) ? pos.zx : pos.xz;
			pos.xz -= 0.5f;

			float3 q = float3(pos.z, height * pos.y - 0.5f * pos.x, height * pos.x + 0.5f * pos.y);

			float s = max(-q.x, 0.0f);
			float t = clamp((q.y - 0.5f * pos.z) / (m2 + 0.25f), 0.0f, 1.0f);

			float a = m2 * (q.x + s) * (q.x + s) + q.y * q.y;
			float b = m2 * (q.x + 0.5f * t) * (q.x + 0.5f * t) + (q.y - m2 * t) * (q.y - m2 * t);

			float d2 = min(q.y, -q.x * m2 - q.y * 0.5f) > 0.0f ? 0.0f : min(a, b);

			return sqrt((d2 + q.z * q.z) / m2) * sign(max(q.z, -pos.y));
		}
	}
}
