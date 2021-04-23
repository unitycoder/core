using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using static Unity.Mathematics.math;

namespace VoxelbasedCom
{
	/// <summary>
	/// Torus density.
	/// </summary>
	public class Torus : IDensity
	{
		private float3 center;
		private float2 radii;

		public Torus(float3 center, float radii)
		{
			this.center = center;
			this.radii = radii;
		}

		public float GetDensity(float3 pos)
		{
			pos -= center;
			float2 q = float2(length(pos.xz) - radii.x, pos.y);
			return length(q) - radii.y;
		}
	}
}
