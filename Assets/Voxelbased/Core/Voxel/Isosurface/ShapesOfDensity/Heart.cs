using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using static Unity.Mathematics.math;

namespace VoxelbasedCom
{
	/// <summary>
	/// Heart density.
	/// </summary>
	public struct Heart : IDensity
	{
		private float3 center;
		private float size;

		public Heart(float3 center, float size)
		{
			this.center = center;
			this.size = size;
		}

		public float GetDensity(float3 pos)
		{
			pos -= center;
			pos = pos / size;
			pos.yz *= 1.4f;
			return Mathf.Pow(2f * pos.x * pos.x + pos.y * pos.y + 2f * pos.z * pos.z - 1, 3) - 0.1f * pos.z * pos.z * pos.y * pos.y * pos.y - pos.y * pos.y * pos.y * pos.x * pos.x;
		}
	}
}
