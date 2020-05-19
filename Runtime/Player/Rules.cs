using UnityEngine;

namespace Jichaels.VRSDK
{
    public static class Rules
    {

        public static float MovementSpeed = 2f;
        public static float SprintMultiplier = 1.5f;

        public static Vector3 CameraOffset = new Vector3(0, -0.1f, 0);
        public static float PlayerHeight = 1.8f;
        public static float PlayerCrouchedHeight = 1.2f;
        public static float CrouchAnimationSpeed = 3f;

        public static float RotationSpeedMouse = 0.15f;
        public static float RotationSpeedJoystick = 75f;
        public static float JoystickDeadZone = 0.125f;
        public static float RotationSnapAngle = 22.5f;
        public static float RotationSnapTimer = 0.5f;

        public static float MouseActionMaxDistance = 1.5f;

        // How much grip + trigger is needed for grabbing/interacting
        public static float GrabbingThreshold = 0.8f;

        public static float ThrowForce = 200f;

        public static float PunchForce = 100f;

        // Multiplier of y delta while climbing
        public static float ClimbForce = 1f;


        public static float FieldOfView = 60;

    }
}