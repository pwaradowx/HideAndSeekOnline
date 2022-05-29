using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Project.Game.Player
{
    public class Seeker : NetworkBehaviour
    {
        [SerializeField] private Camera cam;
        [SerializeField] private RectTransform hitEffect;
        
        private PlayerInput _playerInput;

        private bool _shouldFire;
        private float _damage = 0.75f;
        private const float FireDistance = 100f;

        private bool _hiderGotHit;
        private readonly Vector3 _effectSmallScale = new Vector3(1f, 1f, 1f);
        private readonly Vector3 _effectBigScale = new Vector3(1.1f, 1.1f, 1.1f);
        
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            _playerInput = GetComponent<PlayerInput>();

            _playerInput.actions["Fire"].started += callback => _shouldFire = true;
            _playerInput.actions["Fire"].canceled += callback =>
            {
                _shouldFire = false;
                hitEffect.localScale = _effectSmallScale;
                hitEffect.gameObject.SetActive(false);
            };
            hitEffect.localScale = _effectSmallScale;
            hitEffect.gameObject.SetActive(false);
        }

        private void FixedUpdate()
        {
            if (!IsOwner) return;
            
            if (!_shouldFire) return;
            
            HandleFire();
            HandleHitEffect();
        }

        private void HandleFire()
        {
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out var hit, FireDistance))
            {
                _hiderGotHit = hit.collider.TryGetComponent(out Hider hider);
                if (!_hiderGotHit) return;
                
                InflictDamageServerRpc(hider.NetworkObjectId);
            }
        }

        private void HandleHitEffect()
        {
            if (_hiderGotHit)
            {
                if (!hitEffect.gameObject.activeSelf)
                {
                    hitEffect.gameObject.SetActive(true);
                }

                hitEffect.localScale = hitEffect.localScale == _effectSmallScale ? _effectBigScale : _effectSmallScale;
            }
            else
            {
                if (hitEffect.gameObject.activeSelf)
                {
                    hitEffect.gameObject.SetActive(false);
                }
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
