using UnityEngine;
using UnityEngine.InputSystem;

namespace Jichaels.VRSDK
{

    public class JVRMouseController : MonoBehaviour, IInteractionController
    {
        public JVRPlayer Player { get; private set; }

        [SerializeField] private LayerMask layerMask;

        private bool _leftMouseButtonDown;
        private bool _rightMouseButtonDown;

        private Ray _ray;
        private RaycastHit _hit;
        
        private Vector2 _mousePosition;

        private IJVRMouseInteract _jvrMouseInteract;
        private bool _isHovering;

        public void InitializeController(JVRPlayer player)
        {
            Player = player;
            Player.OnToggleVR += OnToggleVR;
        }

        private void OnDestroy()
        {
            Player.OnToggleVR -= OnToggleVR;
        }

        private void OnToggleVR(bool vr)
        {
            enabled = vr;
        }

        public void UpdateController()
        {
            _mousePosition = InputsManager.Instance.MousePosition;
            
            _ray = Player.HeadSet.Camera.ScreenPointToRay(_mousePosition);
            if (Physics.Raycast(_ray, out _hit, Rules.MouseActionMaxDistance, layerMask))
            {
                IJVRMouseInteract jvrMouseInteract = _hit.collider.GetComponent<IJVRMouseInteract>();

                if (jvrMouseInteract == _jvrMouseInteract)
                {
                    if(_isHovering) _jvrMouseInteract.MouseHoverStay(this);
                }
                else
                {
                    if (_isHovering)
                    {
                        _jvrMouseInteract.MouseHoverExit(this);
                    }
                
                    _jvrMouseInteract = jvrMouseInteract;

                    if (_jvrMouseInteract != null)
                    {
                        _isHovering = true;
                        _jvrMouseInteract.MouseHoverEnter(this);
                        CursorManager.Instance.SetCursor(_jvrMouseInteract.HoverCursor);
                    }
                    else
                    {
                        _isHovering = false;
                        CursorManager.Instance.ResetDefaultCursor();
                    }
                }
            }
            else
            {
                if (_isHovering)
                {
                    _jvrMouseInteract.MouseHoverExit(this);
                    _jvrMouseInteract = null;
                    CursorManager.Instance.ResetDefaultCursor();
                }
                _isHovering = false;
            }
        }

        public void PrimaryAction()
        {
            if (!_isHovering) return;

            if (_jvrMouseInteract.DisableInteraction) return;
            
            _jvrMouseInteract.PrimaryInteraction(this);
        }

        public void SecondaryAction()
        {
            if (!_isHovering) return;

            if (_jvrMouseInteract.DisableInteraction) return;
            
            _jvrMouseInteract.SecondaryInteraction(this);
        }
    }
}