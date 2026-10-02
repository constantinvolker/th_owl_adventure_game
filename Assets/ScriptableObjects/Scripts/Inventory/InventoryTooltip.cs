using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryTooltip : MonoBehaviour
{
    private TextMeshProUGUI tooltipText;
    private InventorySlot inventorySlot;
    private RectTransform slotRect;

    void Start()
    {
        inventorySlot = GetComponent<InventorySlot>();
        slotRect = GetComponent<RectTransform>();

        // Eigenes Tooltip-Text-Objekt für diesen Slot erstellen
        GameObject tooltipGO = new GameObject("TooltipText");
        tooltipGO.transform.SetParent(transform);
        tooltipGO.transform.localPosition = Vector3.zero;

        tooltipText = tooltipGO.AddComponent<TextMeshProUGUI>();
        tooltipText.text = "";
        tooltipText.raycastTarget = false;
        tooltipText.color = Color.white;

        // CanvasGroup für Sichtbarkeit
        CanvasGroup canvasGroup = tooltipGO.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    void Update()
    {
        // 1. Wenn der Slot leer ist: Tooltip sofort verstecken und abbrechen!
        if (inventorySlot == null || inventorySlot._item == null)
        {
            tooltipText.text = "";
            tooltipText.GetComponent<CanvasGroup>().alpha = 0f;
            return;
        }

        // 2. Prüfen, ob die Maus über dem Slot ist
        bool isMouseOver = RectTransformUtility.RectangleContainsScreenPoint(slotRect, Input.mousePosition);

        if (isMouseOver)
        {
            tooltipText.text = inventorySlot._item.itemName;
            tooltipText.GetComponent<CanvasGroup>().alpha = 1f;

            // Tooltip-Position an die Maus anheften    
            Vector2 mousePos = Input.mousePosition;
            Vector2 offset = new Vector2(20f, -30f);
            tooltipText.transform.position = mousePos + offset;
        }
        else
        {
            // 3. Maus ist nicht mehr auf dem Slot: Verstecken
            tooltipText.text = "";
            tooltipText.GetComponent<CanvasGroup>().alpha = 0f;
        }
    }
}