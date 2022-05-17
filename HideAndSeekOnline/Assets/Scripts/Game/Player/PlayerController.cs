using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Project.Game.Player
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerController : NetworkBehaviour
    {
        [Header("Camera Settings")] 
        [SerializeField] private Camera cam;
        [SerializeField] private Transform camTarget;
        [SerializeField] private float sensitivityX;
        [SerializeField] private float sensitivityY;
        [SerializeField] private float topClamp;
        [SerializeField] private float bottomClamp;

        private PlayerInput _playerInput;
        
        // Camera variables
        private float _camTargetX;
        private float _camTargetY;

        // Movement variables
        private CharacterController _character;

        private float _speed = 10f;

        private void OnEnable()
        {
            _playerInput = GetComponent<PlayerInput>();
            _character = GetComponent<CharacterController>();

            Cursor.lockState = CursorLockMode.Locked;
        }

        private void FixedUpdate()
        {
            HandlePlayerMovementAndRotation();
            HandleCameraRotation();
        }

        private void HandlePlayerMovementAndRotation()
        {
            if (!IsOwner) return;

            Vector2 input = _playerInput.actions["Move"].ReadValue<Vector2>();

            if (!(input.magnitude > 0)) return;

            Vector3 xDir = cam.transform.right * input.x;
            Vector3 zDir = Quaternion.Euler(0f, cam.transform.eulerAngles.y, 0f) * Vector3.forward * input.y;
            Vector3 targetDirection = xDir + zDir;

            _character.Move(targetDirection.normalized * _speed * Time.deltaTime);
        }

        private void HandleCameraRotation()
        {
            if (!IsOwner) return;

            Vector2 input = _playerInput.actions["Look"].ReadValue<Vector2>();
            
            _camTargetX += input.x * sensitivityX * Time.deltaTime;
            _camTargetY += input.y * sensitivityY * Time.deltaTime * -1;
                
            _camTargetY = Mathf.Clamp(_camTargetY, bottomClamp, topClamp);
                
            camTarget.transform.rotation = Quaternion.Euler(_camTargetY, _camTargetX, 0f);
        }
    }
}
