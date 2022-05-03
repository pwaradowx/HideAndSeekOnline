using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace Project.Game.Player
{
    public class PlayerEntity : NetworkBehaviour
    {
        [SerializeField] private Behaviour[] componentsToDisable;
        [SerializeField] private GameObject[] objectsToDisable;
        
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                _nameplate.text = value;
            }
        }
        private string _name;

        private TextMeshPro _nameplate;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            
            _nameplate = GetComponentInChildren<TextMeshPro>();

            if (!IsLocalPlayer)
            {
                foreach (var component in componentsToDisable)
                {
                    component.enabled = false;
                }

                foreach (var obj in objectsToDisable)
                {
                    obj.SetActive(false);
                }
            }
        }
    }
}
