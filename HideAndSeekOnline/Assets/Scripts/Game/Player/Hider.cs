using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Project.Game.Player
{
    public class Hider : NetworkBehaviour
    {
        [SerializeField] private Camera cam;
        [SerializeField] private Transform body;
        [SerializeField] private Slider healthBar;

        public delegate void Die(ulong id, Vector3 deathPlace);
        public static event Die OnDieCallback;

        private PlayerInput _playerInput;
        
        private float _health;
        private const int MaxHealth = 15;
        private bool _deadAlready;
        
        private const float SwapDistance = 25f;
        
        private bool _shouldRotate;
        private const float RotationSpeed = 100f;

        public void TakeDamage(float damage)
        {
            _health -= damage;

            healthBar.value = _health;

            if (_health <= 0 && !_deadAlready)
            {
                _deadAlready = true;
                OnDieCallback?.Invoke(OwnerClientId, transform.position);
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
            if (!IsOwner || PlayerEntity.IsGamePaused) return;
            
            if (_playerInput.actions["Swap"].phase == InputActionPhase.Performed) HandleBodySwap();
            if (_shouldRotate) HandleBodyRotation();
        }

        private void HandleBodySwap()
        {
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out var hit, SwapDistance))
            {
                if (!hit.collider.TryGetComponent(out Swappable swappable)) return;

                HandleBodySwapServerRpc(NetworkObjectId, 
                    swappable.GetComponent<NetworkObject>().NetworkObjectId);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void HandleBodySwapServerRpc(ulong playerObjectID, ulong swappableObjectID)
        {
            HandleBodySwapClientRpc(playerObjectID, swappableObjectID);
        }

        [ClientRpc]
        private void HandleBodySwapClientRpc(ulong playerObjectID, ulong swappableObjectID)
        {
            var meshFilter = NetworkManager.Singleton.SpawnManager.SpawnedObjects[playerObjectID].transform.GetChild(0)
                .GetComponent<MeshFilter>();
            var meshRenderer = NetworkManager.Singleton.SpawnManager.SpawnedObjects[playerObjectID].transform.GetChild(0)
                .GetComponent<MeshRenderer>();
            var character = NetworkManager.Singleton.SpawnManager.SpawnedObjects[playerObjectID].transform
                .GetComponent<CharacterController>();
            var feet = NetworkManager.Singleton.SpawnManager.SpawnedObjects[playerObjectID].transform.GetChild(1);
            var swappable = NetworkManager.Singleton.SpawnManager.SpawnedObjects[swappableObjectID]
                .GetComponent<Swappable>();
            
            meshFilter.mesh = swappable.MyMesh;
            meshRenderer.materials = swappable.MyMeshRenderer.materials;
            character.radius = swappable.ControllerRadius;
            character.height = swappable.ControllerHeight;
            character.skinWidth = swappable.SkinWidth;
            feet.localPosition = swappable.ModelPosition;
            body.transform.localPosition = swappable.ModelPosition;
            body.transform.localScale = new Vector3(2f, 2f, 2f);
            healthBar.maxValue = swappable.Health;
            healthBar.value = swappable.Health;
            _health = swappable.Health;
        }

        private void HandleBodyRotation()
        {
            body.transform.Rotate(Vector3.up * RotationSpeed * Time.deltaTime);
        }
    }
}
