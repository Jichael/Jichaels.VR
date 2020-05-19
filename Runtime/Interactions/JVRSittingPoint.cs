using UnityEngine;

namespace Jichaels.VRSDK
{

    public class JVRSittingPoint : MonoBehaviour, IJVRLaserInteract, IJVRMouseInteract
    {

        [SerializeField] private CursorInfo cursorInfo;
        private Vector3 _initialPosition;

        public void Action(JVRPlayer player)
        {
            if (player.IsSitting)
            {
                player.Teleport(_initialPosition);
            }
            else
            {
                player.Sit(this);
            }
        }

        public void Sit(JVRPlayer player)
        {
            _initialPosition = player.Transform.position;
            Transform t = transform;
            player.Transform.position = t.position;
            player.Transform.rotation = t.rotation;
        }

        public void StandUp(JVRPlayer player)
        {
            player.StandUp();
            player.Transform.position = _initialPosition;
        }

        #region IJVRLaserInteract

        public void JVRLaserInteract(JVRLaserController jvrLaserController)
        {
            Action(jvrLaserController.HandController.Player);
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

        #region IJVRMouseInteract

        public void PrimaryInteraction(JVRMouseController mouseController)
        {
            Action(mouseController.Player);
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

        public bool DisableInteraction { get; set; }
    }
}