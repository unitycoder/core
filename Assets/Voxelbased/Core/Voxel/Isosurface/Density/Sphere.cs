using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VoxelbasedCom
{
	public class Sphere : Density
	{
		private Vector3 center;
		private float radius;


        public Sphere( Vector3 center, float radius)
		{
			this.center = center;
			this.radius = radius;
        }

		public override float GetDensity(float x, float y, float z)
		{
			x -= center.x;
			y -= center.y;
			z -= center.z;

			float sqr_dist = Mathf.Pow(x,2) + Mathf.Pow(y, 2) + Mathf.Pow(z, 2);
            float sqr_rad = Mathf.Pow(radius, 2);
			float d = sqr_dist - sqr_rad;
			return d;
		}
	}
}
