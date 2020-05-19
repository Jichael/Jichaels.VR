using UnityEngine;

namespace Jichaels.VRSDK
{
    public abstract class JVRHandControllerModel : MonoBehaviour
    {
        protected Transform Transform;

        public abstract void SetVisibility(bool isVisible);
        public abstract void StartInteraction();
        public abstract void StopInteraction();
        public abstract void SetGripAndTriggerValue(float gripValue, float triggerValue);

        protected virtual void Awake()
        {
            Transform = transform;
        }

        public virtual void SetPositionAndRotation(Vector3 position, Quaternion rotation)
        {
            Transform.SetPositionAndRotation(position, rotation);
        }
    }
}