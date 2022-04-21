using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using Unity.Netcode.Transports.UTP;
using UnityEngine.SceneManagement;
using Project.Global;

namespace Project.RoomConnection
{
    public class RoomConnection : MonoBehaviour
    {
        [SerializeField] private TMP_InputField ipAddressField;
        [SerializeField] private TMP_InputField passwordField;
        [SerializeField] private TMP_InputField usernameField;
        [SerializeField] private TextMeshProUGUI incorrectNameWarning;

        [SerializeField] private ResetNameButton resetNameButton;

        private bool _isItNewAccount;

        private const string PasswordKey = "RoomPassword";

        public void HostButtonClick()
        {
            NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
            
            SavePassword(passwordField.text);

            string localIP = GetLocalIP();
            NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.ServerListenAddress = localIP;
            NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Address = localIP;

            NetworkManager.Singleton.StartHost();
        }
        
        public void JoinButtonClick()
        {
            if (_isItNewAccount)
            {
                if (usernameField.text == "" || usernameField.text.Length > 10)
                {
                    incorrectNameWarning.gameObject.SetActive(true);
                    return;
                }
                // If it is a new account give name field text as a name parameter.
                NetworkManager.Singleton.NetworkConfig.ConnectionData = Encoding.ASCII.GetBytes(
                    $"{passwordField.text} {usernameField.text}");
            }
            else
            {
                // If some name has been loaded from json give it as a name parameter.
                NetworkManager.Singleton.NetworkConfig.ConnectionData = Encoding.ASCII.GetBytes(
                    $"{passwordField.text} {UserConfig.Instance.UserName}");
            }
            NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Address =
                ipAddressField.text;

            NetworkManager.Singleton.StartClient();
        }

        private void ApprovalCheck(byte[] connectionData, ulong userID, NetworkManager.ConnectionApprovedDelegate callback)
        {
            bool approval = Encoding.ASCII.GetString(connectionData).Split()[0] == GetPassword();

            if (approval)
            {
                UserConfig.Instance.TryToSetName(userID, Encoding.ASCII.GetString(connectionData).Split()[1]);
                
                callback(false, null, true, Vector3.zero, Quaternion.identity);
            }
            else
            {
                bool isHost = NetworkManager.Singleton.LocalClientId == userID;

                if (isHost)
                {
                    if (_isItNewAccount)
                    {
                        if (usernameField.text == "" || usernameField.text.Length > 10)
                        {
                            incorrectNameWarning.gameObject.SetActive(true);
                            NetworkManager.Singleton.Shutdown();
                            return;
                        }
                        UserConfig.Instance.TryToSetName(userID, usernameField.text);
                    }
                    else UserConfig.Instance.TryToSetName(userID, UserConfig.Instance.UserName);
                }

                callback(false, null, isHost, Vector3.zero, Quaternion.identity);
            }
        }

        private void SavePassword(string password) => PlayerPrefs.SetString(PasswordKey, password);
        
        private string GetPassword() => PlayerPrefs.GetString(PasswordKey);

        private string GetLocalIP()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        private void Start()
        {
            NetworkManager.Singleton.OnClientConnectedCallback += GoToRoom;

            string username = UserConfig.Instance.UserName;
            _isItNewAccount = username == "";
            usernameField.gameObject.SetActive(username == "");

            resetNameButton.OnUserNameResetEvent += () =>
            {
                _isItNewAccount = true;
                usernameField.gameObject.SetActive(true);
            };
        }

        private void GoToRoom(ulong userID)
        {
            if (userID != NetworkManager.Singleton.LocalClientId) return;

            if (_isItNewAccount)
            {
                UserConfig.Instance.UserName = usernameField.text;
                UserConfig.Instance.SaveUserData(usernameField.text);
            }
            NetworkManager.Singleton.SceneManager.LoadScene("Room", LoadSceneMode.Single);
        }
    }
}
