using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Project.RoomConnection
{
    public class DisconnectHandler : MonoBehaviour
    {
        private void Start()
        {
            if (NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsHost)
            {
                NetworkManager.Singleton.OnClientDisconnectCallback += DisconnectClient;
            }
        }

        private void DisconnectClient(ulong id)
        {
            Cursor.lockState = CursorLockMode.None;

            NetworkManager.Singleton.Shutdown();
            Destroy(NetworkManager.Singleton.gameObject);
                    
            SceneManager.LoadScene("RoomConnection", LoadSceneMode.Single);
        }

        private void OnDestroy()
        {
            // TODO: It's still a bad place to unsubscribe. Logically I should unsubscribe on disconnection.
            if (NetworkManager.Singleton) NetworkManager.Singleton.OnClientDisconnectCallback -= DisconnectClient;
        }
    }
}
