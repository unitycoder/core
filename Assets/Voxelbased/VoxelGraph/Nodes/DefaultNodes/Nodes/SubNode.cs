﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;

namespace VoxelGraph.Nodes
{
	[System.Serializable, NodeMenuItem("Operations/Sub", typeof(VoxelGraph))]
	public class SubNode : BaseNode
	{
		[Input(name = "A")] public float inputA;
		[Input(name = "B")] public float inputB;

		[Output(name = "Out")] public float output;

		public override string name => "Sub";

		protected override void Process()
		{
			output = inputA - inputB;
		}
	}
}