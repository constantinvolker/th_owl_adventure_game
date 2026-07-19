using UnityEngine;

[CreateAssetMenu(fileName = "Item_New", menuName = "Inventory/Item Data")]
public class ItemData : ScriptableObject
{
    [Header("Identity")]
    [Tooltip("Unique ID used for saving. Auto-filled from asset name if left empty.")]
    public string itemID;

    public string itemName;
    [TextArea] public string description;

    [Header("Visuals")]
    public Sprite icon;

    [Header("Settings")]
    public bool isBackpack;

    // Called by Unity when the asset is created or reset in the Editor
    void OnValidate()
    {
        if (string.IsNullOrEmpty(itemID))
            itemID = name; // e.g. "Item_Backpack"
    }
}