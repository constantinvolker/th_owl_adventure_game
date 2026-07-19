using UnityEngine;

/// Attach to the NPC's collider object.
/// Forwards click events from ClickController to the DialogueTrigger.
[RequireComponent(typeof(Collider2D))]
public class NpcHotspot : MonoBehaviour
{
    // DialogueTrigger can live on the same GO or a parent
    private DialogueTrigger _trigger;

    void Awake()
    {
        _trigger = GetComponentInParent<DialogueTrigger>();

        if (_trigger == null)
            Debug.LogWarning($"[NpcHotspot] No DialogueTrigger found on '{gameObject.name}' or its parents.");
    }

    public void Interact()
    {
        _trigger?.StartDialogue();
    }
}
