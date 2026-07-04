using UnityEngine;

/// Attach alongside UseHotspot.
/// When triggered, adds a "result" item to inventory.
/// Use case: empty bottle + tap = full bottle.
/// Wire UseHotspot.OnUsed -> ItemTransformer.Transform()
public class ItemTransformer : MonoBehaviour
{
    [Tooltip("Item added to inventory when transformation occurs.")]
    [SerializeField] private ItemData resultItem;

    [Tooltip("Optional dialogue played after transformation.")]
    [SerializeField] private DialogueData transformDialogue;

    public void Transform()
    {
        if (resultItem == null) return;

        InventoryManager.Instance.AddItem(resultItem);

        if (transformDialogue != null)
            DialogueManager.Instance.PlaySimpleDialogue(transformDialogue);
    }
}