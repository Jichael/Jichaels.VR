using UnityEngine;
using UnityEngine.UI;

namespace Jichaels.VRSDK
{

    [RequireComponent(typeof(Button))]
    public class JVRUIButton : JVRClickableButton, IJVRLaserInteract, IJVRMouseInteract
    {

        [SerializeField] private Button button;

        [SerializeField] private CursorInfo cursorInfo;
        

#if UNITY_EDITOR
        private void OnValidate()
        {
            button = GetComponent<Button>();
            if (GetComponent<Collider>() == null)
            {
                BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
                boxCollider.isTrigger = true;
                Vector3 size = ((RectTransform) transform).sizeDelta;
                size.z = 1;
                boxCollider.size = size;
            }
            
            // Works but trigger warnings in Editor
            //gameObject.layer = LayerMask.NameToLayer("JVRInteract");
            
        }
#endif

        private void Awake()
        {
            button.onClick.AddListener(() => HasBeenClicked = true);
        }

        #region IJVRLaserInteract

        public void JVRLaserInteract(JVRLaserController jvrLaserController)
        {
            if (!button.interactable) return;
            button.onClick.Invoke();
        }

        public void LaserHoverEnter(JVRLaserController jvrLaserController)
        {
            button.OnPointerEnter(null);
        }

        public void LaserHoverStay(JVRLaserController jvrLaserController)
        {

        }

        public void LaserHoverExit(JVRLaserController jvrLaserController)
        {
            button.OnPointerExit(null);
        }

        #endregion

        #region IJVRMouseInteract

        public void PrimaryInteraction(JVRMouseController mouseController) {}

        public void SecondaryInteraction(JVRMouseController mouseController) {}

        public void MouseHoverEnter(JVRMouseController mouseController) {}

        public void MouseHoverStay(JVRMouseController mouseController) {}

        public void MouseHoverExit(JVRMouseController mouseController) {}

        public CursorInfo HoverCursor => cursorInfo;

        #endregion

        public bool DisableInteraction { get; set; }
    }
}