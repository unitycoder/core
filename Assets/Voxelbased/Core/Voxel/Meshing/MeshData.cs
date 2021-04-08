using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

namespace VoxelbasedCom
{
    /// <summary>
    /// Data for mesh creation
    /// </summary>
    public class MeshData : IDisposable
    {
        public NativeArray<int> triangles;
        public NativeArray<Vector3> vertices;
        public Counter counter;
        public List<Vector3> normals;

        public void Dispose()
        {
            triangles.Dispose();
            vertices.Dispose();
            counter.Dispose();
        }
    }

    /// <summary>
    /// Hermite data structure
    /// </summary>
    public class HermiteData
    {
        public List<Vector3> intersectionPoints = new List<Vector3>();
        public List<Vector3> gradientVectors = new List<Vector3>();
    }

    /// <summary>
    /// Voxel data
    /// </summary>
    public struct Voxel
    {
        public Vector3 pos;
        public float density;
    }

    /// <summary>
    /// Vertex data
    /// </summary>
    public class Vertex
    {
        public int index = 0;
        public int edgeFlags = 0;
        public Vector3 pos = new Vector3();
        public Vector3 normal = new Vector3();
    }

    /// <summary>
    /// Triangle used to pass data to a mesh lists of new data.
    /// </summary>
	public class Triangle
    {
        private Vertex[] vertices = new Vertex[3];
        //private voxelType type;
        /// <summary>
        /// true 0, 1, 2 false 2, 1, 0
        /// </summary>
		public bool WindingOrder = true;

        #region Properties

        public Vertex this[int index]
        {
            get
            {
                return vertices[index];
            }
            set
            {
                vertices[index] = value;
            }
        }

        //public voxelType Type
        //{
        //    get
        //    {
        //        return type;
        //    }
        //    set
        //    {
        //        if (value != voxelType.Air || value != voxelType.Border)
        //            type = value;
        //    }
        //}

        #endregion

        public Triangle()
        {
            for (int i = 0; i < 3; i++)
            {
                vertices[i] = new Vertex();
            }
        }
    }

    /// <summary>
    /// Y-axis cut of hermite data
    ///  </summary>
    public class Row
    {
        public static int size;
        public Vector3 pos;
        public readonly Voxel[] points = new Voxel[size * size];
        public readonly Vertex[] vertices = new Vertex[size * size];
    }
}
