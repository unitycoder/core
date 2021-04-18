using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VoxelbasedCom
{
    /// <summary>
    /// It is a empty density
    /// </summary>
	public class None : Density 
	{
		public None()
		{
		}
		public override float GetDensity(float x, float y, float z)
		{
			return 1f;
		}
	}
}
