using UnityEngine;

namespace Jichaels.VRSDK
{

    [RequireComponent(typeof(CharacterController))]
    public class JVRPlayerController : MonoBehaviour
    {
        public bool IsCrouched { get; private set; }
        public bool IsSprinting { get; private set; }
        public bool IsSitting { get; private set; }
        
        
        public bool LockMovement { get; set; }
        public bool LockRotation { get; set; }
        
        [SerializeField] private bool snapRotation;

        private Transform _transform;
        private CharacterController _characterController;

        private void Awake()
        {
            _transform = transform;
            _characterController = GetComponent<CharacterController>();
            _characterController.height = Rules.PlayerHeight;
        }


        

        public void Move(Vector3 movement)
        {
            if (LockMovement) return;
            
            _characterController.SimpleMove(movement);
        }
        
        public void Rotate(float rotation)
        {
            if (LockRotation) return;
            
            _transform.Rotate(Vector3.up, rotation);
        }
        
        public void Climb(Vector3 motion)
        {
            if (LockMovement) return;

            _characterController.Move(motion);
        }

        public void Teleport(Vector3 destination)
        {
            
        }
    }

}