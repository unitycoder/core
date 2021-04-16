using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.GraphToolsFoundation.Overdrive;
using UnityEngine;

namespace VoxelGraph.Editor
{
    public class VoxelGraphViewWindow : GraphViewEditorWindow
    {
        [MenuItem("Voxel Based/Voxel Graph")]
        public static void ShowWindow()
        {
            GetWindow<VoxelGraphViewWindow>();
        }
        
        protected override void OnEnable()
        {
            base.OnEnable();

            EditorToolName = "Voxel Graph";
        }
        
        protected override GraphView CreateGraphView()
        {
            return new VoxelGraphView(this, true, CommandDispatcher);
        }

        protected override BlankPage CreateBlankPage()
        {
            var onboardingProviders = new List<OnboardingProvider>();
            onboardingProviders.Add(new VoxelGraphOnboardingProvider());

            return new BlankPage(CommandDispatcher, onboardingProviders);
        }

        protected override bool CanHandleAssetType(GraphAssetModel asset)
        {
            return asset is VoxelGraphAsset;
        }
    }
}
