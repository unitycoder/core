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
		private Quaternion rotation = Quaternion.identity;

		public Cube(Vector3 c, float rad)
		{
			this.center = c;
			this.radius = rad;
		}

		public Cube(Vector3 c, float rad, Quaternion rot)
		{
			this.center = c;
			this.radius = rad;
			this.rotation = rot;
			//IsRotated = true;
		}

		public override float GetDensity(float x, float y, float z)
		{
			Vector3 p = new Vector3(x, y, z);

			//if(IsRotated)
			//	p = RotatePointAroundPivot(p, center, Quaternion.Inverse(rotation));
			
			float xt = p.x - center.x;
			float yt = p.y - center.y;
			float zt = p.z - center.z;
			
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

        public Vector3 RotatePointAroundPivot(Vector3 point, Vector3 axis, Quaternion angles)
        {
            Vector3 dir = point - axis;
            dir = angles * dir;
            point = dir + axis;

            return point;
        }
    }
}