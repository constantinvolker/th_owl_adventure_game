using UnityEngine;
using UnityEngine.UI;

public class KioskHideButton : MonoBehaviour
{
    [SerializeField] private Button button;

    private void Start()
    {
#if KIOSK_BUILD
        button.gameObject.SetActive(false);
#endif
    }
}

// Primär für die Arcade. Wenn wir beim bauen den Flag setzen sind der Knopf Exit weg, damit man das spiel nicht beenden kann.
// Flag kann unter Edit > Project Settings > Player > Other Settings > Scripting Define Symbols gesetzt werden