using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;

namespace VoxelGraph.Nodes
{
	[System.Serializable, NodeMenuItem("Voxel Graph/Output", typeof(VoxelGraph))]
	public class OutputNode : LinearConditionalNode
	{
		[Input(name = "Density")] public float density;

		public override string name => "Output";

		public override bool deletable => false;

		protected override void Process()
		{
			// Do stuff
		}
	}
}