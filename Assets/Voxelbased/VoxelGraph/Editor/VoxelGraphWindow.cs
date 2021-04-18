using GraphProcessor;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using VoxelGraph.Editor.Toolbar;

namespace VoxelGraph.Editor
{
    public class VoxelGraphWindow : BaseGraphWindow
    {
        VoxelGraph			tmpGraph;
        VoxelGraphToolbarView	toolbarView;

        [MenuItem("Voxel Based/Voxel Graph")]
        public static BaseGraphWindow OpenWithTmpGraph()
        {
            var graphWindow = CreateWindow<VoxelGraphWindow>();

            // When the graph is opened from the window, we don't save the graph to disk
            graphWindow.tmpGraph = ScriptableObject.CreateInstance<VoxelGraph>();
            graphWindow.tmpGraph.hideFlags = HideFlags.HideAndDontSave;
            graphWindow.InitializeGraph(graphWindow.tmpGraph);

            graphWindow.Show();

            return graphWindow;
        }
        
        public static BaseGraphWindow OpenWithGraph(VoxelGraph graph)
        {
            var graphWindow = GetWindow<VoxelGraphWindow>();

            // When the graph is opened from the window
            graphWindow.tmpGraph = graph;
            graphWindow.tmpGraph.hideFlags = HideFlags.None;
            graphWindow.InitializeGraph(graphWindow.tmpGraph);

            graphWindow.Show();

            return graphWindow;
        }

        protected override void OnDestroy()
        {
            graphView?.Dispose();
            
            if (tmpGraph.hideFlags == HideFlags.HideAndDontSave)
                DestroyImmediate(tmpGraph);
        }

        protected override void InitializeWindow(BaseGraph graph)
        {
            titleContent = new GUIContent("Voxel Graph");

            if (graphView == null)
            {
                graphView = new VoxelGraphView(this);
                toolbarView = new VoxelGraphToolbarView(graphView);
                graphView.Add(toolbarView);
            }

            rootView.Add(graphView);
        }

        protected override void InitializeGraphView(BaseGraphView view)
        {
            // graphView.OpenPinned< ExposedParameterView >();
            // toolbarView.UpdateButtonStatus();
        }
        
        [OnOpenAsset(1)]
        public static bool OpenGraphAsset(int instanceId, int line)
        {
            var obj = EditorUtility.InstanceIDToObject(instanceId);
            if (obj is VoxelGraph)
            {
                string path = AssetDatabase.GetAssetPath(instanceId);
                var asset = AssetDatabase.LoadAssetAtPath<VoxelGraph>(path);
                if (asset == null)
                    return false;

                OpenWithGraph(asset);

                return true;
            }

            return false;
        }
    }
}