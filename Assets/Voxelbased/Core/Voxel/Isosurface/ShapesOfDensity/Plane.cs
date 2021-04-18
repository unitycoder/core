using UnityEngine;

namespace VoxelbasedCom
{
    /// <summary>
    /// Plane density.
    /// </summary>
	public class Plane : Density {

		private Vector3 center;
		private float radius;

		public Plane(Vector3 center, float radius)
		{
			this.center = center;
			this.radius = radius;
		}
		
		public override float GetDensity(float x, float y, float z)
		{
			return y - (center.y / 2);
		}
	}
}
