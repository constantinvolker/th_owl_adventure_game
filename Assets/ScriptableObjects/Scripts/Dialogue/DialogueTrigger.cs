using UnityEngine;

/// Attach to any NPC.
/// npcID must be unique across all scenes, e.g. "Randy_Bedroom".
public class DialogueTrigger : MonoBehaviour
{
    [Header("Identity")]
    [Tooltip("Unique ID for this NPC instance. Used for save state. E.g. 'Randy_Bedroom'.")]
    [SerializeField] private string npcID;

    [Header("Dialogues")]
    [SerializeField] private DialogueData greetingDialogue;
    [SerializeField] private DialogueData mainOptionsDialogue;
    [SerializeField] private DialogueData repeatingDialogue;

    [Header("Anchor")]
    [SerializeField] private Transform speakerAnchor;

    public DialogueData MainOptionsDialogue => mainOptionsDialogue;

    /// Play a specific dialogue anchored to this NPC — ignores internal state.
    /// Queues if a dialogue is already playing.
    public void PlaySpecificDialogue(DialogueData data)
    {
        if (data == null) return;
        Transform anchor = speakerAnchor != null ? speakerAnchor : transform;

        if (DialogueManager.Instance.IsPlaying)
            DialogueManager.Instance.QueueDialogue(data, anchor);
        else
            DialogueManager.Instance.PlayDialogue(data, anchor, null);
    }

    private bool _hasSpoken;

    void Start()
    {
        // Restore spoken state from save
        if (!string.IsNullOrEmpty(npcID))
            _hasSpoken = GameManager.Instance.HasSpokenTo(npcID);
    }

    public void StartDialogue()
    {
        if (DialogueManager.Instance.IsPlaying) return;

        Transform anchor = speakerAnchor != null ? speakerAnchor : transform;

        if (!_hasSpoken)
        {
            _hasSpoken = true;
            if (!string.IsNullOrEmpty(npcID))
                GameManager.Instance.MarkSpokenTo(npcID);

            if (greetingDialogue != null)
            {
                DialogueManager.Instance.PlayDialogue(greetingDialogue, anchor, this);
                return;
            }
        }
        else if (repeatingDialogue != null)
        {
            DialogueManager.Instance.PlayDialogue(repeatingDialogue, anchor, this);
            return;
        }

        if (mainOptionsDialogue != null)
            DialogueManager.Instance.PlayDialogue(mainOptionsDialogue, anchor, this);
    }
}