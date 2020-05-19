using UnityEngine;

namespace Jichaels.VRSDK
{

    public class JVRDrawable : MonoBehaviour
    {
        private MeshRenderer _renderer;
        private MeshFilter _meshFilter;
        private MeshCollider _meshCollider;

        private void Awake()
        {
            _renderer = GetComponent<MeshRenderer>();
            _meshFilter = GetComponent<MeshFilter>();
            _meshCollider = GetComponent<MeshCollider>();
        }

        public void Initialize(Material material, Mesh mesh)
        {
            mesh.Optimize();
            _renderer.material = material;
            _meshFilter.mesh = mesh;
            _meshCollider.sharedMesh = mesh;
        }

    }
}