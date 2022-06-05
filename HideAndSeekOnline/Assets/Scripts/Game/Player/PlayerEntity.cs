using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Project.Game.Player
{
    public class PlayerEntity : NetworkBehaviour
    {
        [SerializeField] private GameObject playerInterface;
        [SerializeField] private TextMeshProUGUI nameplate;
        [SerializeField] private MatchMenu matchMenu;
        
        [SerializeField] private Behaviour[] componentsToDisable;

        public static bool IsGamePaused { get; private set; }
        public string Name { set => nameplate.text = value; }

        private PlayerInput _playerInput;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            
            if (IsOwner)
            {
                playerInterface.SetActive(true);
                
                matchMenu.Hide();
                
                _playerInput = GetComponent<PlayerInput>();
                _playerInput.actions["MatchMenu"].started += HandleMatchMenu;
                
                IsGamePaused = false;
            }

            if (!IsLocalPlayer)
            {
                foreach (var component in componentsToDisable)
                {
                    component.enabled = false;
                }
            }
        }

        private void HandleMatchMenu(InputAction.CallbackContext callback)
        {
            if (!IsOwner) return;
            
            if (IsGamePaused)
            {
                matchMenu.Hide();
                Cursor.lockState = CursorLockMode.Locked;
                IsGamePaused = false;
            }
            else
            {
                matchMenu.Show();
                Cursor.lockState = CursorLockMode.None;
                IsGamePaused = true;
            }
        }

        public override void OnDestroy()
        { 
            base.OnDestroy();
            
            _playerInput.actions["MatchMenu"].started -= HandleMatchMenu;
        }
    }
}
