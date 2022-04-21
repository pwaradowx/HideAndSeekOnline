using System;
using System.IO;
using Project.Global;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Project.RoomConnection
{
    [RequireComponent(typeof(Button))]
    public class ResetNameButton : MonoBehaviour, IPointerClickHandler
    {
        public delegate void ResetUserName();
        public event ResetUserName OnUserNameResetEvent;
        
        private bool _isNameExist;
        private string _pathToData;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!_isNameExist) return;

            try
            {
                File.Delete(_pathToData);
                UserConfig.Instance.UserName = "";
                gameObject.SetActive(false);
                
                OnUserNameResetEvent?.Invoke();
            }
            catch (Exception e)
            {
                print("Can't find user config file.");
                Console.WriteLine(e);
                throw;
            }
        }

        private void Start()
        {
            _pathToData = Application.persistentDataPath + "/UserConfig.txt";

            _isNameExist = UserConfig.Instance.UserName != "";
            if (!_isNameExist) gameObject.SetActive(false);
        }
    }
}
