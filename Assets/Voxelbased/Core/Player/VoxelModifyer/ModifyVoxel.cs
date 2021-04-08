using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VoxelbasedCom
{
    public class ModifyVoxel : MonoBehaviour
    {
        public Voxelbased voxelbased;
        public Camera cam;
        public Shape shape;
        public float size = 3.0f;

        private List<Chunk> chunks;

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            chunks = new List<Chunk>();
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

                    Chunk c = hitInfo.transform.gameObject.GetComponent<Chunk>();
                  
                    if (c != null && !chunks.Contains(c))
                    {
                        chunks.Add(c);
                    }

                    //voxelbased.UpdateVoxel(hitInfo, hitInfo.point, chunks, "Add", shape, size, true);
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
                    Chunk c = hitInfo.transform.gameObject.GetComponent<Chunk>();
                    if (c != null && !chunks.Contains(c))
                    {
                        chunks.Add(c);
                    }
                    //voxelbased.UpdateVoxel(hitInfo, hitInfo.point, chunks, "Remove", shape, size, true);
                }
            }
        }
    }
}

