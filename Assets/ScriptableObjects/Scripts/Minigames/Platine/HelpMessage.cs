using UnityEngine;
using TMPro;
using System.Collections;

public class HelpMessage : MonoBehaviour
{
    [Header("UI Komponenten")]
    public GameObject textPanel;         // Das übergeordnete UI-Objekt/Hintergrund (zum Ein/Ausschalten)
    public TextMeshPro uiTextDisplay; // Das eigentliche Textfeld für den Inhalt

    private Coroutine hideTimerCoroutine;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    void OnMouseDown()
    {
        if (hideTimerCoroutine != null)
        {
            StopCoroutine(hideTimerCoroutine);
        }

        // Wenn das Textfeld bereits offen ist, schließe es einfach beim nächsten Klick
        if (textPanel.activeSelf)
        {
            textPanel.SetActive(false);
            return;
        }

        // Prüfen, ob der PuzzleManager existiert
        if (PuzzleManager.Instance != null)
        {
            // Erst prüfen, ob das Rätsel generell schon gelöst ist (deine alte Variable)
            if (PuzzleManager.Instance.isPuzzleSolved)
            {
                uiTextDisplay.text = "Das passt alles!";
            }
            else
            {
                Debug.Log(PuzzleManager.Instance.errorSlot);
                switch (PuzzleManager.Instance.errorSlot)
                {
                    case "TargetResistor":
                        uiTextDisplay.text = "Der Widerstand ist noch nicht an der richtigen Stelle. Was war nochmal das Formelzeichen für den Widerstand?";
                        break;
                    case "TargetCoil":
                        uiTextDisplay.text = "Die Spule ist noch nicht an der richtigen Stelle. Was war nochmal das Formelzeichen für die Induktivität?";
                        break;
                    case "TargetSwitch":
                        uiTextDisplay.text = "Da fehlt noch der Schalter... Mal sehen wie viele Anschlüsse da gebraucht werden.";
                        break;
                    case "TargetDiode":
                        uiTextDisplay.text = "Die Diode ist noch nicht an der richtigen Stelle? Was war nochmal die Haupteigenschaft einer Diode?";
                        break;
                    case "TargetCapacitor":
                        uiTextDisplay.text = "Der Kondensator ist noch nicht an der richtigen Stelle. Was war nochmal das Formelzeichen für die Kapazität?";
                        break;
                    case "TargetLamp":
                        uiTextDisplay.text = "Die Lampe ist noch nicht an der richtigen Stelle!";
                        break;
                }
            }

            // Textfenster sichtbar machen
            textPanel.SetActive(true);

            hideTimerCoroutine = StartCoroutine(HideTextAfterDelay(5f));
        }
    }

    private IEnumerator HideTextAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // Warte exakt X Sekunden
        textPanel.SetActive(false);             // Fenster ausblenden
    }
}
