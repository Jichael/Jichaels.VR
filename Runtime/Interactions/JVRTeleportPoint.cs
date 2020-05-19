using System;
using UnityEngine;

namespace Jichaels.VRSDK
{

    public class JVRTeleportPoint : MonoBehaviour, IJVRLaserInteract
    {

        [SerializeField] private TeleportMode teleportMode;

        [SerializeField] private Animator animator;
        
        private static readonly int Hovered = Animator.StringToHash("Hovered");

        public void JVRLaserInteract(JVRLaserController jvrLaserController)
        {
            switch (teleportMode)
            {
                case TeleportMode.Teleport:
                    jvrLaserController.HandController.Player.Teleport(transform.position);
                    break;
                case TeleportMode.Dash:
                    jvrLaserController.HandController.Player.Dash(transform.position);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void LaserHoverEnter(JVRLaserController jvrLaserController)
        {
            animator.SetBool(Hovered, true);
        }

        public void LaserHoverStay(JVRLaserController jvrLaserController)
        {

        }

        public void LaserHoverExit(JVRLaserController jvrLaserController)
        {
            animator.SetBool(Hovered, false);
        }

        public bool DisableInteraction { get; set; }
    }

    public enum TeleportMode
    {
        Teleport,
        Dash
    }

}