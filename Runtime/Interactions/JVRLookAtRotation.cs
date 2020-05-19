using UnityEngine;

namespace Jichaels.VRSDK
{

    public class JVRLookAtRotation : MonoBehaviour, IJVRHandInteract
    {

        [SerializeField] private Transform toRotate;

        [SerializeField] private Vector3 minRotationDelta;
        [SerializeField] private Vector3 maxRotationDelta;

        private bool _isGrabbed;

        // TODO : No clue where does this come from but it works -> understand or rework
        private readonly Vector3 _localRotationRangeCenter = new Vector3(0, 0.999f, 0.044f);

        private void Grab(JVRHandController jvrHandController)
        {
            jvrHandController.SetGrabbedObject(this);
            _isGrabbed = true;
        }

        private void Release(JVRHandController jvrHandController)
        {
            _isGrabbed = false;
            jvrHandController.StopGrabbing();
        }

        private void RotateTo(Vector3 lookPosition)
        {
            Vector3 up = toRotate.up;
            Vector3 forward = toRotate.forward;
            Vector3 right = toRotate.right;
            Vector3 rotatePosition = toRotate.position;
            Vector3 pos1 = rotatePosition + up;
            Vector3 pos2 = rotatePosition + forward;
            Plane p = new Plane(rotatePosition, pos1, pos2);
            lookPosition = p.ClosestPointOnPlane(lookPosition);

            Vector3 worldRotationRangeCenter = toRotate.parent.TransformDirection(_localRotationRangeCenter);
            Vector3 targetForward = lookPosition - rotatePosition;

            float targetAngle = Vector3.SignedAngle(worldRotationRangeCenter, targetForward, right);

            float clampedAngle = Mathf.Clamp(targetAngle, minRotationDelta.x, maxRotationDelta.x);

            Vector3 clampedForward = Quaternion.AngleAxis(clampedAngle, right) * worldRotationRangeCenter;

            toRotate.rotation = Quaternion.LookRotation(clampedForward, Vector3.Cross(clampedForward, right));
        }

        #region Hand interaction
        public bool DisableInteraction { get; set; }
        public void HandInteractionStart(JVRHandController jvrHandController)
        {
            // TODO : grab visual
        }

        public void HandInteractionStay(JVRHandController jvrHandController)
        {
            if (_isGrabbed)
            {
                if (jvrHandController.Grip + jvrHandController.Trigger < Rules.GrabbingThreshold)
                {
                    Release(jvrHandController);
                }
                else
                {
                    RotateTo(jvrHandController.CurrentPositionWorld);
                }
            }
            else
            {
                if(jvrHandController.IsGrabbing) return;
                
                if (jvrHandController.Grip + jvrHandController.Trigger >= Rules.GrabbingThreshold)
                {
                    Grab(jvrHandController);
                }
            }
        }

        public void HandInteractionStop(JVRHandController jvrHandController)
        {
            // TODO : release visual
        }
        
        #endregion
    }

}