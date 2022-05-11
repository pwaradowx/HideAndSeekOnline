using UnityEngine;
using Unity.Netcode;
using Project.Global;
using Project.Game.Player;

namespace Project.Game.Server
{
    public class SpawnPlayers : NetworkBehaviour
    {
        [SerializeField] private GameObject hiderPrefab;
        [SerializeField] private GameObject seekerPrefab;

        [SerializeField] private RoleGiver roleGiver;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            
            if (!NetworkManager.Singleton.IsHost) return;
            
            Camera.main.gameObject.SetActive(false);

            foreach (var player in NetworkManager.Singleton.ConnectedClientsList)
            {
                SpawnPlayerServerRpc(player.ClientId);
            }
        }
        
        [ServerRpc(RequireOwnership = false)]
        private void SpawnPlayerServerRpc(ulong userID)
        {
            int role = roleGiver.GiveRole(NetworkManager.Singleton.ConnectedClientsList.Count);

            GameObject player;

            if (role == (int) RoleGiver.Roles.Hider)
            {
                player = Instantiate(hiderPrefab, new Vector3(Random.Range(-10f, 10f), 1f, 0f), 
                    Quaternion.identity);
            }
            else if (role == (int) RoleGiver.Roles.Seeker)
            {
                player = Instantiate(seekerPrefab, new Vector3(Random.Range(-10f, 10f), 1f, 0f), 
                    Quaternion.identity);
            }
            else
            {
                Debug.Log("Unsuccessful attempt to give role to the player!");
                return;
            }
            
            player.GetComponent<NetworkObject>().SpawnAsPlayerObject(userID, true);
            
            string username = UserConfig.Instance.GetNameByID(userID);
            SetNameToPlayerClientRpc(player.GetComponent<NetworkObject>().NetworkObjectId, username);
        }

        [ClientRpc]
        private void SetNameToPlayerClientRpc(ulong objectID, string username)
        {
            NetworkManager.Singleton.SpawnManager.SpawnedObjects[objectID].GetComponent<PlayerEntity>().Name = username;
        }
    }
}
