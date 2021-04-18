using GraphProcessor;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using VoxelGraph.Nodes;

namespace VoxelGraph
{
    [CreateAssetMenu(fileName = "New Voxel Graph", menuName = "Voxel Based/New Voxel Graph")]
    public class VoxelGraph : BaseGraph
    {
        public VoxelGraph() : base()
        {
            this.nodes.Add(BaseNode.CreateFromType<StartNode>(Vector2.zero));
            this.nodes.Add(BaseNode.CreateFromType<OutputNode>(new Vector2(400, 0)));
            
            GenerateFloatExposedParameter("X", 0);
            GenerateFloatExposedParameter("Y", 0);
            GenerateFloatExposedParameter("Z", 0);
        }

        protected virtual void GenerateFloatExposedParameter(string name, float value)
        {
            var exposedParameter = new FloatParameter();
            exposedParameter.Initialize(name, value);
            this.exposedParameters.Add(exposedParameter);
        }
    }
}