using Unity.Netcode;
using UnityEngine;
using System.Threading.Tasks;
using Project.Game.Player;
using Project.Global;

namespace Project.Game.Server
{
    public class MatchLogic : NetworkBehaviour
    {
        [SerializeField] private MatchMenu matchMenu;
        [SerializeField] private GameObject barrier;
        [SerializeField] private GameObject ghostCamera;
        
        private int _aliveHidersAmount;
        
        public override async void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (!IsHost) return;

            _aliveHidersAmount = NetworkManager.Singleton.ConnectedClientsList.Count - 1;
            Hider.OnDieCallback += OnHiderDies;

            await StartHidePhase();
            await StartSeekPhase();
        }

        private void OnHiderDies(ulong id, Vector3 deathPlace)
        {
            _aliveHidersAmount -= 1;

            var player = NetworkManager.Singleton.ConnectedClients[id].PlayerObject;
            if (player != null) player.Despawn();

            if (_aliveHidersAmount > 0) SpawnGhostCamera(id, deathPlace);
            else EndOfTheMatch();
        }

        private async Task StartHidePhase()
        {
            int timeout = 30;
            
            if (matchMenu != null) matchMenu.UpdateTimerClientRpc(timeout);

            while (timeout > 0)
            {
                timeout -= 1;
                
                if (matchMenu != null) matchMenu.UpdateTimerClientRpc(timeout);
                
                if (timeout <= 0)
                {
                    DisableBarrierClientRpc();
                }

                await Task.Delay(1000);

                await Task.Yield();
            }
        }

        private async Task StartSeekPhase()
        {
            int timeout = 180;

            while (timeout > 0)
            {
                if (matchMenu != null) matchMenu.UpdateTimerClientRpc(timeout);

                timeout -= 1;
                
                if (timeout <= 0)
                {
                    EndOfTheMatch();
                }

                await Task.Delay(1000);

                await Task.Yield();
            }
        }

        [ClientRpc]
        private void DisableBarrierClientRpc()
        {
            barrier.SetActive(false);
        }

        private void EndOfTheMatch()
        {
            UnlockCursorClientRpc();
            
            foreach (var clientID in NetworkManager.Singleton.ConnectedClientsIds)
            {
                var player = NetworkManager.Singleton.ConnectedClients[clientID].PlayerObject;
                if (player.IsSpawned) player.Despawn();
            }

            if (matchMenu == null) return;
            
            matchMenu.DisableTimerClientRpc();
            matchMenu.ShowWinDeclarationClientRpc(_aliveHidersAmount > 0 ? "Hiders won!" : "Seeker won!");
            matchMenu.ShowGameEndMenuClientRpc();
        }

        [ClientRpc]
        private void UnlockCursorClientRpc()
        {
            Cursor.lockState = CursorLockMode.None;
        }

        // Method which called when hider dies.
        // It spawns the camera that can fly on map without any obstacles.
        private void SpawnGhostCamera(ulong id, Vector3 spawnPlace)
        {
            GameObject ghostCam = Instantiate(ghostCamera, spawnPlace, Quaternion.identity);

            var netGhostCam = ghostCam.GetComponent<NetworkObject>();
            if (netGhostCam != null) netGhostCam.SpawnAsPlayerObject(id, true);

            var player = netGhostCam.GetComponent<PlayerEntity>();
            if (player != null) player.Name = UserConfig.Instance.GetNameByID(id);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            Hider.OnDieCallback -= OnHiderDies;
        }
    }
}
