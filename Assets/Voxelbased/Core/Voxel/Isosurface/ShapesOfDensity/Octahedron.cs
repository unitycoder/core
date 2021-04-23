using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using static Unity.Mathematics.math;
//https://www.iquilezles.org/www/articles/distfunctions/distfunctions.htm

namespace VoxelbasedCom
{
	/// <summary>
	/// Octahedron density.
	/// </summary>
	public struct Octahedron : IDensity
	{
		private float3 center;
		private float size;

		public Octahedron(float3 center, float size)
		{
			this.center = center;
			this.size = size;
		}

		public float GetDensity(float3 pos) 
		{
			pos -= center;
			pos = abs(pos);
			return (pos.x + pos.y + pos.z - size) * 0.57735027f;
		}
	}
}
