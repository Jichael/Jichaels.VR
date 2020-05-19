using System;
using System.Collections;
using Jichaels.Core;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using InputDevice = UnityEngine.InputSystem.InputDevice;

namespace Jichaels.VRSDK
{
    public class JVRPlayer : MonoBehaviour
    {

        private const string MouseKeyboardScheme = "MouseKeyboard";
        private const string GamepadScheme = "Gamepad";
        private const string VRScheme = "VR";

        public JVRHeadSet HeadSet => headSet;
        [SerializeField] private JVRHeadSet headSet;
        public JVRHandController LeftHandController => leftHandController;
        [SerializeField] private JVRHandController leftHandController;
        public JVRHandController RightHandController => rightHandController;
        [SerializeField] private JVRHandController rightHandController;

        public JVRMouseController MouseController => mouseController;
        [SerializeField] private JVRMouseController mouseController;
        
        public Transform Transform { get; private set; }
        private CharacterController _characterController;
        private PlayerInput _playerInput;
        public CustomControlScheme CurrentControlScheme { get; private set; }

        private Vector2 _inputMovement;
        private Vector3 _movement;
        
        public float Velocity { get; private set; }

        private Vector2 _inputRotation;
        private float _lastRotation;
        private Vector3 _currentLocalRotationEulers;

        public bool IsCrouched { get; private set; }
        public bool IsSprinting { get; private set; }
        public bool IsSitting { get; private set; }
        public bool IsClimbing { get; set; }

        [Header("ControlScheme")] 
        [SerializeField] private CustomControlScheme initialScheme;
        [SerializeField] private bool allowMouseKeyboard = true;
        [SerializeField] private bool allowGamepad = true;
        [SerializeField] private bool allowVR = true;
        
        [Header("VR Constraints"), ShowIf("allowVR")]
        [SerializeField, ShowIf("allowVR")] private bool allowVRMovement;
        [SerializeField, ShowIf("allowVR")] private bool allowVRRotation;
        [SerializeField, ShowIf("allowVRRotation")] private bool snapRotation;
        [SerializeField, ShowIf("allowVR")] private bool allowVRTeleportation;
        [SerializeField, ShowIf("allowVRTeleportation")] private float dashSpeed = 1;
        
        public bool LockMovement { get; set; }
        public bool LockRotation { get; set; }

        public event Action<bool> OnToggleVR;

        private void Awake()
        {
            
            // TODO : not working atm on the unity package
            /*
             * Get a reference to OculusLoader (or ideally, any loader that is active)
             *
             *  Then, to toggle VR :
             *
             * ON : referenceToLoader.Initialize(); referenceToLoader.Start();
             *
             * OFF : referenceToLoader.Stop(); referenceToLoader.Deinitialize();
             * 
             */
            
            Transform = transform;

            _characterController = GetComponent<CharacterController>();
            _characterController.height = Rules.PlayerHeight;

            _playerInput = GetComponent<PlayerInput>();

            LeftHandController.InitializeController(this);
            RightHandController.InitializeController(this);
            MouseController.InitializeController(this);

        }

        private void Start()
        {
            CurrentControlScheme = CustomControlScheme.Unassigned;
            if (initialScheme == CustomControlScheme.VR)
            {
                _playerInput.SwitchCurrentControlScheme(VRScheme);
            }
            else if (initialScheme == CustomControlScheme.MouseKeyboard)
            {
                _playerInput.SwitchCurrentControlScheme(MouseKeyboardScheme);
            }
            else if (initialScheme == CustomControlScheme.Gamepad)
            {
                _playerInput.SwitchCurrentControlScheme(GamepadScheme);
            }
            else
            {
                Debug.LogWarning("No default control scheme selected. There will be no input on JVRPlayer", this);
                return;
            }
            
            UpdateControlScheme();
        }

        private void OnEnable()
        {
            InputUser.onChange += OnControlSchemeChanged;
        }

        private void OnDisable()
        {
            InputUser.onChange -= OnControlSchemeChanged;
        }

        private void Update()
        {
            Move();
            Rotate();

            if (CurrentControlScheme == CustomControlScheme.VR)
            {
                LeftHandController.UpdateController();
                RightHandController.UpdateController();
            }
            else
            {
                MouseController.UpdateController();
            }
        }

        #region Movements

