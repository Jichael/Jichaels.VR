using System;
using UnityEngine;

namespace Jichaels.VRSDK
{

    public class JVRTeleportArea : MonoBehaviour, IJVRLaserInteract
    {

        [SerializeField] private TeleportMode teleportMode;

        [SerializeField] private GameObject teleportTarget;

        private void Awake()
        {
            teleportTarget.SetActive(false);
        }

        public void JVRLaserInteract(JVRLaserController jvrLaserController)
        {
            switch (teleportMode)
            {
                case TeleportMode.Teleport:
                    jvrLaserController.HandController.Player.Teleport(teleportTarget.transform.position);
                    break;
                case TeleportMode.Dash:
                    jvrLaserController.HandController.Player.Dash(teleportTarget.transform.position);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void LaserHoverEnter(JVRLaserController jvrLaserController)
        {
            if (jvrLaserController.HandController.Player.LockMovement) return;

            teleportTarget.transform.position = jvrLaserController.PointerPosition;
            teleportTarget.SetActive(true);
        }

        public void LaserHoverStay(JVRLaserController jvrLaserController)
        {
            if (jvrLaserController.HandController.Player.LockMovement) return;
            teleportTarget.transform.position = jvrLaserController.PointerPosition;
        }

        public void LaserHoverExit(JVRLaserController jvrLaserController)
        {
            teleportTarget.SetActive(false);
        }

        public bool DisableInteraction { get; set; }
    }

}