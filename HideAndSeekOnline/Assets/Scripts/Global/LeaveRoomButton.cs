using UnityEngine.EventSystems;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Project.Global
{
    public class LeaveRoomButton : NetworkBehaviour, IPointerClickHandler
    {
        public void OnPointerClick(PointerEventData eventData)
        {
            if (NetworkManager.Singleton.IsHost)
            {
                // Firstly disconnect all connected clients.
                for (int i = 0; i < NetworkManager.Singleton.ConnectedClientsList.Count; i++)
                {
                    ulong id = NetworkManager.Singleton.ConnectedClientsList[i].ClientId;
                    
                    if (NetworkManager.Singleton.LocalClientId == id) continue;
                    
                    RequestPlayerDespawnServerRpc(id);

                    NetworkManager.Singleton.DisconnectClient(id);
                }
                
                // Then disconnect host itself.
                RequestPlayerDespawnServerRpc(OwnerClientId);

                DisconnectUser(OwnerClientId);

                UserConfig.Instance.IsGameStarted = false;
            }
            else
            {
                // Disconnect single client.
                RequestPlayerDespawnServerRpc(OwnerClientId);
                
                DisconnectUser(OwnerClientId);
            }
        }
        
        [ServerRpc]
        private void RequestPlayerDespawnServerRpc(ulong id)
        {
            foreach (var clientID in NetworkManager.Singleton.ConnectedClientsIds)
            {
                if (clientID == id)
                {
                    var player = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(id);
                    if (player != null) player.Despawn();
                }
                break;
            }
        }

        private void Start()
        {
            if (NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsHost)
            {
                NetworkManager.Singleton.OnClientDisconnectCallback += DisconnectUser;
            }
        }
        
        private void DisconnectUser(ulong id)
        {
            Cursor.lockState = CursorLockMode.None;

            NetworkManager.Singleton.Shutdown();
            Destroy(NetworkManager.Singleton.gameObject);
            
            if (NetworkManager.Singleton) NetworkManager.Singleton.OnClientDisconnectCallback -= DisconnectUser;
                    
            SceneManager.LoadScene("RoomConnection", LoadSceneMode.Single);
        }
    }
}
