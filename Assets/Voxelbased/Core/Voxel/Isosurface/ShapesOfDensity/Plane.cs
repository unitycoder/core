using UnityEngine;
using Unity.Mathematics;
using static Unity.Mathematics.math;

namespace VoxelbasedCom
{
    /// <summary>
    /// Plane density.
    /// </summary>
	public struct Plane : IDensity 
	{
		private Vector3 center;
		private float radius;

		public Plane(Vector3 center, float radius)
		{
			this.center = center;
			this.radius = radius;
		}
		
		public float GetDensity(float3 pos)
		{
			return pos.y - (center.y / 2);
		}
	}
}
