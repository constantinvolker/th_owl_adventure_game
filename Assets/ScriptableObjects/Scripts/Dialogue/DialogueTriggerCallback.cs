using UnityEngine;
using UnityEngine.Events;

/// Attach alongside DialogueTrigger.
/// Fires OnDialogueEnded when THIS trigger's dialogue finishes.
public class DialogueTriggerCallback : MonoBehaviour
{
    public UnityEvent OnDialogueEnded;

    private DialogueTrigger _trigger;
    private bool _waiting = false;

    void Awake()
    {
        _trigger = GetComponent<DialogueTrigger>();
    }

    void OnEnable()
    {
        DialogueManager.Instance.OnDialogueEnded += HandleDialogueEnded;
    }

    void OnDisable()
    {
        if (DialogueManager.Instance != null)
            DialogueManager.Instance.OnDialogueEnded -= HandleDialogueEnded;
    }

    public void StartListening()
    {
        _waiting = true;
    }

    private void HandleDialogueEnded()
    {
        if (!_waiting) return;
        _waiting = false;
        OnDialogueEnded?.Invoke();
    }
}
