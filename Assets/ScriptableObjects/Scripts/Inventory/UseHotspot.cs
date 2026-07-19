using UnityEngine;
using UnityEngine.Events;

/// Attach to any world object that reacts to a specific item being used on it.
public class UseHotspot : MonoBehaviour
{
    [SerializeField] private ItemData requiredItem;
    [SerializeField] private bool     consumeItem = true;

    [Tooltip("Optional: overrides global wrong item dialogue for this specific object.")]
    [SerializeField] private DialogueData wrongItemDialogue;

    [Space]
    public UnityEvent OnUsed;
    public UnityEvent OnWrongItem;

    public void TryUse(ItemData usedItem)
    {
        if (usedItem == null) return;

        if (usedItem == requiredItem)
        {
            if (consumeItem)
                InventoryManager.Instance.RemoveItem(usedItem);

            ItemSelectionState.Instance.Deselect();
            OnUsed?.Invoke();
        }
        else
        {
            // Local dialogue overrides global fallback
            var dialogue = wrongItemDialogue ?? GameManager.Instance.wrongItemDialogue;
            if (dialogue != null)
                DialogueManager.Instance.PlaySimpleDialogue(dialogue);

            OnWrongItem?.Invoke();
        }
    }
}