using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VoxelbasedCom
{
	/// <summary>
	/// Cube density.
	/// </summary>
	public class Cube : Density
	{
		private Vector3 center;
		private float radius;

		public Cube(Vector3 c, float radius)
		{
			this.center = c;
			this.radius = radius;
		}

		public override float GetDensity(float x, float y, float z)
		{
			float xt = x - center.x;
			float yt = y - center.y;
			float zt = z - center.z;
			
			float xd = (xt * xt) - radius * radius;
			float yd = (yt * yt) - radius * radius;
			float zd = (zt * zt) - radius * radius;
			float d;
			
			if(xd > yd)
				if(xd > zd)
					d = xd;
			else
				d = zd;
			else if(yd > zd)
				d = yd;
			else
				d = zd;
			
			return d;
		}
    }
}
