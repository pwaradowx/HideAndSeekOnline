using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Project.Global;

namespace Project.Room
{
    public class RoomManager : NetworkBehaviour
    {
        [SerializeField] private TextMeshProUGUI ipHolder;
        [SerializeField] private RoomPlayerCard[] roomPlayerCards;
        [SerializeField] private GameObject startGameButton;
        
        [SerializeField] private bool isItDevBuild;

        private NetworkList<RoomPlayerState> _roomPlayers;

        public void OnStartGameButtonClicked()
        {
            StartGameServerRpc();
        }
        
        public void OnReadyButtonClicked()
        {
            ToggleReadyServerRpc();
        }

        [ServerRpc(RequireOwnership = false)]
        private void StartGameServerRpc(ServerRpcParams serverParams = default)
        {
            if (serverParams.Receive.SenderClientId != NetworkManager.Singleton.LocalClientId) return;

            if (!IsEveryoneReady()) return;

            UserConfig.Instance.IsGameStarted = true;
            NetworkManager.Singleton.SceneManager.LoadScene("House", LoadSceneMode.Single);
        }
        
        [ServerRpc(RequireOwnership = false)]
        private void ToggleReadyServerRpc(ServerRpcParams serverParams = default)
        {
            for (int i = 0; i < _roomPlayers.Count; i++)
            {
                if (_roomPlayers[i].UserID == serverParams.Receive.SenderClientId)
                {
                    _roomPlayers[i] = new RoomPlayerState(
                        _roomPlayers[i].UserName, _roomPlayers[i].UserID, !_roomPlayers[i].IsReady);
                }
            }
        }
        
        private void OnEnable()
        {
            _roomPlayers = new NetworkList<RoomPlayerState>();
            
            if (NetworkManager.Singleton.IsClient)
            {
                _roomPlayers.OnListChanged += HandleRoomPlayersStateChanged;
            }

            if (NetworkManager.Singleton.IsServer)
            {
                startGameButton.SetActive(true);

                NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
                NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnected;

                foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
                {
                    HandleClientConnected(client.ClientId);
                }
            }

            ipHolder.text = NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Address;
        }
        
        private void HandleRoomPlayersStateChanged(NetworkListEvent<RoomPlayerState> roomState)
        {
            for (int i = 0; i < roomPlayerCards.Length; i++)
            {
                if (_roomPlayers.Count > i)
                {
                    roomPlayerCards[i].UpdateDisplay(_roomPlayers[i]);
                }
                else
                {
                    roomPlayerCards[i].DisableDisplay();
                }
            }

            if (NetworkManager.Singleton.IsHost)
            {
                startGameButton.GetComponent<Button>().interactable = IsEveryoneReady();
            }
        }
        
        private void HandleClientConnected(ulong clientID)
        {
            _roomPlayers.Add(new RoomPlayerState(UserConfig.Instance.GetNameByID(clientID), clientID, false));
        }
        
        private void HandleClientDisconnected(ulong clientID)
        {
            for (int i = 0; i < _roomPlayers.Count; i++)
            {
                if (_roomPlayers[i].UserID == clientID)
                {
                    _roomPlayers.RemoveAt(i);
                    break;
                }
            }
        }

        private bool IsEveryoneReady()
        {
            if (isItDevBuild) return true;
            
            if (_roomPlayers.Count < 2) return false;

            foreach (var player in _roomPlayers)
            {
                if (!player.IsReady) return false;
            }

            return true;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            
            _roomPlayers.OnListChanged -= HandleRoomPlayersStateChanged;

            if (NetworkManager.Singleton)
            {
                NetworkManager.Singleton.OnClientConnectedCallback -= HandleClientConnected;
                NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnected;
            }
        }
    }
}
