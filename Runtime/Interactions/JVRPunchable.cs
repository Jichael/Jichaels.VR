using UnityEngine;

namespace Jichaels.VRSDK
{

    [RequireComponent(typeof(Rigidbody))]
    public class JVRPunchable : MonoBehaviour, IJVRHandInteract
    {

        private Rigidbody _rb;

        [SerializeField, Range(0.1f, 10), Tooltip("Minimum time between two hits")]
        private float minHitInterval = 0.5f;

        private float _lastHit;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
        }

        public bool DisableInteraction { get; set; }
        public void HandInteractionStart(JVRHandController jvrHandController)
        {
            
        }

        public void HandInteractionStay(JVRHandController jvrHandController)
        {
            if (jvrHandController.Grip + jvrHandController.Trigger < 1) return;
            
            if (_lastHit + minHitInterval >= Time.time) return;
            
            _lastHit = Time.time;
            _rb.AddForce(jvrHandController.DeltaPositionWorld * Rules.PunchForce, ForceMode.VelocityChange);
        }

        public void HandInteractionStop(JVRHandController jvrHandController)
        {
            
        }
    }
}