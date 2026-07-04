using System;
using UnityEngine;

/// Tracks which item is currently "held" by the cursor.
/// Lives on PERSISTOBJECTS. ClickController and InventorySlot talk through here.
public class ItemSelectionState : MonoBehaviour
{
    public static ItemSelectionState Instance { get; private set; }

    public event Action<ItemData> OnItemSelected;    // item was picked up onto cursor
    public event Action           OnItemDeselected;  // cursor returned to normal

    public ItemData SelectedItem { get; private set; }
    public bool     HasSelection => SelectedItem != null;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    /// Called by InventorySlot when the player clicks a slot.
    public void SelectItem(ItemData item)
    {
        if (item == null) return;

        // Clicking the already-selected item deselects it
        if (SelectedItem == item)
        {
            Deselect();
            return;
        }

        SelectedItem = item;
        OnItemSelected?.Invoke(item);
    }

    /// Called by ClickController after a successful use, or on right-click / escape.
    public void Deselect()
    {
        SelectedItem = null;
        OnItemDeselected?.Invoke();
    }
}