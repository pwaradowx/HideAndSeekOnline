using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Project.Game.Player
{
    public class Hider : NetworkBehaviour
    {
        [SerializeField] private Transform body;

        private PlayerInput _playerInput;

        private bool _shouldRotate;

        private float _health;
        private const float MaxHealth = 100f;
        
        private const float RotationSpeed = 100f;

        public void TakeDamage(float damage)
        {
            _health -= damage;

            if (_health <= 0)
            {
                NetworkManager.Singleton.SpawnManager.SpawnedObjects[NetworkObjectId].Despawn();
            }
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            _playerInput = GetComponent<PlayerInput>();

            _playerInput.actions["Rotate"].started += callback => _shouldRotate = true;
            _playerInput.actions["Rotate"].canceled += callback => _shouldRotate = false;

            _health = MaxHealth;
        }

        private void FixedUpdate()
        {
            if (!_shouldRotate) return;
            
            HandleBodyRotation();
        }

        private void HandleBodyRotation()
        {
            body.transform.Rotate(Vector3.up * RotationSpeed * Time.deltaTime);
        }
    }
}
