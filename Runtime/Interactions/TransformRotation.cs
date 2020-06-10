using System.Collections;
using Jichaels.Core;
using Sirenix.OdinInspector;
using UnityEngine;

public class TransformRotation : MonoBehaviour
{

    [SerializeField] private Transform toRotate;

    [SerializeField] private bool smoothRotation = true;
    [SerializeField, ShowIf("smoothRotation")] private float rotationSpeed = 1;
    
    private Quaternion _startRotation;
    private Quaternion _endRotation;

    private bool _isRotating;
    private Coroutine _coroutine;

    public void Rotate(Vector3 eulers, bool forceNoSmooth = false)
    {
        _endRotation = Quaternion.Euler(eulers);

        if (forceNoSmooth || !smoothRotation)
        {
            Rotate();
        }
        else
        {
            RotateSmooth();
        }
    }

    private void Rotate()
    {
        toRotate.localRotation = _endRotation;
    }

    private void RotateSmooth()
    {
        if (_isRotating) StopCoroutine(_coroutine);
        _isRotating = true;
        _coroutine = StartCoroutine(RotateSmoothCo());
    }

    private IEnumerator RotateSmoothCo()
    {
        _startRotation = toRotate.localRotation;
        float delta = 0;

        while (delta < 1)
        {
            delta += Time.deltaTime * rotationSpeed;
            toRotate.localRotation = Quaternion.Slerp(_startRotation, _endRotation, delta);
            yield return Yielders.EndOfFrame;
        }

        _isRotating = false;
    }

}