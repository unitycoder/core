using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;

namespace VoxelGraph.Nodes
{
	[System.Serializable, NodeMenuItem("String", typeof(VoxelGraph))]
	public class StringNode : BaseNode
	{
		[Output(name = "Out"), SerializeField] public string output;

		public override string name => "String";
	}
}