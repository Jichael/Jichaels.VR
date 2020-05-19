using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Jichaels.VRSDK
{

    public class JVRHandController : MonoBehaviour, IInteractionController
    {
        public JVRPlayer Player { get; private set; }
        public Transform Transform { get; private set; }
        public VRControllerModule VrControllerModule { get; set; }
        public bool HasModule { get; private set; }
        public Vector3 CurrentPositionWorld { get; private set; }
        public Vector3 CurrentPositionLocal { get; private set; }
        private Vector3 _previousPositionLocal;
        private Vector3 _previousPositionWorld;
        public Vector3 DeltaPositionWorld => CurrentPositionWorld - _previousPositionWorld;
        public Vector3 DeltaPositionLocal => CurrentPositionLocal - _previousPositionLocal;
        public float Grip { get; set; }
        public float Trigger { get; set; }

        public bool IsGrabbing { get; private set; }
        public IJVRHandInteract Grabbed { get; private set; }

        [SerializeField] private JVRHandControllerModel handControllerModel;
        
        [Header("Physic calculation")]
        [SerializeField] private float raycastRadius;
        [SerializeField] private Vector3 raycastOffset;
        [SerializeField] private LayerMask layerMask;
        private readonly Collider[] _hit = new Collider[8];
        private readonly List<IJVRHandInteract> _hitInteractions = new List<IJVRHandInteract>();

        private bool _showHandModel = true;

        private IJVRHandInteract _currentInteraction;
        private bool _isInteracting;

        public bool useTrackedPoseDriver;
        [SerializeField] private InputAction positionAction;
        [SerializeField] private InputAction rotationAction;


        public void InitializeController(JVRPlayer jvrPlayer)
        {
            Player = jvrPlayer;
            
            Player.OnToggleVR += OnToggleVR;

            Transform = transform;

            if (!useTrackedPoseDriver)
            {
                positionAction.Enable();
                rotationAction.Enable();
            }
        }

        private void OnDestroy()
        {
            Player.OnToggleVR -= OnToggleVR;
        }

        private void OnToggleVR(bool vr)
        {
            gameObject.SetActive(vr);
        }

        public void SetModelVisibility(bool isVisible)
        {
            _showHandModel = isVisible;
            handControllerModel.SetVisibility(_showHandModel);
        }

        public void UpdateController()
        {

            _previousPositionWorld = CurrentPositionWorld;
            _previousPositionLocal = CurrentPositionLocal;
            
            if (!useTrackedPoseDriver)
            {
                CurrentPositionLocal = positionAction.ReadValue<Vector3>();
                Transform.localPosition = CurrentPositionLocal;
                Quaternion CurrentRotationLocal = rotationAction.ReadValue<Quaternion>();
                Transform.localRotation = CurrentRotationLocal;
            }
            else
            {
                
                CurrentPositionLocal = Transform.localPosition;
            }
            
            
            CurrentPositionWorld = Transform.position;
            Quaternion CurrentRotationWorld = Transform.rotation;


            if (_showHandModel)
            {
                handControllerModel.SetPositionAndRotation(CurrentPositionWorld, CurrentRotationWorld);
                handControllerModel.SetGripAndTriggerValue(Grip, Trigger);
            }

            SphereCast();
        }

        public void SetGrabbedObject(IJVRHandInteract grabbed)
        {
            Grabbed = grabbed;
            IsGrabbing = true;
        }

        public void StopGrabbing()
        {
            Grabbed = null;
            IsGrabbing = false;
        }

        private void SphereCast()
        {

            if (IsGrabbing)
            {
                Grabbed.HandInteractionStay(this);
                return;
            }

            int hitCount = Physics.OverlapSphereNonAlloc(Transform.position + raycastOffset, raycastRadius, _hit, layerMask);

            if (hitCount == 0)
            {
                if (!_isInteracting) return;
                
                StopInteraction();
            }
            else
            {

                _hitInteractions.Clear();
                bool contains = false;

                for (int i = 0; i < hitCount; i++)
                {
                    IJVRHandInteract interact = _hit[i].GetComponent<IJVRHandInteract>();
                    if (interact == null) continue;
                    
                    _hitInteractions.Add(interact);
                    
                    if (!_isInteracting) continue;
                    if (interact != _currentInteraction) continue;
                    contains = true;
                    break;
                }

                if (_hitInteractions.Count == 0)
                {
                    if (!_isInteracting) return;
                
                    StopInteraction();
                }
                else
                {
                    if (!_isInteracting)
                    {
                        StartInteraction(_hitInteractions[0]);
                    }
                    else
                    {
                        if (contains)
                        {
                            _currentInteraction.HandInteractionStay(this);
                        }
                        else
                        {
                            StopInteraction();
                            StartInteraction(_hitInteractions[0]);
                        }
                    }
                }
            }
        }

        private void StartInteraction(IJVRHandInteract interact)
        {
            _isInteracting = true;
            _currentInteraction = interact;
            _currentInteraction.HandInteractionStart(this);
            if(_showHandModel) handControllerModel.StartInteraction();
        }

        private void StopInteraction()
        {
            _isInteracting = false;
            _currentInteraction.HandInteractionStop(this);
            _currentInteraction = null;
            if(_showHandModel) handControllerModel.StopInteraction();
        }

        public void SetModule(VRControllerModule module)
        {
            if (HasModule)
            {
                Destroy(VrControllerModule.gameObject);
                HasModule = false;
                VrControllerModule = null;
            }

            if (module == null) return;
            
            VrControllerModule = Instantiate(module, Transform);
            HasModule = true;
        }

        public void PrimaryAction()
        {
            if (!HasModule) return;
            VrControllerModule.PrimaryAction();
        }

        public void SecondaryAction()
        {
            if (!HasModule) return;
            VrControllerModule.SecondaryAction();
        }

    }
}