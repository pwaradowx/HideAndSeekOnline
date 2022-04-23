using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Project.Global
{
    public class UserConfig : MonoBehaviour
    {
        public static UserConfig Instance;

        public bool IsGameStarted { get; set; }
        public string UserName { get; set; }

        private readonly Dictionary<ulong, string> _roomPlayersNames = new Dictionary<ulong, string>();

        private string _pathToData;

        /// <summary>
        /// Returns name of user with given ID.
        /// This method should be called only from the host.
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public string GetNameByID(ulong userID)
        {
            return _roomPlayersNames[userID];
        }

        /// <summary>
        /// Add this user's ID and it's name to the dictionary.
        /// This method should be called only from the host.
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="username"></param>
        public void TryToSetName(ulong userID, string username)
        {
            _roomPlayersNames.Add(userID, username);
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            
            DontDestroyOnLoad(gameObject);
            
            Setup();
        }

        private void Setup()
        {
            _pathToData = Application.persistentDataPath + "/UserConfig.txt";
            
            UserName = GetUserData().UserName;
        }
        
        public void SaveUserData(string userName)
        {
            UserConfigData data = new UserConfigData(userName);
            string json = JsonUtility.ToJson(data);
            File.WriteAllText(_pathToData, json);
        }

        private UserConfigData GetUserData()
        {
            if (File.Exists(_pathToData))
            {
                string json = File.ReadAllText(_pathToData);
                UserConfigData data = JsonUtility.FromJson<UserConfigData>(json);
                return data;
            }
            else
            {
                print("Can not find file with user data!");
                return new UserConfigData("");
            }
        }
    }
}
