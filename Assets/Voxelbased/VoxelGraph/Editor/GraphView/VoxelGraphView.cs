using UnityEditor.GraphToolsFoundation.Overdrive;

namespace VoxelGraph.Editor
{
    public class VoxelGraphView : GraphView
    {
        VoxelGraphViewWindow m_VoxelGraphViewWindow;

        public VoxelGraphViewWindow window
        {
            get { return m_VoxelGraphViewWindow; }
        }

        public VoxelGraphView(VoxelGraphViewWindow voxelGraphViewWindow, bool withWindowedTools, CommandDispatcher store) : base(voxelGraphViewWindow, store, "VoxelGraphView")
        {
            m_VoxelGraphViewWindow = voxelGraphViewWindow;
        }
    }
}