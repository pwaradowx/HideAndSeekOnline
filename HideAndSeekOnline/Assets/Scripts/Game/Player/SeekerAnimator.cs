using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Project.Game.Player
{
    public class SeekerAnimator : NetworkBehaviour
    {
        [SerializeField] private Camera cam;
        [SerializeField] private Animator seekerAnimator;
        [SerializeField] private Transform body;
        [SerializeField] private Transform lookTarget;

        private PlayerController _playerController;
        private PlayerInput _playerInput;
        
        private Ray _lookRay;
        private const float LookTargetOffset = 20f;

        private int _dirXAnimHash;
        private int _dirYAnimHash;

        private float _targetRotation;
        private float _rotationVelocity;
        private const float RotationSmoothTime = 0.1f;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            
            _playerController = GetComponent<PlayerController>();
            _playerInput = GetComponent<PlayerInput>();
            
            HashAnimatorVariables();
        }

        private void HashAnimatorVariables()
        {
            _dirXAnimHash = Animator.StringToHash("DirectionX");
            _dirYAnimHash = Animator.StringToHash("DirectionY");
        }

        private void FixedUpdate()
        {
            if (!IsOwner || PlayerEntity.IsGamePaused) return;
            
            HandleLookTarget();
            
            HandleMovementAnimations();
        }
        
        private void HandleLookTarget()
        {
            _lookRay = new Ray(cam.transform.position, cam.transform.forward);
            lookTarget.transform.position = _lookRay.GetPoint(LookTargetOffset);
        }

        private void HandleMovementAnimations()
        {
            if (_playerInput.actions["Move"].phase != InputActionPhase.Disabled)
            {
                _targetRotation = cam.transform.eulerAngles.y;
                float rotationAngle = Mathf.SmoothDampAngle(
                    body.eulerAngles.y, _targetRotation, ref _rotationVelocity, RotationSmoothTime);
                body.rotation = Quaternion.Euler(0f, rotationAngle, 0f);
            }
            
            float dirX = Vector3.Dot(_playerController.TargetDirection.normalized, cam.transform.right);
            float dirY = Vector3.Dot(_playerController.TargetDirection.normalized, 
                Quaternion.Euler(0f, cam.transform.eulerAngles.y, 0f) * Vector3.forward);
            
            UpdateAnimatorServerRpc(dirX, dirY);
        }

        [ServerRpc(RequireOwnership = false)]
        private void UpdateAnimatorServerRpc(float dirX, float dirY)
        {
            UpdateAnimatorClientRpc(dirX, dirY);
        }

        [ClientRpc]
        private void UpdateAnimatorClientRpc(float dirX, float dirY)
        {
            seekerAnimator.SetFloat(_dirXAnimHash, dirX, 0.1f, Time.deltaTime);
            seekerAnimator.SetFloat(_dirYAnimHash, dirY, 0.1f, Time.deltaTime);
        }
    }
}
