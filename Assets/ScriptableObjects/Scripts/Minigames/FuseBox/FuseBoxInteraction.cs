using UnityEngine;

public class FuseBoxInteraction : MonoBehaviour
{
    [Header("Minigame")]
    public GameObject minigameUI;

    void Start()
    {
        minigameUI.SetActive(false);
    }

    void OnMouseDown()
    {
        OpenMinigame();
    }

    public void OpenMinigame()
    {
        minigameUI.SetActive(true);
    }

    public void CloseMinigame()
    {
        minigameUI.SetActive(false);
    }
}