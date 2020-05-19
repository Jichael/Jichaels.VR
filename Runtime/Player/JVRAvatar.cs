using System.Collections;
using Jichaels.Core;
using UnityEngine;

namespace Jichaels.VRSDK
{

    public class JVRAvatar : MonoBehaviour
    {

        [SerializeField] private JVRPlayer player;

        [SerializeField] private Transform headTransform;
        [SerializeField] private Vector3 headPositionOffset;
        [SerializeField] private Vector3 headRotationOffset;

        [SerializeField] private Transform leftArmTransform;
        [SerializeField] private Vector3 leftArmPositionOffset;
        [SerializeField] private Vector3 leftArmRotationOffset;

        [SerializeField] private Transform rightArmTransform;
        [SerializeField] private Vector3 rightArmPositionOffset;
        [SerializeField] private Vector3 rightArmRotationOffset;

        [SerializeField] private Animator animator;

        [SerializeField] private IKLimbStepper leftLegIK;
        [SerializeField] private IKLimbStepper rightLegIK;
        
        private Vector3 _headOffset;
        private Vector3 _leftArmOffset;
        private Vector3 _rightArmOffset;
        private Transform _transform;
        private Coroutine _limbsCoroutine;
        
        private static readonly int VelocityXZ = Animator.StringToHash("VelocityXZ");

        private void Awake()
        {
            _transform = transform;
            _headOffset = _transform.position - headTransform.position;
            player.OnToggleVR += OnToggleVR;
            player.LeftHandController.SetModelVisibility(false);
            player.RightHandController.SetModelVisibility(false);
        }
        
        private void OnDestroy()
        {
            player.OnToggleVR -= OnToggleVR;
        }

        private void OnToggleVR(bool vr)
        {
            gameObject.SetActive(vr);
            if (vr)
            {
                _limbsCoroutine = StartCoroutine(LimbsUpdateCo());
            }
        }

        private void LateUpdate()
        {
            animator.SetFloat(VelocityXZ, player.Velocity);
            
            // IK
            headTransform.position = player.HeadSet.CameraTransform.TransformPoint(headPositionOffset);
            headTransform.rotation = player.HeadSet.CameraTransform.rotation * Quaternion.Euler(headRotationOffset);

            leftArmTransform.position = player.LeftHandController.Transform.TransformPoint(leftArmPositionOffset);
            leftArmTransform.rotation = player.LeftHandController.Transform.rotation * Quaternion.Euler(leftArmRotationOffset);

            rightArmTransform.position = player.RightHandController.Transform.TransformPoint(rightArmPositionOffset);
            rightArmTransform.rotation = player.RightHandController.Transform.rotation * Quaternion.Euler(rightArmRotationOffset);

            _transform.position = headTransform.position + _headOffset;
            _transform.forward = Vector3.ProjectOnPlane(headTransform.up, Vector3.up).normalized;
        }


        private IEnumerator LimbsUpdateCo()
        {
            while (true)
            {
                do
                {
                    leftLegIK.TryMove();
                    yield return Yielders.EndOfFrame;
                } while (leftLegIK.Moving);
                
                do
                {
                    rightLegIK.TryMove();
                    yield return Yielders.EndOfFrame;
                } while (rightLegIK.Moving);
            }
        }
    }

}