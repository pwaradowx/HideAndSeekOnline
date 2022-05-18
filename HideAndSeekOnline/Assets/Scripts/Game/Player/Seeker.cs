using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Project.Game.Player
{
    public class Seeker : NetworkBehaviour
    {
        [SerializeField] private Camera cam;
        
        private PlayerInput _playerInput;

        private bool _shouldFire;
        private float _damage = 0.75f;
        private const float FireDistance = 100f;
        
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            _playerInput = GetComponent<PlayerInput>();

            _playerInput.actions["Fire"].started += callback => _shouldFire = true;
            _playerInput.actions["Fire"].canceled += callback => _shouldFire = false;
        }

        private void FixedUpdate()
        {
            if (!_shouldFire) return;
            
            HandleFire();
        }

        private void HandleFire()
        {
            print("Here");
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out var hit, FireDistance))
            {
                if (!hit.collider.TryGetComponent(out Hider hider)) return;
                
                InflictDamageServerRpc(hider.NetworkObjectId);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void InflictDamageServerRpc(ulong objectID)
        {
            InflictDamageClientRpc(objectID);
        }

        [ClientRpc]
        private void InflictDamageClientRpc(ulong objectID)
        {
            NetworkManager.Singleton.SpawnManager.SpawnedObjects[objectID].GetComponent<Hider>().TakeDamage(_damage);
        }
    }
}
