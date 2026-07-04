using UnityEngine;
using UnityEngine.Events;

/// Attach to the storage box/crate.
/// Player picks up wood pieces and uses them on this box.
/// When all pieces are deposited, fires OnAllDeposited.
public class WorkbenchCleanup : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("How many items need to be deposited.")]
    [SerializeField] private int totalItems = 3;

    [Tooltip("Item that counts as a valid deposit.")]
    [SerializeField] private ItemData woodpieceItem;

    [Header("Dialogue")]
    [SerializeField] private DialogueData depositDialogue;
    [SerializeField] private DialogueData wrongItemDialogue;
    [SerializeField] private DialogueData allDoneDialogue;

    public UnityEvent OnAllDeposited;

    private int _depositedCount = 0;

    /// Called by ClickController when item is used on this hotspot.
    public void TryDeposit(ItemData usedItem)
    {
        if (usedItem != woodpieceItem)
        {
            var dialogue = wrongItemDialogue ?? GameManager.Instance.wrongItemDialogue;
            if (dialogue != null)
                DialogueManager.Instance.PlaySimpleDialogue(dialogue);
            return;
        }

        InventoryManager.Instance.RemoveItem(usedItem);
        ItemSelectionState.Instance.Deselect();
        _depositedCount++;

        if (depositDialogue != null)
            DialogueManager.Instance.PlaySimpleDialogue(depositDialogue);

        if (_depositedCount >= totalItems)
        {
            if (allDoneDialogue != null)
                DialogueManager.Instance.QueueDialogue(allDoneDialogue);
            OnAllDeposited?.Invoke();
        }
    }
}
