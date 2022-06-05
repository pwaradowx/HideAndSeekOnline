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
                    
                    var clientObject = NetworkManager.Singleton.ConnectedClients[id].PlayerObject;
                    if (clientObject != null) clientObject.Despawn();

                    NetworkManager.Singleton.DisconnectClient(id);
                }
                
                // Then disconnect host itself.
                var hostObject = NetworkManager.Singleton.ConnectedClients[OwnerClientId].PlayerObject;
                if (hostObject != null) hostObject.Despawn();

                NetworkManager.Singleton.Shutdown();
                Destroy(NetworkManager.Singleton.gameObject);

                UserConfig.Instance.IsGameStarted = false;
                
                SceneManager.LoadScene("RoomConnection", LoadSceneMode.Single);
            }
            else
            {
                // Disconnect single client.
                Cursor.lockState = CursorLockMode.None;
                
                RequestClientDisconnectServerRpc(OwnerClientId);
                
                NetworkManager.Singleton.Shutdown();
                Destroy(NetworkManager.Singleton.gameObject);
                
                SceneManager.LoadScene("RoomConnection", LoadSceneMode.Single);
            }
        }
        
        [ServerRpc]
        private void RequestClientDisconnectServerRpc(ulong id)
        {
            foreach (var clientID in NetworkManager.Singleton.ConnectedClientsIds)
            {
                if (clientID == id) NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(id).Despawn();
                break;
            }
        }
    }
}
