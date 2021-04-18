using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;

namespace VoxelGraph.Nodes
{
	[System.Serializable, NodeMenuItem("Primitives/Float", typeof(VoxelGraph))]
	public class FloatNode : BaseNode
	{
		[Output("Out")] public float output;

		[Input("In")] public float input;

		public override string name => "Float";

		protected override void Process() => output = input;
	}
}