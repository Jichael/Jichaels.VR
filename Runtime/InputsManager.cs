using UnityEngine;
using UnityEngine.InputSystem;

namespace Jichaels.VRSDK
{
    public class InputsManager : MonoBehaviour
    {
        // TODO : how to handle and refractor the player ? separate inputs, character logic, scheme update...
        public static InputsManager Instance { get; private set; }

        [SerializeField] private InputActionReference mousePositionReference;
        [SerializeField] private InputActionReference movementReference;
        [SerializeField] private InputActionReference rotationReference;

        public Vector2 MousePosition { get; private set; }
        public Vector2 Movement { get; private set; }
        public Vector2 Rotation { get; private set; }

        private void Awake()
        {
            Instance = this;
            mousePositionReference.action.Enable();
        }

        private void Update()
        {
            MousePosition = mousePositionReference.action.ReadValue<Vector2>();
            CursorManager.Instance.SetCursorPosition(MousePosition);
            
            Movement = movementReference.action.ReadValue<Vector2>();
            
            Rotation = rotationReference.action.ReadValue<Vector2>();
        }

    }
}