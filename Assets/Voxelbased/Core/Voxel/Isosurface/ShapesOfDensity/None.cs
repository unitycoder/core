using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using static Unity.Mathematics.math;


namespace VoxelbasedCom
{
    /// <summary>
    /// It is a empty density
    /// </summary>
	public struct None : IDensity 
	{
		public float GetDensity(float3 pos)
		{
			return 1f;
		}
	}
}
