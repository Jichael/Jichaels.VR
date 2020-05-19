using UnityEngine;

namespace Jichaels.VRSDK
{

    public class JVRLadder : MonoBehaviour, IJVRHandInteract
    {

        // TODO : redo all this it's a mess
        
        
        private JVRPlayer _player;

        public void OnTriggerEnter(Collider other)
        {
            JVRPlayer player = other.GetComponent<JVRPlayer>();
            if (!player) return;
            if (player.CurrentControlScheme == CustomControlScheme.VR) return;
            _player = player;
            _player.IsClimbing = true;
        }

        public void OnTriggerExit(Collider other)
        {
            JVRPlayer player = other.GetComponent<JVRPlayer>();
            if (player != _player) return;

            _player.IsClimbing = false;
            _player = null;
        }

        #region HandInteraction
        
        public bool DisableInteraction { get; set; }
        public void HandInteractionStart(JVRHandController jvrHandController)
        {
            // TODO : "grab" ladder
        }

        public void HandInteractionStay(JVRHandController jvrHandController)
        {
            if (jvrHandController.Grip + jvrHandController.Trigger > Rules.GrabbingThreshold)
            {
                jvrHandController.Player.IsClimbing = true;
            }
        }

        public void HandInteractionStop(JVRHandController jvrHandController)
        {
            // TODO : "release" ladder
        }
        
        #endregion
    }
}