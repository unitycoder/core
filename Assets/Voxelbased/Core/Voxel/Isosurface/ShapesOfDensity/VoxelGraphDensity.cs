using GraphProcessor;
using UnityEngine;
using VoxelGraph.Extensions;
using VoxelGraph.Processor;

namespace VoxelbasedCom
{
    public class VoxelGraphDensity : Density
    {
        private Vector3 center;
        private VoxelGraphProcessor processor;
        private BaseGraph graph;

        public VoxelGraphDensity(Vector3 center, BaseGraph graph)
        {
            this.center = center;

            this.graph = graph.Clone();
            processor = new VoxelGraphProcessor(this.graph);
        }

        public override float GetDensity(float x, float y, float z)
        {
            x -= center.x;
            y -= center.y;
            z -= center.z;

            graph.SetParameterValue("X", x);
            graph.SetParameterValue("Y", y);
            graph.SetParameterValue("Z", z);
            
            processor.Run();
            return processor.OutputNode.density;
        }
    }
}