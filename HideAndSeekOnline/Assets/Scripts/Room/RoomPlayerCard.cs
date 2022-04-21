using TMPro;
using UnityEngine;

namespace Project.Room
{
    public class RoomPlayerCard : MonoBehaviour
    {
        [SerializeField] private GameObject waitingPanel;
        [SerializeField] private GameObject playerDataPanel;

        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI readyText;

        public void UpdateDisplay(RoomPlayerState playerState)
        {
            waitingPanel.SetActive(false);
            playerDataPanel.SetActive(true);

            nameText.text = playerState.UserName.ToString();
            readyText.text = playerState.IsReady ? "Ready" : "Not Ready";
        }
    }
}
