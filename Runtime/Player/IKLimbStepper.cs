using System.Collections;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

namespace Jichaels.VRSDK
{
    public class IKLimbStepper : MonoBehaviour
    {
        [SerializeField] private Transform homeTransform;

        [SerializeField] private float wantStepAtDistance = 1;
        [SerializeField] private float moveDuration = 0.2f;
        [SerializeField] private float stepOvershootFraction = 0.2f;

        public bool Moving { get; private set; }

        private Transform _transform;

        private WaitForEndOfFrame _endOfFrame = new WaitForEndOfFrame();


        private void Awake()
        {
            _transform = transform;
        }

        public void TryMove()
        {
            if (Moving) return;

            float distanceFromHome = Vector3.Distance(_transform.position, homeTransform.position);

            if (distanceFromHome > wantStepAtDistance) StartCoroutine(MoveLimbCo());
        }

        private IEnumerator MoveLimbCo()
        {
            Moving = true;

            Quaternion startRotation = _transform.rotation;
            Vector3 startPosition = _transform.position;

            Quaternion endRotation = homeTransform.rotation;

            Vector3 homePosition = homeTransform.position;

            Vector3 towardHome = homePosition - startPosition;

            float overshootDistance = wantStepAtDistance * stepOvershootFraction;
            Vector3 overshootVector = towardHome * overshootDistance;
            overshootVector = Vector3.ProjectOnPlane(overshootVector, Vector3.up);

            Vector3 endPosition = homePosition + overshootVector;

            Vector3 centerPoint = (startPosition + endPosition) / 2;
            centerPoint += homeTransform.up * Vector3.Distance(startPosition, endPosition) / 2;

            float delta = 0;

            do
            {
                delta += Time.deltaTime;
                float normalizedTime = Easing.InOutCubic(delta / moveDuration);

                _transform.position = Vector3.Lerp(
                    Vector3.Lerp(startPosition, centerPoint, normalizedTime),
                    Vector3.Lerp(centerPoint, endPosition, normalizedTime),
                    normalizedTime
                );
                _transform.rotation = Quaternion.Slerp(startRotation, endRotation, normalizedTime);

                yield return _endOfFrame;

            } while (delta < moveDuration);

            Moving = false;
        }

    }
}