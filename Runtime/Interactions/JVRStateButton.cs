using System;
using System.Collections;
using Jichaels.Core;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Jichaels.VRSDK
{

    public class JVRStateButton : MonoBehaviour, IJVRFingerInteract, IJVRMouseInteract
    {

        [InlineButton("SnapDefaultPosition", "Snap"), InlineButton("SetDefaultPosition", "Set")]
        [SerializeField] private Vector3 defaultPosition;
        [InlineButton("SnapPressedPosition", "Snap"), InlineButton("SetPressedPosition", "Set")]
        [SerializeField] private Vector3 pressedPosition;
        [InlineButton("SnapUnpressPosition", "Snap"), InlineButton("SetUnpressPosition", "Set")]
        [SerializeField] private Vector3 unpressPosition;

        [SerializeField] private InteractionDirection pushDirection;        

        [SerializeField] private float springEffectMultiplier = 0.5f;
        [SerializeField] private float clickDelay = 0.2f;

        [SerializeField] private CursorInfo cursorInfo;
        
        
        public bool Pressed { get; private set; }
        public event Action<JVRStateButton, bool> OnStateChange;

        private bool _changedState;
        private Transform _transform;

        private float _delta;
        private float _delay;
        private Coroutine _coroutine;
        private bool _coroutineRunning;

        // Should never rotate, if it does, don't cache it
        private Vector3 _pushDirection;

        private void Awake()
        {
            _transform = transform;
            _transform.localPosition = Pressed ? pressedPosition : defaultPosition;

            switch (pushDirection)
            {
                case InteractionDirection.Forward:
                    _pushDirection = _transform.forward;
                    break;
                case InteractionDirection.Backward:
                    _pushDirection = -_transform.forward;
                    break;
                case InteractionDirection.Up:
                    _pushDirection = _transform.up;
                    break;
                case InteractionDirection.Down:
                    _pushDirection = -_transform.up;
                    break;
                case InteractionDirection.Left:
                    _pushDirection = -_transform.right;
                    break;
                case InteractionDirection.Right:
                    _pushDirection = _transform.right;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void MoveButton(Vector3 position)
        {
            if (_changedState)
            {
                if (_delay > 0)
                {
                    _delay -= Time.deltaTime;
                    return;
                }
            }

            _changedState = false;
            _transform.localPosition = Vector3.MoveTowards(_transform.localPosition, position, _delta);

            if (!Pressed && _transform.localPosition == pressedPosition)
            {
                Pressed = true;
                _changedState = true;
            }
            else if (Pressed && _transform.localPosition == unpressPosition)
            {
                Pressed = false;
                _changedState = true;
            }

            if (_changedState)
            {
                _delay = clickDelay;
                OnStateChange?.Invoke(this, Pressed);
            }
        }

        public void UnpressButton()
        {
            Pressed = false;
            OnStateChange?.Invoke(this, Pressed);
            _transform.localPosition = defaultPosition;
        }

        public void PressButton()
        {
            Pressed = true;
            OnStateChange?.Invoke(this, Pressed);
            _transform.localPosition = pressedPosition;
        }

        private IEnumerator SpringEffectCo()
        {
            while (_transform.localPosition != (Pressed ? pressedPosition : defaultPosition))
            {
                _transform.localPosition = Vector3.MoveTowards(_transform.localPosition,
                    Pressed ? pressedPosition : defaultPosition, Time.deltaTime * springEffectMultiplier);

                yield return Yielders.EndOfFrame;
            }

            _coroutineRunning = false;
        }

        #region IJVRMouseInteract

        public void PrimaryInteraction(JVRMouseController mouseController)
        {
            if (Pressed)
            {
                UnpressButton();
            }
            else
            {
                PressButton();
            }
        }

        public void SecondaryInteraction(JVRMouseController mouseController) { }

        public void MouseHoverEnter(JVRMouseController mouseController) { }

        public void MouseHoverStay(JVRMouseController mouseController) { }

        public void MouseHoverExit(JVRMouseController mouseController) { }
        public CursorInfo HoverCursor => cursorInfo;

        #endregion

        #region IJVRFingerInteract

        public void InteractionStart(JVRFingerController jvrFingerController)
        {
            if (_coroutineRunning) StopCoroutine(_coroutine);
        }

        public void InteractionStay(JVRFingerController jvrFingerController)
        {
            _delta = Vector3.Dot(jvrFingerController.DeltaPositionWorld, _pushDirection);
            if (_delta > 0)
            {
                MoveButton(Pressed ? unpressPosition : pressedPosition);
            }
        }

        public void InteractionStop(JVRFingerController jvrFingerController)
        {
            _coroutineRunning = true;
            _coroutine = StartCoroutine(SpringEffectCo());
        }

        #endregion

        public bool DisableInteraction { get; set; }
        
        
        #if UNITY_EDITOR

        public void SetDefaultPosition()
        {
            defaultPosition = transform.localPosition;
        }

        public void SnapDefaultPosition()
        {
            transform.localPosition = defaultPosition;
        }
        
        public void SetPressedPosition()
        {
            pressedPosition = transform.localPosition;
        }

        public void SnapPressedPosition()
        {
            transform.localPosition = pressedPosition;
        }
        
        public void SetUnpressPosition()
        {
            unpressPosition = transform.localPosition;
        }

        public void SnapUnpressPosition()
        {
            transform.localPosition = unpressPosition;
        }
        
        #endif
    }

    public enum InteractionDirection
    {
        Forward,
        Backward,
        Up,
        Down,
        Left,
        Right
    }
}