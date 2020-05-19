using System.Collections.Generic;
using UnityEngine;

namespace Jichaels.VRSDK
{

    public class JVRTrailDrawer : VRControllerModule
    {

        public JVRGrabbable LastDrawnObject { get; private set; }

        [SerializeField] private new Camera camera;
        [SerializeField] private TrailRenderer trailRenderer;
        [SerializeField] private JVRDrawable templateMain;
        [SerializeField] private JVRDrawable templateSub;
        [SerializeField] private Transform drawPosition;

        private List<TrailRenderer> _trails = new List<TrailRenderer>();
        private List<Material> _materials = new List<Material>();
        private Material _currentMaterial;
        private int _index;
        private bool _draw;
        private Mesh _mesh;

        protected override void Awake()
        {
            base.Awake();
            _currentMaterial = trailRenderer.sharedMaterial;
            Initialized = true;
        }
        
        private void OnEnable()
        {
            HandController.VrControllerModule = this;
        }

        private void OnDisable()
        {
            HandController.VrControllerModule = null;
        }

        private void Update()
        {
            if (!_draw) return;
            _trails[_trails.Count - 1].transform.position = drawPosition.position;
        }

        private void ToggleDraw()
        {
            _draw = !_draw;
            if (_draw) CreateTrail();
        }

        // TODO : pool trail
        private void CreateTrail()
        {
            _materials.Add(_currentMaterial);
            _trails.Add(Instantiate(trailRenderer));
            _trails[_index].transform.position = drawPosition.position;
            _trails[_index].material = _materials[_index];
            _trails[_index].Clear();
            _index++;
        }

        private void ClearTrails()
        {
            for (int i = 0; i < _trails.Count; i++)
            {
                Destroy(_trails[i].gameObject);
            }

            _trails.Clear();
            _materials.Clear();
            _index = 0;
        }

        private void BakeMesh()
        {
            if (_trails.Count == 0) return;

            _draw = false;

            Transform mainTransform = null;

            for (int i = 0; i < _index; i++)
            {
                _mesh = new Mesh();
                _trails[i].BakeMesh(_mesh, camera);
                JVRDrawable instance = Instantiate(i == 0 ? templateMain : templateSub);
                instance.Initialize(_materials[i], _mesh);
                Transform tmpTransform = instance.transform;

                if (i == 0)
                {
                    mainTransform = tmpTransform;
                    tmpTransform.position = Vector3.zero;
                }
                else
                {
                    tmpTransform.SetParent(mainTransform);
                    tmpTransform.localPosition = Vector3.zero;
                }
            }

            if (mainTransform == null)
            {
                Debug.LogError("mainTransform is null");
                return;
            }
                
            LastDrawnObject = mainTransform.GetComponent<JVRGrabbable>();

            ClearTrails();

        }

        public void SetMaterial(Material material)
        {
            _currentMaterial = material;
            if (_draw) CreateTrail();
        }

        public override void PrimaryAction()
        {
            if(Initialized) BakeMesh();
        }

        public override void SecondaryAction()
        {
            if(Initialized) ToggleDraw();
        }
    }

}