using System.Collections;
using Jichaels.Core;
using UnityEngine;
using UnityEngine.InputSystem.XR;

namespace Jichaels.VRSDK
{
    public class JVRHeadSet : MonoBehaviour
    {
        public Camera Camera => VRCamera;
        public Transform CameraTransform { get; private set; }

        [SerializeField] private JVRPlayer player;
        

        [SerializeField] private Camera VRCamera;

        [SerializeField] private TrackedPoseDriver headSetTracking;

        [SerializeField] private Transform trackingSpace;


        private void Awake()
        {
            CameraTransform = VRCamera.transform;
            player.OnToggleVR += OnToggleVR;
        }

        private void OnDestroy()
        {
            player.OnToggleVR -= OnToggleVR;
        }

        private void OnToggleVR(bool vr)
        {

            float playerHeight = player.IsCrouched ? Rules.PlayerCrouchedHeight : Rules.PlayerHeight;

            if (vr)
            {
                SetTrackingState(true);
                SetTrackingSpace(playerHeight);
                CameraTransform.localPosition = Vector3.zero;
                Camera.stereoTargetEye = StereoTargetEyeMask.Both;
            }
            else
            {
                SetTrackingState(false);
                ResetTrackingSpace();
                CameraTransform.localPosition = playerHeight / 2 * Vector3.up + Rules.CameraOffset;
                CameraTransform.localEulerAngles = Vector3.zero;
                Camera.stereoTargetEye = StereoTargetEyeMask.None;
                Camera.fieldOfView = Rules.FieldOfView;
            }
        }

        private void SetTrackingState(bool active)
        {
            headSetTracking.enabled = active;
        }

        private void ResetTrackingSpace()
        {
            trackingSpace.localPosition = Vector3.zero;
            trackingSpace.localRotation = Quaternion.identity;
        }

        public void SetTrackingSpace(float characterHeight)
        {
            StartCoroutine(SetTrackingSpaceCo(characterHeight));
        }


        private IEnumerator SetTrackingSpaceCo(float characterHeight)
        {
            yield return Yielders.EndOfFrame;

            Vector3 localPos = characterHeight / 2 * Vector3.up + Rules.CameraOffset -
                               CameraTransform.localPosition;
            trackingSpace.localPosition = localPos;

            Vector3 localRot = Quaternion.Inverse(CameraTransform.localRotation).eulerAngles;
            localRot.x = 0;
            localRot.z = 0;
            trackingSpace.localEulerAngles = localRot;
        }
    }
}
