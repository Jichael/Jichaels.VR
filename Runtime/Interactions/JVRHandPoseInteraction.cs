using Sirenix.OdinInspector;
using UnityEngine;

namespace Jichaels.VRSDK
{
    public class JVRHandPoseInteraction : MonoBehaviour, IJVRHandInteract
    {

        [SerializeField] private JVRRiggedHandPose handPose;        
        
        public bool DisableInteraction { get; set; }
        public void HandInteractionStart(JVRHandController jvrHandController)
        {
            // TODO : How ?
            //jvrHandController.riggedHand.ApplyPose(handPose);
        }

        public void HandInteractionStay(JVRHandController jvrHandController) { }

        public void HandInteractionStop(JVRHandController jvrHandController)
        {
            //jvrHandController.riggedHand.RemovePose();
        }



        [SerializeField] private JVRRiggedHand hand;

        [Button]
        private void MoveHand()
        {
            var transform1 = hand.transform;
            transform1.position = handPose.handPosition;
            transform1.eulerAngles = handPose.handRotation;
        }
        
        [Button]
        private void ApplyPose()
        {
            hand.ApplyPose(handPose);
        }

        [Button]
        private void SaveCurrentPose()
        {
            var transform1 = hand.transform;
            handPose.handPosition = transform1.position;
            handPose.handRotation = transform1.eulerAngles;
            
            handPose.thumbPose.targetPosition = hand.thumbTarget.position;
            handPose.thumbPose.targetRotation = hand.thumbTarget.eulerAngles;
            
            handPose.indexPose.targetPosition = hand.indexTarget.position;
            handPose.indexPose.targetRotation = hand.indexTarget.eulerAngles;
            
            handPose.middlePose.targetPosition = hand.middleTarget.position;
            handPose.middlePose.targetRotation = hand.middleTarget.eulerAngles;
            
            handPose.ringPose.targetPosition = hand.ringTarget.position;
            handPose.ringPose.targetRotation = hand.ringTarget.eulerAngles;
            
            handPose.pinkyPose.targetPosition = hand.pinkyTarget.position;
            handPose.pinkyPose.targetRotation = hand.pinkyTarget.eulerAngles;
        }
        
    }
}