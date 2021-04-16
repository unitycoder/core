using System;
using UnityEditor.GraphToolsFoundation.Overdrive;

namespace VoxelGraph.Editor.Nodes.Math
{
    [Serializable]
    [SearcherItem(typeof(VoxelGraphStencil), SearcherContext.Graph, "Math/Operators/Addition")]
    public class MathAdditionOperator : MathOperator
    {
        public MathAdditionOperator()
        {
            Title = "Add";
        }

        public override float Evaluate()
        {
            return left + right;
        }
    }
}