using System.Collections.Generic;
using UnityEngine;

namespace Jichaels.VRSDK
{
    public class JVRFingerController : MonoBehaviour
    {
        
        public Vector3 CurrentPositionWorld { get; private set; }
        private Vector3 _previousPositionWorld;
        public Vector3 DeltaPositionWorld => CurrentPositionWorld - _previousPositionWorld;

        public JVRHandController jvrHandController;

        [SerializeField] private float activationRadius;
        [SerializeField] private LayerMask layerMask;

        private Transform _transform;

        private readonly Collider[] _hit = new Collider[1];
        private readonly List<IJVRFingerInteract> _fingerInteracts = new List<IJVRFingerInteract>();

        private IJVRFingerInteract _currentInteraction;
        private bool _isInteracting;

        private void Awake()
        {
            _transform = transform;
        }
        
        private void Update()
        {
            _previousPositionWorld = CurrentPositionWorld;
            CurrentPositionWorld = _transform.position;
            
            if (jvrHandController.IsGrabbing) return;
            if (jvrHandController.Grip + jvrHandController.Trigger >= Rules.GrabbingThreshold) return;

            int hitCount = Physics.OverlapSphereNonAlloc(_transform.position, activationRadius, _hit, layerMask);

            if (hitCount == 0)
            {
                if (!_isInteracting) return;
                StopInteraction();
            }
            else
            {
                
                _fingerInteracts.Clear();
                bool contains = false;

                for (int i = 0; i < hitCount; i++)
                {
                    IJVRFingerInteract interact = _hit[i].GetComponent<IJVRFingerInteract>();
                    if (interact == null) continue;
                    
                    _fingerInteracts.Add(interact);

                    if (!_isInteracting) continue;
                    if (interact != _currentInteraction) continue;
                    contains = true;
                    break;
                }

                if (_fingerInteracts.Count == 0)
                {
                    if (!_isInteracting) return;
                    StopInteraction();
                }
                else
                {
                    if (!_isInteracting)
                    {
                        StartInteraction(_fingerInteracts[0]);
                    }
                    else
                    {
                        if (contains)
                        {
                            _currentInteraction.InteractionStay(this);
                        }
                        else
                        {
                            StopInteraction();
                            StartInteraction(_fingerInteracts[0]);
                        }
                    }
                }
            }
        }

        private void StartInteraction(IJVRFingerInteract interact)
        {
            _isInteracting = true;
            _currentInteraction = interact;
            _currentInteraction.InteractionStart(this);
        }

        private void StopInteraction()
        {
            _isInteracting = false;
            _currentInteraction.InteractionStop(this);
            _currentInteraction = null;
        }
    }
}