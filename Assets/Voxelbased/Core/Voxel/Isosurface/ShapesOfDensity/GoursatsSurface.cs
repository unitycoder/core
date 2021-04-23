using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using static Unity.Mathematics.math;

namespace VoxelbasedCom
{
	public struct GoursatsSurface : IDensity
	{
		private float3 center;
		private float size;

		public GoursatsSurface(float3 center, float size)
		{
			this.center = center;
			this.size = size;
		}

        public float GetDensity(float3 pos)
        {
			pos -= center;
			pos /= size - 4.5f;
			float3 a = pow(pos, 4);
			return a.x + a.y + a.z - 1.5f * (pos.x * pos.x + pos.y * pos.y + pos.z * pos.z) + 1.0f;
		}
    }
}
