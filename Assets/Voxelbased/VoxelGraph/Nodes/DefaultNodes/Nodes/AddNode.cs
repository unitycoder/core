using GraphProcessor;

namespace VoxelGraph.Nodes
{
    [System.Serializable, NodeMenuItem("Operations/Add", typeof(VoxelGraph))]
    public class AddNode : BaseNode
    {
        [Input(name = "A")] public float inputA;
        [Input(name = "B")] public float inputB;

        [Output(name = "Out")] public float output;

        public override string name => "Add";

        protected override void Process()
        {
            output = inputA + inputB;
        }
    }
}