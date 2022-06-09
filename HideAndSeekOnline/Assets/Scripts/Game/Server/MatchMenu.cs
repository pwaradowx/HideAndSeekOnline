using UnityEngine;
using TMPro;
using Unity.Netcode;

namespace Project.Game.Server
{
    public class MatchMenu : NetworkBehaviour
    {
        [SerializeField] private TextMeshProUGUI timer;
        [SerializeField] private TextMeshProUGUI winDeclaration;
        [SerializeField] private GameObject gameEndMenu;

        [ClientRpc]
        public void UpdateTimerClientRpc(float value)
        {
            timer.text = $"{value}";
        }

        [ClientRpc]
        public void DisableTimerClientRpc()
        {
            timer.gameObject.SetActive(false);
        }

        [ClientRpc]
        public void ShowWinDeclarationClientRpc(string value)
        {
            winDeclaration.gameObject.SetActive(true);
            winDeclaration.text = value;
        }

        [ClientRpc]
        public void ShowGameEndMenuClientRpc()
        {
            gameEndMenu.SetActive(true);
        }
    }
}
