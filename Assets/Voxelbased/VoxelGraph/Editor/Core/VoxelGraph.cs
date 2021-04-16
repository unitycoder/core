using System;
using System.Collections.Generic;
using UnityEditor.GraphToolsFoundation.Overdrive.BasicModel;
using VoxelGraph.Editor.Nodes.Math;

namespace VoxelGraph.Editor
{
    public class VoxelGraph : GraphModel
    {
        static private readonly List<MathNode> s_EmptyNodes = new List<MathNode>();

        public VoxelGraph()
        {
            StencilType = null;
        }

        public override Type DefaultStencilType => typeof(VoxelGraphStencil);
    }
}