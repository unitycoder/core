using System.Linq;
using UnityEditor.GraphToolsFoundation.Overdrive;
using UnityEditor.GraphToolsFoundation.Overdrive.BasicModel;
using VoxelGraph.Editor.Nodes.Math;

namespace VoxelGraph.Editor.Nodes
{
    public class OutputNode : NodeModel
    {
        public VoxelGraph VoxelGraph { get; set; }
        
        public IPortModel Density { get; private set; }
        
        public float density
        {
            get
            {
                IPortModel leftPort = Density;
                if (leftPort == null) return 0;

                return GetValue(leftPort);
            }
        }

        public OutputNode()
        {
            Title = "Output";
        }
        
        protected float GetValue(IPortModel port)
        {
            if (port == null)
                return 0;
            var node = port.GetConnectedEdges().FirstOrDefault()?.FromPort.NodeModel;
            MathNode leftMathNode = node as MathNode;
            if (node is MathNode mathNode)
                return mathNode.Evaluate();
            else if (node is IVariableNodeModel varNode)
                return (float)varNode.VariableDeclarationModel.InitializationModel.ObjectValue;
            else if (node is IConstantNodeModel constNode)
                return (float)constNode.ObjectValue;
            else
                return (float)port.EmbeddedValue.ObjectValue;
        }
        
        protected override void OnDefineNode()
        {
            base.OnDefineNode();
            
            Density = this.AddDataInputPort<float>("density");
        }

        protected override void InitCapabilities()
        {
            base.InitCapabilities();
            
            this.SetCapability(UnityEditor.GraphToolsFoundation.Overdrive.Capabilities.Deletable, false);
        }
    }
}