using UnityEditor.GraphToolsFoundation.Overdrive;
using UnityEditor.GraphToolsFoundation.Overdrive.BasicModel;
using UnityEngine.UIElements;

namespace VoxelGraph.Editor
{
    public class VoxelGraphOnboardingProvider : OnboardingProvider
    {
        public override VisualElement CreateOnboardingElements(CommandDispatcher store)
        {
            var template = new VoxelGraphTemplate(VoxelGraphStencil.GraphName);
            return AddNewGraphButton<VoxelGraphAsset>(template);
        }
    }
}