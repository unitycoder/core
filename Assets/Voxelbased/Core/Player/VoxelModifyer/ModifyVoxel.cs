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
        public int shapeSize = 3;
        public OperationType operationType;

        private Dictionary<Vector3, BaseModification> modifyVertex;
        private Voxelbased vb;

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            vb = voxel.GetComponent<Voxelbased>();
            modifyVertex = new Dictionary<Vector3, BaseModification>();
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                ModifyChunks();
            }
        }

        void ModifyChunks()
        {
            Ray ray = new Ray(cam.transform.position, cam.transform.forward);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo))
            {
                if (hitInfo.transform.tag == "Ground")
                {
                    UpdateVoxel(hitInfo, hitInfo.point, operationType, shape, shapeSize, true);
                }
            }
        }

        public void UpdateVoxel(RaycastHit hit, Vector3 hitPos, OperationType operationType, Shape shapeType, float shapeSize, bool finish)
        {
            Vector3 pos = Vector3.zero;

            pos.x = Mathf.CeilToInt(hitPos.x);
            pos.y = Mathf.CeilToInt(hitPos.y);
            pos.z = Mathf.CeilToInt(hitPos.z);

            if (!modifyVertex.ContainsKey(pos))
            {
                modifyVertex.Add(pos, new BaseModification(pos, operationType, shapeType, shapeSize));
            }

            foreach (Transform child in voxel.transform)
            {
                Chunk chunk = child.GetComponent<Chunk>();

                Isosurface isosurface = new Isosurface(vb.shapeSelector, vb.density, vb.isosurfaceAlgorithm, modifyVertex);
                chunk.CreateChunk(isosurface, vb.chunkSize);
            }

        }
    }
}
