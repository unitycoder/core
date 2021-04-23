using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using static Unity.Mathematics.math;

namespace VoxelbasedCom
{
	public struct Sphere : IDensity
	{
		private float radius;
		private float3 center;
		public Sphere(float3 center, float radius)
		{
			this.center = center;
			this.radius = radius;
        }

		public float GetDensity(float3 pos)
		{
			pos -= center;
			return length(pos);
		}
	}
}