        private void Move()
        {
            if (LockMovement) return;

            if (IsSitting) return;

            if (!allowVRMovement && CurrentControlScheme == CustomControlScheme.VR) return;

            _inputMovement = InputsManager.Instance.Movement;

            if (IsClimbing)
            {
                Climb();
            }
            else
            {
                if (CurrentControlScheme == CustomControlScheme.MouseKeyboard)
                {
                    // Go to player's direction   
                    _movement = Rules.MovementSpeed * _inputMovement.y * Transform.forward +
                                Rules.MovementSpeed * _inputMovement.x * Transform.right;
                }
                else if (CurrentControlScheme == CustomControlScheme.VR)
                {
                    // Go to camera's direction   
                    _movement = Rules.MovementSpeed * _inputMovement.y * HeadSet.CameraTransform.forward +
                                Rules.MovementSpeed * _inputMovement.x * HeadSet.CameraTransform.right;
                }
                else if (CurrentControlScheme == CustomControlScheme.Gamepad)
                {
                    // Go to player's direction   
                    _movement = Rules.MovementSpeed * _inputMovement.y * Transform.forward +
                                Rules.MovementSpeed * _inputMovement.x * Transform.right;
                }

                if (IsSprinting) _movement *= Rules.SprintMultiplier;

                _movement.y = 0;

                _characterController.SimpleMove(_movement);

                Velocity = Mathf.Abs(_movement.x) + Mathf.Abs(_movement.z);
                
            }

        }

        private void Rotate()
        {
            //if (IsSitting) return;

            if (LockRotation) return;

            if (!allowVRRotation && CurrentControlScheme == CustomControlScheme.VR) return;

            _inputRotation = InputsManager.Instance.Rotation;

            if (CurrentControlScheme == CustomControlScheme.VR)
            {
                if (!snapRotation)
                {
                    if (Mathf.Abs(_inputRotation.x) < Rules.JoystickDeadZone) return;
                    Transform.Rotate(Vector3.up, _inputRotation.x * Time.deltaTime * Rules.RotationSpeedJoystick);
                }
                else
                {
                    _lastRotation += Time.deltaTime;
                    if (_lastRotation < Rules.RotationSnapTimer) return;
                    _lastRotation = 0;
                    float rotation = 0;

                    if (_inputRotation.x > Rules.JoystickDeadZone)
                    {
                        rotation = Rules.RotationSnapAngle;
                    }
                    else if (_inputRotation.x < -Rules.JoystickDeadZone)
                    {
                        rotation = -Rules.RotationSnapAngle;
                    }

                    Transform.Rotate(Vector3.up, rotation);
                }
            }
            else
            {
                if (CurrentControlScheme == CustomControlScheme.MouseKeyboard)
                {
                    // Should not apply Time.deltaTime to mouse delta (not a real delta, it's accumulating)
                    _inputRotation *= Rules.RotationSpeedMouse;
                }
                else if (CurrentControlScheme == CustomControlScheme.Gamepad)
                {
                    if (Mathf.Abs(_inputRotation.x) < Rules.JoystickDeadZone &&
                        Mathf.Abs(_inputRotation.y) < Rules.JoystickDeadZone) return;
                    _inputRotation *= Time.deltaTime * Rules.RotationSpeedJoystick;
                }

                Transform.Rotate(Vector3.up, _inputRotation.x);
                _currentLocalRotationEulers.x = Mathf.Clamp(_currentLocalRotationEulers.x - _inputRotation.y, -80, 80);
                HeadSet.CameraTransform.localEulerAngles = _currentLocalRotationEulers;
            }

        }

        private void Climb()
        {

            if (IsSitting)
            {
                IsClimbing = false;
                return;
            }

            _movement = Vector3.zero;
            bool stopClimbing = true;

            if (CurrentControlScheme == CustomControlScheme.VR)
            {
                if (LeftHandController.Grip + LeftHandController.Trigger > Rules.GrabbingThreshold)
                {
                    _movement = -(LeftHandController.DeltaPositionLocal.x * Transform.right +
                                  LeftHandController.DeltaPositionLocal.y * Transform.up) * Rules.ClimbForce;
                    stopClimbing = false;
                }

                if (RightHandController.Grip + RightHandController.Trigger > Rules.GrabbingThreshold)
                {
                    _movement = -(RightHandController.DeltaPositionLocal.x * Transform.right +
                                  RightHandController.DeltaPositionLocal.y * Transform.up) * Rules.ClimbForce;
                    stopClimbing = false;
                }
            }
            else
            {
                _movement = Rules.MovementSpeed * _inputMovement.y * Transform.up +
                            Rules.MovementSpeed * _inputMovement.x * Transform.right;
                _movement *= Time.deltaTime;
                stopClimbing = false;
            }

            if (stopClimbing)
            {
                IsClimbing = false;
            }
            else
            {
                _characterController.Move(_movement);
            }
        }

