using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using Unity.Netcode.Transports.UTP;

namespace Project.RoomConnection
{
    public class RoomConnection : MonoBehaviour
    {
        [SerializeField] private TMP_InputField ipAddressField;
        [SerializeField] private TMP_InputField passwordField;
        [SerializeField] private TextMeshProUGUI ipAddress;

        private const string PasswordKey = "RoomPassword";

        public void HostButtonClick()
        {
            NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
            
            SavePassword(passwordField.text);

            string localIP = GetLocalIP();
            ipAddress.text = localIP;
            NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.ServerListenAddress = localIP;
            NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Address = localIP;

            NetworkManager.Singleton.StartHost();
        }
        
        public void JoinButtonClick()
        {
            NetworkManager.Singleton.NetworkConfig.ConnectionData = Encoding.ASCII.GetBytes(passwordField.text);
            NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Address =
                ipAddressField.text;

            NetworkManager.Singleton.StartClient();
        }

        private void ApprovalCheck(byte[] connectionData, ulong userID, NetworkManager.ConnectionApprovedDelegate callback)
        {
            bool approval = Encoding.ASCII.GetString(connectionData) == GetPassword();

            if (approval)
            {
                callback(true, null, true, Vector3.zero, Quaternion.identity);
            }
            else
            {
                bool isHost = NetworkManager.Singleton.LocalClientId == userID;

                callback(isHost, null, isHost, Vector3.zero, Quaternion.identity);
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
    }
}
