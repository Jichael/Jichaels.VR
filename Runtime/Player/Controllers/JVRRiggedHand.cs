using System;
using System.Collections;
using Jichaels.Core;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Jichaels.VRSDK
{
    public class JVRRiggedHand : JVRHandControllerModel
    {

        public Transform thumbTarget;
        public Transform indexTarget;
        public Transform middleTarget;
        public Transform ringTarget;
        public Transform pinkyTarget;

        [SerializeField] private float animationSpeed;

        [SerializeField] private JVRRiggedHandPose defaultPose;
        [SerializeField] private JVRRiggedHandPose indexPointPose;

        private Coroutine _coroutine;
        private bool _isAnimating;

        private Vector3 _startPos1;
        private Vector3 _startRot1;
        
        private Vector3 _startPos2;
        private Vector3 _startRot2;
        
        private Vector3 _startPos3;
        private Vector3 _startRot3;
        
        private Vector3 _startPos4;
        private Vector3 _startRot4;
        
        private Vector3 _startPos5;
        private Vector3 _startRot5;
        
        public void ApplyPose(JVRRiggedHandPose handPose)
        {
            if (_isAnimating)
            {
                StopCoroutine(_coroutine);
            }

            _isAnimating = true;
            _coroutine = StartCoroutine(ApplyPoseCo(handPose));
        }

        private IEnumerator ApplyPoseCo(JVRRiggedHandPose handPose)
        {
            float delta = 0;

            if (handPose.overrideThumb)
            {
                _startPos1 = thumbTarget.position;
                _startRot1 = thumbTarget.eulerAngles;
            }
            
            if (handPose.overrideIndex)
            {
                _startPos2 = indexTarget.position;
                _startRot2 = indexTarget.eulerAngles;
            }
            
            if (handPose.overrideMiddle)
            {
                _startPos3 = middleTarget.position;
                _startRot3 = middleTarget.eulerAngles;
            }
            
            if (handPose.overrideRing)
            {
                _startPos4 = ringTarget.position;
                _startRot4 = ringTarget.eulerAngles;
            }
            
            if (handPose.overridePinky)
            {
                _startPos5 = pinkyTarget.position;
                _startRot5 = pinkyTarget.eulerAngles;
            }

            while (delta < 1)
            {
                delta += Time.deltaTime * animationSpeed;

                if (handPose.overrideThumb)
                {
                    thumbTarget.position = Vector3.Lerp(_startPos1, handPose.thumbPose.targetPosition, delta);
                    thumbTarget.eulerAngles = Vector3.Lerp(_startRot1, handPose.thumbPose.targetRotation, delta);
                }

                if (handPose.overrideIndex)
                {
                    indexTarget.position = Vector3.Lerp(_startPos2, handPose.indexPose.targetPosition, delta);
                    indexTarget.eulerAngles = Vector3.Lerp(_startRot2, handPose.indexPose.targetRotation, delta);
                }

                if (handPose.overrideMiddle)
                {
                    middleTarget.position = Vector3.Lerp(_startPos3, handPose.middlePose.targetPosition, delta);
                    middleTarget.eulerAngles = Vector3.Lerp(_startRot3, handPose.middlePose.targetRotation, delta);
                }

                if (handPose.overrideRing)
                {
                    ringTarget.position = Vector3.Lerp(_startPos4, handPose.ringPose.targetPosition, delta);
                    ringTarget.eulerAngles = Vector3.Lerp(_startRot4, handPose.ringPose.targetRotation, delta);
                }

                if (handPose.overridePinky)
                {
                    pinkyTarget.position = Vector3.Lerp(_startPos5, handPose.pinkyPose.targetPosition, delta);
                    pinkyTarget.eulerAngles = Vector3.Lerp(_startRot5, handPose.pinkyPose.targetRotation, delta);
                }

                yield return Yielders.EndOfFrame;
            }

            _isAnimating = false;
        }

        public void RemovePose()
        {
            if (_isAnimating)
            {
                StopCoroutine(_coroutine);
            }
        }

        public override void SetVisibility(bool isVisible)
        {
            gameObject.SetActive(isVisible);
        }

        public override void StartInteraction()
        {
            // TODO
        }

        public override void StopInteraction()
        {
            // TODO
        }

        public override void SetGripAndTriggerValue(float gripValue, float triggerValue)
        {
            // TODO
        }
    }
    
    [Serializable]
    public class JVRRiggedHandPose
    {

        public Vector3 handPosition;
        public Vector3 handRotation;

        public bool overrideThumb;
        [ShowIf("overrideThumb")] 
        public JVRRiggedFingerPose thumbPose;
        
        public bool overrideIndex;
        [ShowIf("overrideIndex")] 
        public JVRRiggedFingerPose indexPose;
        
        public bool overrideMiddle;
        [ShowIf("overrideMiddle")] 
        public JVRRiggedFingerPose middlePose;
        
        public bool overrideRing;
        [ShowIf("overrideRing")] 
        public JVRRiggedFingerPose ringPose;
        
        public bool overridePinky;
        [ShowIf("overridePinky")] 
        public JVRRiggedFingerPose pinkyPose;


        public JVRRiggedHand hand;
        [Button]
        private void ToWorldPosition()
        {
            thumbPose.targetPosition = hand.thumbTarget.TransformPoint(thumbPose.targetPosition);
            indexPose.targetPosition = hand.indexTarget.TransformPoint(indexPose.targetPosition);
            middlePose.targetPosition = hand.middleTarget.TransformPoint(middlePose.targetPosition);
            ringPose.targetPosition = hand.ringTarget.TransformPoint(ringPose.targetPosition);
            pinkyPose.targetPosition = hand.pinkyTarget.TransformPoint(pinkyPose.targetPosition);
        }
    }

    [Serializable]
    public struct JVRRiggedFingerPose
    {
        public Vector3 targetPosition;
        public Vector3 targetRotation;
    }
}