        public void StandUp()
        {
            if (!IsSitting) return;

            if (LockMovement) return;

            _characterController.detectCollisions = true;
            IsCrouched = false;
            Crouch();
            IsSitting = false;
        }

        public void Sit(JVRSittingPoint sittingPoint)
        {
            if (IsSitting) return;

            if (LockMovement) return;

            _characterController.detectCollisions = false;
            IsCrouched = true;
            Crouch();
            sittingPoint.Sit(this);
            IsSitting = true;
        }

        public void Teleport(Vector3 destination)
        {
            if (LockMovement) return;
            if (!allowVRTeleportation && CurrentControlScheme == CustomControlScheme.VR) return;
            Vector3 delta = destination - Transform.position;
            _characterController.detectCollisions = false;
            _characterController.enableOverlapRecovery = false;
            _characterController.Move(delta);
            _characterController.detectCollisions = true;
            _characterController.enableOverlapRecovery = true;
            StandUp();
        }

        public void Teleport(Transform destination)
        {
            Teleport(destination.position);
        }

        public void Dash(Vector3 destination, bool ignoreCollision = false)
        {
            if (LockMovement) return;
            if (!allowVRTeleportation && CurrentControlScheme == CustomControlScheme.VR) return;
            StartCoroutine(DashCo(destination, ignoreCollision));
        }

        private IEnumerator DashCo(Vector3 destination, bool ignoreCollision)
        {
            Vector3 startPosition = Transform.position;
            destination.y += _characterController.height / 2;
            float delta = 0;

            if (ignoreCollision) _characterController.detectCollisions = false;

            while (Vector3.Distance(Transform.position, destination) > 0.1f)
            {
                delta += Time.deltaTime * dashSpeed;
                Transform.position = Vector3.Lerp(startPosition, destination, delta);
                yield return Yielders.EndOfFrame;
            }

            if (ignoreCollision) _characterController.detectCollisions = true;

            StandUp();
        }

        private void Crouch()
        {
            StartCoroutine(CrouchCo());
        }

        private IEnumerator CrouchCo()
        {

            float delta = 0;
            float startPos = _characterController.height;
            float endPos = IsCrouched ? Rules.PlayerCrouchedHeight : Rules.PlayerHeight;

            while (Math.Abs(_characterController.height - endPos) > 0.0001f)
            {
                delta += Time.deltaTime * Rules.CrouchAnimationSpeed;
                _characterController.height = Mathf.Lerp(startPos, endPos, delta);
                HeadSet.CameraTransform.localPosition =
                    _characterController.height / 2 * Vector3.up + Rules.CameraOffset;
                yield return Yielders.EndOfFrame;
            }

        }

        #endregion

        private CustomControlScheme GetCurrentControlScheme()
        {
            switch (_playerInput.currentControlScheme)
            {
                case VRScheme:
                    return CustomControlScheme.VR;
                case MouseKeyboardScheme:
                    return CustomControlScheme.MouseKeyboard;
                case GamepadScheme:
                    return CustomControlScheme.Gamepad;
                default:
                    Debug.LogError($"Invalid control scheme : {_playerInput.currentControlScheme}");
                    return CurrentControlScheme;
            }
        }

        private void OnControlSchemeChanged(InputUser user, InputUserChange change, InputDevice device)
        {
            if (_playerInput.user != user) return;
            if (change != InputUserChange.ControlSchemeChanged) return;

            UpdateControlScheme();
        }

        private void UpdateControlScheme()
        {
            CustomControlScheme previousScheme = CurrentControlScheme;
            CurrentControlScheme = GetCurrentControlScheme();
            
            if (previousScheme == CustomControlScheme.Unassigned)
            {
                if (initialScheme == CustomControlScheme.VR)
                {
                    SetVRState(true);
                    SetVRSettings();
                }
                else
                {
                    SetVRState(false);
                    SetNonVRSettings();
                }
            }
            else if (CurrentControlScheme == CustomControlScheme.VR)
            {
                SetVRState(true);
                SetVRSettings();
            }
            else if (previousScheme == CustomControlScheme.VR)
            {
                SetVRState(false);
                SetNonVRSettings();
            }
        }

        private void SetVRSettings()
        {
            //XRGeneralSettings.Instance.Manager.StartSubsystems();
        }

        private void SetNonVRSettings()
        {
            //XRGeneralSettings.Instance.Manager.StopSubsystems(); // Throws an error with URP ?
            _currentLocalRotationEulers.x = 0;
            _currentLocalRotationEulers.y = 0;
            _currentLocalRotationEulers.z = 0;
        }


