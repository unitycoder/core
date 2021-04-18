using System.Collections;
using System.Collections.Generic;
using GraphProcessor;
using UnityEngine;
using VoxelGraph.Processor;

public class DensityGenerator : MonoBehaviour
{
    [Header("Graph to Run on Start")]
    public VoxelGraph.VoxelGraph graph;

    private VoxelGraphProcessor processor;

    private void Start()
    {
        if(graph != null)
            processor = new VoxelGraphProcessor(graph);

        // graph.SetParameterValue("Input", (float)i++);
        // graph.SetParameterValue("GameObject", assignedGameObject);
        // processor.Run();
        // Debug.Log("Output: " + graph.GetParameterValue("Output"));
        GenerateDensity(transform.position.x, transform.position.y, transform.position.z);
        Debug.Log(processor.OutputNode?.density);

    }

    private void GenerateDensity(float x, float y, float z)
    {
        graph.SetParameterValue("X", x);
        graph.SetParameterValue("Y", y);
        graph.SetParameterValue("Z", z);
        
        processor.Run();
    }
}
