using System.Collections;
using Jichaels.Core;
using Sirenix.OdinInspector;
using UnityEngine;

public class TransformRotation : MonoBehaviour
{

    [SerializeField] private Transform toRotate;

    [SerializeField] private bool smoothRotation = true;
    [SerializeField, ShowIf("smoothRotation")] private float rotationSpeed = 1;
    
    private Vector3 _startPosition;
    private Vector3 _endPosition;

    private bool _isRotating;
    private Coroutine _coroutine;

    public void Rotate(Vector3 eulers, bool forceNoSmooth = false)
    {
        _endPosition = eulers;

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
        toRotate.localEulerAngles = _endPosition;
    }

    private void RotateSmooth()
    {
        if (_isRotating) StopCoroutine(_coroutine);
        _isRotating = true;
        _coroutine = StartCoroutine(RotateSmoothCo());
    }

    private IEnumerator RotateSmoothCo()
    {
        _startPosition = toRotate.localEulerAngles;
        float delta = 0;

        while (delta < 1)
        {
            delta += Time.deltaTime * rotationSpeed;
            toRotate.localEulerAngles = Vector3.Lerp(_startPosition, _endPosition, delta);
            yield return Yielders.EndOfFrame;
        }

        _isRotating = false;
    }

}

public class RendererChangeMaterial : MonoBehaviour
{

    [SerializeField] private new Renderer renderer;

    public void ChangeMaterial(Material material)
    {
        renderer.material = material;
    }
    
}