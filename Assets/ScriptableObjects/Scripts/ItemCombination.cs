using UnityEngine;

/// Define a combination rule between two items.
/// Create via: Right-click -> Inventory -> Item Combination
[CreateAssetMenu(fileName = "Combo_New", menuName = "Inventory/Item Combination")]
public class ItemCombination : ScriptableObject
{
    [Tooltip("First item in combination (order doesn't matter).")]
    public ItemData itemA;

    [Tooltip("Second item in combination.")]
    public ItemData itemB;

    [Tooltip("Item produced by the combination.")]
    public ItemData result;

    [Tooltip("Consume both items on combination.")]
    public bool consumeBoth = true;

    [Tooltip("Dialogue played when combination succeeds.")]
    public DialogueData successDialogue;

    [Tooltip("Dialogue played when wrong item is used on this slot.")]
    public DialogueData failDialogue;
}
