using UnityEngine;
using TMPro;
using UnityEngine.EventSystems; // ZWINGEND ERFORDERLICH für UI-Events!

public class InventoryTooltip : MonoBehaviour
{
    [Header("UI Komponenten (Welt-Text)")]
    public TextMeshProUGUI uiTextDisplay;     // Der TextContainer (Welt-TextMeshPro)
    private InventorySlot inventorySlot;
    private Coroutine hideCoroutine;

    void Start()
    {
        // Sicherstellen, dass der Text am Anfang unsichtbar ist
        inventorySlot = GetComponent<InventorySlot>();

        // 2. AUTOMATISCHE SUCHE im PERSISTOBJECTS-Klon:
        // Wir suchen in der gesamten Spielwelt nach dem Text-Objekt.
        // Ersetze "DeinNeuesTextObjektName" durch den exakten Namen des Objekts im Prefab!
        if (uiTextDisplay == null)
        {
            GameObject gefundenesTextObjekt = GameObject.Find("ItemNameText");

            if (gefundenesTextObjekt != null)
            {
                uiTextDisplay = gefundenesTextObjekt.GetComponent<TextMeshProUGUI>();
            }
        }

        uiTextDisplay.text = "";
    }

    public void OnPointerEnter()
    {
        if (hideCoroutine != null) StopCoroutine(hideCoroutine);

        uiTextDisplay.text = inventorySlot._item.itemName; 
        uiTextDisplay.color = new Color(uiTextDisplay.color.r, uiTextDisplay.color.g, uiTextDisplay.color.b, 1f);

        uiTextDisplay.ForceMeshUpdate();

        hideCoroutine = StartCoroutine(HideTextAfterDelay(3f));
    }

    void Update()
    {
        // Nur bewegen, wenn das Textfeld existiert und gerade Text anzeigt
        if (uiTextDisplay != null && !string.IsNullOrEmpty(uiTextDisplay.text))
        {
            // Holt die aktuelle Mausposition im Bildschirm-Raum (da es ein Canvas-UI-Text ist)
            Vector2 mousePos = Input.mousePosition;

            // Leicht nach rechts oben versetzen (z.B. 20 Pixel X, 20 Pixel Y), 
            // damit der Text nicht direkt unter dem Mauszeiger klebt und flackert!
            Vector2 offset = new Vector2(20f, -30f);

            uiTextDisplay.transform.position = mousePos + offset;
        }
    }

    // Der Timer, der nach X Sekunden den Text löscht
    private System.Collections.IEnumerator HideTextAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (uiTextDisplay != null)
        {
            uiTextDisplay.text = "";
        }
    }
}
