using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject settingsPanel;

    [Header("Buttons")]
    [SerializeField] private Button continueButton;

    void Start()
    {
        if (continueButton != null)
            continueButton.gameObject.SetActive(GameManager.Instance.HasSaveData());
    }

    public void StartNewGame()
    {
        GameManager.Instance.StartNewGame();
    }

    public void ContinueGame()
    {
        if (!GameManager.Instance.HasSaveData()) return;

        // Load the last room the player was in
        SceneLoader.Instance.LoadRoom(
            GameManager.Instance.GetLastScene(),
            GameManager.Instance.GetLastSpawn()
        );
    }

    public void OpenSettings()
    {
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}