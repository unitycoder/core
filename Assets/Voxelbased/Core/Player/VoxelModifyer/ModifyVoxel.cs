using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VoxelbasedCom
{
    public class ModifyVoxel : MonoBehaviour
    {
        public GameObject voxel;
        public Camera cam;
        public Shape shape;
        [Range(1, 10)]
        public int size = 3;

        private Dictionary<Vector3, ModifyVertex> modifyVertex;
        private Voxelbased vb;

        private void Start()
        {
            vb = voxel.GetComponent<Voxelbased>();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            modifyVertex = new Dictionary<Vector3, ModifyVertex>();
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                VertexAdd();
            }

            if (Input.GetMouseButtonDown(1))
            {
                VertexRemove();
            }
        }

        void VertexAdd()
        {
            Ray ray = new Ray(cam.transform.position, cam.transform.forward);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo))
            {
                if (hitInfo.transform.tag == "Ground")
                {
                    UpdateVoxel(hitInfo, hitInfo.point, "Add", shape, size, true);
                }
            }
        }

        void VertexRemove()
        {
            Ray ray = new Ray(cam.transform.position, cam.transform.forward);
            RaycastHit hitInfo;

            if (Physics.Raycast(ray, out hitInfo))
            {
                //Debug.DrawLine(ray.origin, ray.origin + (ray.direction * hit.distance), Color.green, 2);
                if (hitInfo.transform.tag == "Ground")
                {
                    UpdateVoxel(hitInfo, hitInfo.point, "Remove", shape, size, true);
                }
            }
        }

        public void UpdateVoxel(RaycastHit hit, Vector3 hitPos, string progressType, Shape shapeType, float shapeSize, bool finish)
        {
            Vector3 pos = Vector3.zero;

            if (progressType == "Add")
            {
                pos.x = Mathf.CeilToInt(hitPos.x);
                pos.y = Mathf.CeilToInt(hitPos.y);
                pos.z = Mathf.CeilToInt(hitPos.z);
            }
            else if (progressType == "Remove")
            {
                pos.x = Mathf.FloorToInt(hitPos.x);
                pos.y = Mathf.FloorToInt(hitPos.y);
                pos.z = Mathf.FloorToInt(hitPos.z);
            }

            if (!modifyVertex.ContainsKey(pos))
            {
                modifyVertex.Add(pos, new ModifyVertex(pos, progressType, shapeType, shapeSize));
            }

            foreach (Transform child in voxel.transform)
            {
                Chunk chunk = child.GetComponent<Chunk>();

                var isosurface = new Isosurface(vb.shapeSelector, vb.density, vb.isosurfaceAlgorithm, modifyVertex);
                chunk.CreateChunk(isosurface, vb.chunkSize);
            }

        }
    }
}

