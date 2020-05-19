using UnityEngine;

namespace Jichaels.VRSDK
{

    [RequireComponent(typeof(LineRenderer))]
    public class JVRLaserController : VRControllerModule
    {

        [SerializeField] private LayerMask layerMask;
        [SerializeField] private float rayCastLength;
        [SerializeField] private float laserWidth;
        [SerializeField] private int maxBounces;
        [SerializeField] private LineRenderer lineRenderer;

        private RaycastHit _hit;

        private bool _drawRay;
        public IJVRLaserInteract SelectedLaserInteraction { get; private set; }
        private bool _isHovering;
        public Vector3 PointerPosition { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            lineRenderer.startColor = Color.red;
            lineRenderer.endColor = Color.red;
            lineRenderer.startWidth = laserWidth;
            lineRenderer.endWidth = laserWidth;
            lineRenderer.enabled = false;
            Initialized = true;
        }

        private void OnEnable()
        {
            HandController.VrControllerModule = this;
            lineRenderer.enabled = _drawRay;
        }

        private void OnDisable()
        {
            HandController.VrControllerModule = null;
            lineRenderer.enabled = false;
        }

        public void LateUpdate()
        {
            if (!_drawRay) return;

            lineRenderer.SetPosition(0, HandController.CurrentPositionWorld);

            lineRenderer.positionCount = 2;

            DrawRayRecursive(rayCastLength, 0, HandController.CurrentPositionWorld, HandController.Transform.forward);
        }

        private void DrawRayRecursive(float remainingDistance, int index, Vector3 startPos, Vector3 direction)
        {
            if (Physics.Raycast(startPos, direction, out _hit, remainingDistance, layerMask))
            {
                PointerPosition = _hit.point;
                remainingDistance -= Vector3.Distance(startPos, PointerPosition);

                IJVRLaserInteract jvrLaserInteract = _hit.collider.GetComponent<IJVRLaserInteract>();

                if (jvrLaserInteract != SelectedLaserInteraction)
                {
                    if(_isHovering) SelectedLaserInteraction.LaserHoverExit(this);
                    SelectedLaserInteraction = jvrLaserInteract;

                    if (SelectedLaserInteraction != null)
                    {
                        _isHovering = true;
                        SelectedLaserInteraction.LaserHoverEnter(this);
                        lineRenderer.startColor = Color.green;
                        lineRenderer.endColor = Color.green;
                    }
                    else
                    {
                        _isHovering = false;
                        lineRenderer.startColor = Color.red;
                        lineRenderer.endColor = Color.red;
                    }
                }
                else
                {
                    if(_isHovering) SelectedLaserInteraction.LaserHoverStay(this);
                }
            }
            else
            {
                if(_isHovering) SelectedLaserInteraction.LaserHoverExit(this);
                _isHovering = false;
                SelectedLaserInteraction = null;
                lineRenderer.startColor = Color.red;
                lineRenderer.endColor = Color.red;
                PointerPosition = startPos + remainingDistance * direction;
                remainingDistance = 0;
            }

            index++;
            lineRenderer.SetPosition(index, PointerPosition);

            if (index - 1 == maxBounces || remainingDistance < 0.1f)
            {
                return;
            }

            lineRenderer.positionCount++;
            DrawRayRecursive(remainingDistance, index, PointerPosition, Vector3.Reflect(direction, _hit.normal));
        }

        private void ToggleLaserPointer()
        {
            _drawRay = !_drawRay;
            lineRenderer.enabled = _drawRay;
            if (_drawRay)
            {
                lineRenderer.SetPosition(0, HandController.CurrentPositionWorld);
                lineRenderer.SetPosition(1, HandController.CurrentPositionWorld);
            }
            else
            {
                if(_isHovering) SelectedLaserInteraction.LaserHoverExit(this);
                SelectedLaserInteraction = null;
            }
        }


        public override void PrimaryAction()
        {
            if (!Initialized) return;
            
            if(!_isHovering) return;

            if (SelectedLaserInteraction.DisableInteraction) return;
            
            SelectedLaserInteraction.JVRLaserInteract(this);
        }

        public override void SecondaryAction()
        {
            if(Initialized) ToggleLaserPointer();
        }
    }
}