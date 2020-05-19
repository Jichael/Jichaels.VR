using UnityEngine;

namespace Jichaels.VRSDK
{
    public class JVRButton : JVRClickableButton, IJVRMouseInteract, IJVRLaserInteract
    {

        [SerializeField] private CursorInfo cursorInfo;
        
        #region IJVRMouseInteract

        public void PrimaryInteraction(JVRMouseController mouseController)
        {
            HasBeenClicked = true;
        }

        public void SecondaryInteraction(JVRMouseController mouseController)
        {

        }

        public void MouseHoverEnter(JVRMouseController mouseController)
        {

        }

        public void MouseHoverStay(JVRMouseController mouseController)
        {

        }

        public void MouseHoverExit(JVRMouseController mouseController)
        {

        }

        public CursorInfo HoverCursor => cursorInfo;

        #endregion


        #region IJVRLaserInteract

        public void JVRLaserInteract(JVRLaserController jvrLaserController)
        {
            HasBeenClicked = true;
        }

        public void LaserHoverEnter(JVRLaserController jvrLaserController)
        {

        }

        public void LaserHoverStay(JVRLaserController jvrLaserController)
        {

        }

        public void LaserHoverExit(JVRLaserController jvrLaserController)
        {

        }

        #endregion

        public bool DisableInteraction { get; set; }
    }
}