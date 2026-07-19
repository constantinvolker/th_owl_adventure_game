using UnityEngine;
using UnityEngine.Events;

/// Attach to a mess item that can be clicked to clean up.
/// When all CleanupItems in the group are done, fires OnAllCleaned.
public class CleanupItem : MonoBehaviour
{
    [Tooltip("Optional dialogue when this item is cleaned up.")]
    [SerializeField] private DialogueData cleanupDialogue;

    [Tooltip("The CleanupGroup this item belongs to.")]
    [SerializeField] private CleanupGroup group;

    public void Cleanup()
    {
        if (cleanupDialogue != null)
            DialogueManager.Instance.PlaySimpleDialogue(cleanupDialogue);

        gameObject.SetActive(false);

        if (group != null)
            group.ReportCleaned(this);
    }
}
