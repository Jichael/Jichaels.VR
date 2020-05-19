using Sirenix.OdinInspector;
using UnityEngine;

namespace Jichaels.VRSDK
{

    [RequireComponent(typeof(Rigidbody))]
    public class JVRGrabbable : MonoBehaviour, IJVRHandInteract
    {

        public Transform Transform { get; private set; }
        public Rigidbody Rigidbody { get; private set; }
        private Renderer _renderer;

        public bool IsGrabbed { get; private set; }
        private JVRHandController _grabbingHandController;

        private readonly Collider[] _hit = new Collider[3];

        [SerializeField] private bool canBeSnapped;

        [ShowIf("canBeSnapped")]
        [SerializeField] private LayerMask layerMaskSnap;

        // Shader Graph
        // TODO : new HLP in start / stop
        private static readonly int ShowHighLight = Shader.PropertyToID("Boolean_42E0CC83");

        private float _otherControllerValue;
        private float _prevOtherControllerValue;

        private void Awake()
        {
            Transform = transform;
            Rigidbody = GetComponent<Rigidbody>();
            _renderer = GetComponentInChildren<Renderer>();
        }

        private void Grab(JVRHandController handController)
        {
            IsGrabbed = true;
            _grabbingHandController = handController;
            _grabbingHandController.SetGrabbedObject(this);
            Rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            Rigidbody.isKinematic = true;
            Transform.SetParent(_grabbingHandController.transform, true);
        }

        private void Release()
        {
            IsGrabbed = false;
            _grabbingHandController.StopGrabbing();

            bool snapped = false;

            if (canBeSnapped)
            {
                int hitCount = Physics.OverlapSphereNonAlloc(Transform.position, 0.1f, _hit, layerMaskSnap);
                for (int i = 0; i < hitCount; i++)
                {
                    JVRSnapPoint snapPoint = _hit[i].GetComponent<JVRSnapPoint>();
                    if (snapPoint == null) continue;

                    var snapPointTransform = snapPoint.transform;
                    Transform.position = snapPointTransform.position;
                    Transform.rotation = snapPointTransform.rotation;
                    Transform.SetParent(snapPointTransform, true);
                    snapped = true;
                    break;
                }
            }

            if (!snapped)
            {
                Rigidbody.isKinematic = false;
                Rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
                Transform.SetParent(null, true);
                Rigidbody.AddForce(_grabbingHandController.DeltaPositionWorld * Rules.ThrowForce, ForceMode.VelocityChange);
            }

            _grabbingHandController = null;
        }

        #region HandInteract

        public bool DisableInteraction { get; set; }
        public void HandInteractionStart(JVRHandController jvrHandController)
        {
            if (jvrHandController == _grabbingHandController)
            {
                Debug.LogError("Should not get here !");
            }
            else
            {
                _otherControllerValue = jvrHandController.Grip + jvrHandController.Trigger;
            }
        }

        public void HandInteractionStay(JVRHandController jvrHandController)
        {
            if (jvrHandController == _grabbingHandController)
            {
                if (_grabbingHandController.Grip + _grabbingHandController.Trigger < Rules.GrabbingThreshold)
                {
                    Release();
                }
            }
            else
            {
                _prevOtherControllerValue = _otherControllerValue;
                _otherControllerValue = jvrHandController.Grip + jvrHandController.Trigger;
                
                if (_otherControllerValue >= Rules.GrabbingThreshold && _prevOtherControllerValue < Rules.GrabbingThreshold)
                {
                    
                    if (IsGrabbed)
                    {
                        _grabbingHandController.StopGrabbing();
                    }

                    Grab(jvrHandController);
                }
            }
        }

        public void HandInteractionStop(JVRHandController jvrHandController)
        {
            if (jvrHandController == _grabbingHandController)
            {
                Debug.LogError("Should not get here !");
            }
        }
        
        #endregion
    }

}