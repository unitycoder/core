using UnityEditor.GraphToolsFoundation.Overdrive;
using UnityEditor.GraphToolsFoundation.Overdrive.BasicModel;
using UnityEngine;
using VoxelGraph.Editor.Nodes;

namespace VoxelGraph.Editor
{
    public class VoxelGraphTemplate : GraphTemplate<VoxelGraphStencil>
    {
        public VoxelGraphTemplate(string graphName = "Graph") : base(graphName)
        {
            
        }

        public override void InitBasicGraph(IGraphModel graphModel)
        {
            base.InitBasicGraph(graphModel);

            Debug.Log("TEST");
            graphModel.CreateNode(typeof(OutputNode), "Output", Vector2.zero);
        }
    }
}