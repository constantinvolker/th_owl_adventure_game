using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button exitButton;
    [SerializeField] private Button feedbackButton;

    private void Start()
    {
#if KIOSK_BUILD
        exitButton.gameObject.SetActive(false);
        feedbackButton.gameObject.SetActive(false);
#endif
    }
}

// Primär für die Arcade. Wenn wir beim bauen den Flag setzen sind die Knöpfe Feedback und Exit weg, damit man das spiel nicht beenden kann.
// Flag kann unter Edit > Project Settings > Player > Other Settings > Scripting Define Symbols gesetzt werden