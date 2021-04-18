using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;

namespace VoxelGraph.Nodes
{
	[System.Serializable, NodeMenuItem("Primitives/Vector", typeof(VoxelGraph))]
	public class VectorNode : BaseNode
	{
		[Output(name = "Out")] public Vector4 output;

		[Input(name = "In"), SerializeField] public Vector4 input;

		public override string name => "Vector";

		protected override void Process()
		{
			output = input;
		}
	}
}