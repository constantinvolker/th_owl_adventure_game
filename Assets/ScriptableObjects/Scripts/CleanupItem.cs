using System;
using UnityEngine;
using UnityEngine.SceneManagement;

/// Attach to a mess item that can be clicked to clean up.
/// When all CleanupItems in the group are done, fires OnAllCleaned.
public class CleanupItem : MonoBehaviour
{
    [Tooltip("Optional dialogue when this item is cleaned up.")]
    [SerializeField] private DialogueData cleanupDialogue;

    [Tooltip("The CleanupGroup this item belongs to.")]
    [SerializeField] private CleanupGroup group;

    [Tooltip("Eindeutige ID für dieses Item. Wird standardmäßig aus SceneName/ObjectName gebildet, falls leer.")]
    [SerializeField] private string itemID;

    void Reset()
    {
        // Beim Hinzufügen im Editor Standard leer lassen oder automatisch setzen — hier keine GUID mehr,
        // damit Namen wie 'banana_1' nutzbar bleiben.
    }

    void Awake()
    {
        // Wenn keine ID gesetzt ist, verwende SceneName/ObjectName, z. B. "RoomA/banana_1".
        if (string.IsNullOrEmpty(itemID))
            itemID = $"{SceneManager.GetActiveScene().name}/{gameObject.name}";

        // Wenn bereits gesammelt, sofort deaktivieren.
        if (GameManager.Instance != null && GameManager.Instance.IsItemCollected(itemID))
        {
            gameObject.SetActive(false);
        }
    }

    public string GetItemID() => itemID;

    public void Cleanup()
    {
        if (cleanupDialogue != null)
            DialogueManager.Instance.PlaySimpleDialogue(cleanupDialogue);

        // Markiere als gesammelt
        if (!string.IsNullOrEmpty(itemID))
            GameManager.Instance?.MarkItemCollected(itemID);

        gameObject.SetActive(false);

        if (group != null)
            group.ReportCleaned(this);
    }
}
