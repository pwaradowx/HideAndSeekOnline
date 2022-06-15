using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Project.Game.Player
{
    public class DeathCamera : NetworkBehaviour
    {
        private CharacterController _character;
        private Camera _cam;
        private PlayerInput _playerInput;

        private float _camX;
        private float _camY;

        private const float Speed = 0.5f;
        private const float SensitivityX = 10f;
        private const float SensitivityY = 10f;
        private const float TopClamp = 90f;
        private const float BottomClamp = -90f;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (!IsOwner) return;

            _character = GetComponent<CharacterController>();
            _cam = GetComponent<Camera>();
            _playerInput = GetComponent<PlayerInput>();
        }

        private void FixedUpdate()
        {
            if (!IsOwner) return;
            
            HandleMovement();
            HandleRotation();
        }

        private void HandleMovement()
        {
            if (_playerInput.actions["Move"].phase == InputActionPhase.Disabled) return;

            Vector2 input = _playerInput.actions["Move"].ReadValue<Vector2>();

            var cam = _cam.transform;
            Vector3 direction = (cam.right * input.x + cam.forward * input.y) * (Speed * Time.deltaTime);
            
            _character.Move(direction.normalized);
        }

        private void HandleRotation()
        {
            if (_playerInput.actions["Look"].phase == InputActionPhase.Disabled) return;
            
            Vector2 input = _playerInput.actions["Look"].ReadValue<Vector2>();
            
            _camX += input.x * SensitivityX * Time.deltaTime;
            _camY += input.y * SensitivityY * Time.deltaTime * -1;
                
            _camY = Mathf.Clamp(_camY, BottomClamp, TopClamp);
                
            transform.rotation = Quaternion.Euler(_camY, _camX, 0f);
        }
    }
}
