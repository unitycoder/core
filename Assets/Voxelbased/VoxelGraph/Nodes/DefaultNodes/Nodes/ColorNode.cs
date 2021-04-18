using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;

namespace VoxelGraph.Nodes
{
	[System.Serializable, NodeMenuItem("Primitives/Color", typeof(VoxelGraph))]
	public class ColorNode : BaseNode
	{
		[Output(name = "Color"), SerializeField]
		new public Color color;

		public override string name => "Color";
	}
}