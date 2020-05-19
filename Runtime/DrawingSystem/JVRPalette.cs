using UnityEngine;

namespace Jichaels.VRSDK
{

    public class JVRPalette : VRControllerModule
    {

        [SerializeField] private JVRStateButton[] materialButtons;
        [SerializeField] private JVRTrailDrawer drawer;
        
        protected override void Awake()
        {
            base.Awake();
            for (int i = 0; i < materialButtons.Length; i++)
            {
                materialButtons[i].OnStateChange += MaterialButtonOnStateChange;
            }
        }

        private void Start()
        {
            if (HandController.Player.LeftHandController.VrControllerModule == this)
            {
                if (HandController.Player.RightHandController.VrControllerModule.GetType() == typeof(JVRTrailDrawer))
                {
                    drawer = (JVRTrailDrawer)HandController.Player.RightHandController.VrControllerModule;
                }
                else
                {
                    Debug.LogError("JVRPalette only works if the other hand's module is JVRTrailDrawer !", this);
                    return;
                }

            }
            else if (HandController.Player.RightHandController.VrControllerModule == this)
            {
                if (HandController.Player.LeftHandController.VrControllerModule.GetType() == typeof(JVRTrailDrawer))
                {
                    drawer = (JVRTrailDrawer)HandController.Player.LeftHandController.VrControllerModule;
                }
                else
                {
                    Debug.LogError("JVRPalette only works if the other hand's module is JVRTrailDrawer !", this);
                    return;
                }
            }
            else
            {
                Debug.LogError("Should never get here !", this);
                return;
            }
            
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

        private void MaterialButtonOnStateChange(JVRStateButton stateButton, bool pressed)
        {
            if (!pressed) return;
            for (int i = 0; i < materialButtons.Length; i++)
            {
                if (materialButtons[i] == stateButton)
                {
                    drawer.SetMaterial(stateButton.GetComponent<Renderer>().material);
                }
                else
                {
                    materialButtons[i].UnpressButton();
                }
            }
        }

        private void OnDestroy()
        {
            for (int i = 0; i < materialButtons.Length; i++)
            {
                materialButtons[i].OnStateChange -= MaterialButtonOnStateChange;
            }
        }

        public override void PrimaryAction()
        {
            if(Initialized) drawer.LastDrawnObject.Rigidbody.useGravity = !drawer.LastDrawnObject.Rigidbody.useGravity;
        }

        public override void SecondaryAction()
        {
            
        }
    }
}