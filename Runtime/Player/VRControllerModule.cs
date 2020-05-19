using UnityEngine;

namespace Jichaels.VRSDK
{
    public abstract class VRControllerModule : MonoBehaviour
    {
        public JVRHandController HandController { get; private set; }
        public bool Initialized { get; protected set; }
        public abstract void PrimaryAction();
        public abstract void SecondaryAction();

        protected virtual void Awake()
        {
            HandController = GetComponentInParent<JVRHandController>();
            if (HandController == null)
            {
                Debug.LogError("Could not find controller in parent !", this);
            }
        }
    }
}