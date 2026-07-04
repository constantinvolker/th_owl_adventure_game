using UnityEngine;

/// Manages item combinations. Lives on PERSISTOBJECTS.
/// Register all valid combinations in the Inspector.
public class CombinationManager : MonoBehaviour
{
    public static CombinationManager Instance { get; private set; }

    [SerializeField] private ItemCombination[] combinations;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    /// Try to combine selectedItem with targetItem.
    /// Returns true if combination succeeded.
    public bool TryCombine(ItemData selectedItem, ItemData targetItem)
    {
        foreach (var combo in combinations)
        {
            if (combo == null) continue;

            bool match = (combo.itemA == selectedItem && combo.itemB == targetItem)
                      || (combo.itemA == targetItem  && combo.itemB == selectedItem);

            if (!match) continue;

            // Success
            if (combo.consumeBoth)
            {
                InventoryManager.Instance.RemoveItem(selectedItem);
                InventoryManager.Instance.RemoveItem(targetItem);
            }

            if (combo.result != null)
                InventoryManager.Instance.AddItem(combo.result);

            ItemSelectionState.Instance.Deselect();

            if (combo.successDialogue != null)
                DialogueManager.Instance.PlaySimpleDialogue(combo.successDialogue);

            return true;
        }

        // No combination found
        if (combinations.Length > 0)
        {
            // Show generic fail dialogue from first matching fail or global wrong item
            var global = GameManager.Instance.wrongItemDialogue;
            if (global != null)
                DialogueManager.Instance.PlaySimpleDialogue(global);
        }

        return false;
    }
}
