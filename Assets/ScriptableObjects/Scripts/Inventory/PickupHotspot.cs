using UnityEngine;

public class PickupHotspot : MonoBehaviour
{
    [SerializeField] private ItemData     item;

    [Header("Dialogues")]
    [SerializeField] private DialogueData pickupDialogue;
    [SerializeField] private DialogueData noBackpackDialogue;

    [SerializeField] private bool isEmbedded = false;

    public bool HasPickupDialogue => pickupDialogue != null;

    void Start()
    {
        if (isEmbedded) return;
        if (item == null) return;
        if (GameManager.Instance.IsItemCollected(item.itemID))
            gameObject.SetActive(false);
    }

    public void Pickup()
    {
        if (!enabled) return; // disabled when already looted
        if (item == null)
        {
            Debug.LogWarning($"[PickupHotspot] No ItemData assigned on '{gameObject.name}'.");
            return;
        }

        if (!isEmbedded && !item.isBackpack && !InventoryManager.Instance.BackpackUnlocked)
        {
            var dialogue = noBackpackDialogue ?? GameManager.Instance.noBackpackDialogue;
            if (dialogue != null)
                DialogueManager.Instance.PlaySimpleDialogue(dialogue);
            return;
        }

        InventoryManager.Instance.AddItem(item);

        if (pickupDialogue != null)
            DialogueManager.Instance.PlaySimpleDialogue(pickupDialogue);

        if (!isEmbedded)
            Destroy(gameObject);
    }
}