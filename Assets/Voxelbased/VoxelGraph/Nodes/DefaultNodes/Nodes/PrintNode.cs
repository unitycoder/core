using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;

namespace VoxelGraph.Nodes
{
	[NodeMenuItem("Conditional/Print", typeof(VoxelGraph))]
	public class ConditionalPrintNode : LinearConditionalNode
	{
		[Input] public object obj;

		public override string name => "Print";
	}
}