        private void SetVRState(bool vr)
        {
            CursorManager.Instance.SetCanvasActive(!vr);
            OnToggleVR?.Invoke(vr);
        }


        #region Inputs

        public void OnResetTrackingSpace(InputAction.CallbackContext ctx)
        {
            if (!ctx.performed) return;

            HeadSet.SetTrackingSpace(_characterController.height);
        }

        public void OnSwitchKeyboardGamepad(InputAction.CallbackContext ctx)
        {
            if (!ctx.performed) return;
            if (CurrentControlScheme == CustomControlScheme.Gamepad)
            {
                if (allowMouseKeyboard) _playerInput.SwitchCurrentControlScheme(MouseKeyboardScheme);
            }
            else if (CurrentControlScheme == CustomControlScheme.MouseKeyboard)
            {
                if (allowGamepad) _playerInput.SwitchCurrentControlScheme(GamepadScheme);
            }
        }

        public void OnUserPresence(InputAction.CallbackContext ctx)
        {
            if (!ctx.performed) return;

            bool userPresence = Mathf.Approximately(ctx.ReadValue<float>(), 1);

            if (userPresence)
            {
                if (allowVR)
                {
                    _playerInput.SwitchCurrentControlScheme(VRScheme);
                    HeadSet.SetTrackingSpace(_characterController.height);
                }
            }
            else
            {
                if (allowMouseKeyboard) _playerInput.SwitchCurrentControlScheme(MouseKeyboardScheme);
                else if (allowGamepad) _playerInput.SwitchCurrentControlScheme(GamepadScheme);
            }
        }

        public void OnEscape(InputAction.CallbackContext ctx)
        {
            if (!ctx.performed) return;
            // TODO : depends on project
        }

        public void OnCrouch(InputAction.CallbackContext ctx)
        {
            if (!ctx.performed) return;

            if (IsSitting) return;

            IsCrouched = Mathf.Approximately(ctx.ReadValue<float>(), 1);
            Crouch();
        }

        public void OnSprint(InputAction.CallbackContext ctx)
        {
            if (!ctx.performed) return;

            IsSprinting = Mathf.Approximately(ctx.ReadValue<float>(), 1);
        }

        #region Mouse

        public void OnActionPrimary(InputAction.CallbackContext ctx)
        {
            if (!ctx.performed) return;
            bool clickState = Mathf.Approximately(ctx.ReadValue<float>(), 1);
            CursorManager.Instance.SetClickState(clickState);
            if(clickState) MouseController.PrimaryAction();
        }

        public void OnActionSecondary(InputAction.CallbackContext ctx)
        {
            if (!ctx.performed) return;
            if(Mathf.Approximately(ctx.ReadValue<float>(), 1)) MouseController.SecondaryAction();
        }

        #endregion

        #region LeftController

        public void OnLeftControllerTrigger(InputAction.CallbackContext ctx)
        {
            if (!ctx.performed) return;
            LeftHandController.Trigger = ctx.ReadValue<float>();
        }

        public void OnLeftControllerGrip(InputAction.CallbackContext ctx)
        {
            if (!ctx.performed) return;
            LeftHandController.Grip = ctx.ReadValue<float>();
        }

        public void OnLeftControllerPrimaryButton(InputAction.CallbackContext ctx)
        {
            if (!ctx.performed) return;
            LeftHandController.PrimaryAction();
        }

        public void OnLeftControllerSecondaryButton(InputAction.CallbackContext ctx)
        {
            if (!ctx.performed) return;
            LeftHandController.SecondaryAction();
        }

        public void OnOpenModuleSelector(InputAction.CallbackContext ctx)
        {
            if (!ctx.performed) return;
            
            // TODO : selection ui and whatnot
        }

        #endregion LeftController

        #region RightController

        public void OnRightControllerTrigger(InputAction.CallbackContext ctx)
        {
            if (!ctx.performed) return;
            RightHandController.Trigger = ctx.ReadValue<float>();
        }

        public void OnRightControllerGrip(InputAction.CallbackContext ctx)
        {
            if (!ctx.performed) return;
            RightHandController.Grip = ctx.ReadValue<float>();
        }

        public void OnRightControllerPrimaryButton(InputAction.CallbackContext ctx)
        {
            if (!ctx.performed) return;
            RightHandController.PrimaryAction();
        }

        public void OnRightControllerSecondaryButton(InputAction.CallbackContext ctx)
        {
            if (!ctx.performed) return;
            RightHandController.SecondaryAction();
        }

        #endregion RightController

        #endregion Inputs

    }

    public enum CustomControlScheme
    {
        VR,
        MouseKeyboard,
        Gamepad,
        Unassigned
    }

}