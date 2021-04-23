using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using static Unity.Mathematics.math;

namespace VoxelbasedCom
{
	/// <summary>
	/// Cylinder density.
	/// </summary>
	public class Cylinder : IDensity
	{
		private Vector3 center;
		private float radius;
		private float height;

		public Cylinder(Vector3 center, float radius, float height)
		{
			this.center = center;
			this.radius = radius;
			this.height = height;
		}

        public float GetDensity(float3 pos)
        {
			float2 d = abs(float2(length(pos.xz), pos.y)) - float2(height, radius);
			return min(max(d.x, d.y), 0.0f) + length(max(d, 0.0f));
        }
    }
}
