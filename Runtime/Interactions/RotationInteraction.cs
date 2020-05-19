using Jichaels.VRSDK;
using Sirenix.OdinInspector;
using UnityEngine;

public class RotationInteraction : MonoBehaviour, IJVRMouseInteract
{
    
    // TODO : rework this by splitting logic from interaction

    [SerializeField] private TransformRotation transformRotation;    
    
    [SerializeField] private Vector3[] rotations;
    
    #if UNITY_EDITOR
    private int Length => Mathf.Max(rotations.Length - 1, 0);
    [HideIf("Length", 0), PropertyRange(0, "Length")]
    #endif
    [SerializeField] private int defaultRotation;

    [SerializeField] private bool applyDefaultRotationOnAwake;

    private int _currentIndex;

    private void Awake()
    {
        _currentIndex = defaultRotation;
        if(applyDefaultRotationOnAwake) transformRotation.Rotate(rotations[_currentIndex], true);
    }

    public void AddIndex(int value)
    {
        _currentIndex = (_currentIndex + value) % rotations.Length;
    }

    public void SetIndex(int value)
    {
        if (value < 0 || value >= rotations.Length)
        {
            Debug.LogError($"Requested index ({value}) is out of bound !");
            return;
        }

        _currentIndex = value;
    }

    public void Rotate()
    {
        transformRotation.Rotate(rotations[_currentIndex]);
    }


    #region IJVRMouseInteraction

    [SerializeField] private bool disableInteraction;

    public bool DisableInteraction
    {
        get => disableInteraction;
        set => disableInteraction = value;
    }

    public void PrimaryInteraction(JVRMouseController mouseController)
    {
        AddIndex(1);
        Rotate();
    }

    public void SecondaryInteraction(JVRMouseController mouseController)
    {
        
    }

    public void MouseHoverEnter(JVRMouseController mouseController)
    {
        
    }

    public void MouseHoverStay(JVRMouseController mouseController)
    {
        
    }

    public void MouseHoverExit(JVRMouseController mouseController)
    {
        
    }

    public CursorInfo HoverCursor => hoverCursor;

    [SerializeField] private CursorInfo hoverCursor;

    #endregion
}