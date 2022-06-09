using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUI;

    public void Show()
    {
        pauseMenuUI.SetActive(true);
    }

    public void Hide()
    {
        pauseMenuUI.SetActive(false);
    }
}
