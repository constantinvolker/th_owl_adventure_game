using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

/// Like UseHotspot but accepts multiple different items.
/// Each item can trigger a different event.
/// Use for NPCs that need several items delivered.
public class MultiUseHotspot : MonoBehaviour
{
    [System.Serializable]
    public class ItemUseEntry
    {
        public ItemData      requiredItem;
        public bool          consumeItem   = true;
        public DialogueData  successDialogue;
        public DialogueData  alreadyGivenDialogue;
        public UnityEvent    OnUsed;
        [HideInInspector]
        public bool          alreadyGiven  = false;
    }

    [SerializeField] private List<ItemUseEntry> entries;
    [SerializeField] private DialogueData       wrongItemDialogue;

    // Fires when ALL items have been given
    public UnityEvent OnAllItemsGiven;

    public void TryUse(ItemData usedItem)
    {
        if (usedItem == null) return;

        foreach (var entry in entries)
        {
            if (entry.requiredItem != usedItem) continue;

            if (entry.alreadyGiven)
            {
                if (entry.alreadyGivenDialogue != null)
                    DialogueManager.Instance.PlaySimpleDialogue(entry.alreadyGivenDialogue);
                ItemSelectionState.Instance.Deselect();
                return;
            }

            // Correct item
            if (entry.consumeItem)
                InventoryManager.Instance.RemoveItem(usedItem);

            entry.alreadyGiven = true;
            ItemSelectionState.Instance.Deselect();

            if (entry.successDialogue != null)
                DialogueManager.Instance.PlaySimpleDialogue(entry.successDialogue);

            entry.OnUsed?.Invoke();

            // Check if all items delivered
            if (AllItemsGiven())
                OnAllItemsGiven?.Invoke();

            return;
        }

        // Wrong item
        var dialogue = wrongItemDialogue ?? GameManager.Instance.wrongItemDialogue;
        if (dialogue != null)
            DialogueManager.Instance.PlaySimpleDialogue(dialogue);
    }

    private bool AllItemsGiven()
    {
        foreach (var entry in entries)
            if (!entry.alreadyGiven) return false;
        return true;
    }
}
