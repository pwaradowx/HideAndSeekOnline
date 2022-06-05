using UnityEngine;

public class MatchMenu : MonoBehaviour
{
    [SerializeField] private GameObject matchMenuUI;

    public void Show()
    {
        matchMenuUI.SetActive(true);
    }

    public void Hide()
    {
        matchMenuUI.SetActive(false);
    }
}
