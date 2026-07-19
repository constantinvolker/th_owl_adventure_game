using System;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    public event Action OnDialogueStarted;
    public event Action OnDialogueEnded;

    public bool IsPlaying { get; private set; }

    [Header("End Option")]
    [SerializeField] private string[] endOptionVariants = new string[]
    {
        "Tschüss.",
        "Bis später.",
        "Ich muss weiter.",
        "Das war's erstmal.",
        "Wir reden später."
    };

    private DialogueData    _currentData;
    private int             _lineIndex;
    private Transform       _speakerTransform;
    private DialogueTrigger _currentTrigger;
    private string          _currentEndLabel;

    // Queue for sequential dialogues (e.g. open box → pickup item)
    private readonly Queue<(DialogueData data, Transform speaker)> _queue = new();

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    // ── Public API ────────────────────────────────────────────────────────────

    public void PlayDialogue(DialogueData data, Transform speaker, DialogueTrigger trigger = null)
    {
        if (data == null) return;

        _currentData      = data;
        _lineIndex        = 0;
        _speakerTransform = speaker;
        _currentTrigger   = trigger;
        IsPlaying         = true;

        if (data == trigger?.MainOptionsDialogue || _currentEndLabel == null)
            _currentEndLabel = endOptionVariants[UnityEngine.Random.Range(0, endOptionVariants.Length)];

        PlayerMovement.Instance.canMove = false;
        OnDialogueStarted?.Invoke();

        if (data.lines == null || data.lines.Length == 0)
        {
            ShowOptionsOrEnd();
            return;
        }

        DialogueUI.Instance.ShowLine(data.lines[0], _speakerTransform);
    }

    /// Plays a simple dialogue anchored to the player.
    /// If a dialogue is already playing, queues it to play after.
    public void PlaySimpleDialogue(DialogueData data)
    {
        if (data == null) return;

        Transform playerTransform = PlayerMovement.Instance?.transform;
        if (playerTransform == null) return;

        if (IsPlaying)
        {
            _queue.Enqueue((data, playerTransform));
            return;
        }

        PlayDialogue(data, playerTransform, null);
    }

    /// Queues a dialogue to play after the current one finishes.
    public void QueueDialogue(DialogueData data, Transform speaker = null)
    {
        if (data == null) return;
        var t = speaker ?? PlayerMovement.Instance?.transform;
        if (t == null) return;
        _queue.Enqueue((data, t));
    }

    public void Advance()
    {
        if (!IsPlaying) return;

        _lineIndex++;

        if (_lineIndex < _currentData.lines.Length)
        {
            DialogueUI.Instance.ShowLine(_currentData.lines[_lineIndex], _speakerTransform);
            return;
        }

        ShowOptionsOrEnd();
    }

    public void ChooseOption(DialogueOption option)
    {
        if (option.isEndOption) { EndDialogue(); return; }

        if (option.nextDialogue != null)
            PlayDialogue(option.nextDialogue, _speakerTransform, _currentTrigger);
        else
            ReturnToMainOptions();
    }

    public void EndDialogue()
    {
        IsPlaying        = false;
        _currentEndLabel = null;
        _currentTrigger  = null;
        PlayerMovement.Instance.canMove = true;
        DialogueUI.Instance.Hide();
        OnDialogueEnded?.Invoke();

        // Play next queued dialogue if any
        if (_queue.Count > 0)
        {
            var (data, speaker) = _queue.Dequeue();
            PlayDialogue(data, speaker, null);
        }
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private void ShowOptionsOrEnd()
    {
        bool isMainOptions = _currentTrigger != null
            && _currentData == _currentTrigger.MainOptionsDialogue;

        if (_currentData.options != null && _currentData.options.Length > 0)
        {
            var opts = isMainOptions
                ? BuildOptionsWithEnd(_currentData.options)
                : _currentData.options;
            DialogueUI.Instance.ShowOptions(opts, _speakerTransform);
            return;
        }

        if (_currentTrigger != null
            && _currentTrigger.MainOptionsDialogue != null
            && !isMainOptions)
        {
            ReturnToMainOptions();
            return;
        }

        EndDialogue();
    }

    private void ReturnToMainOptions()
    {
        if (_currentTrigger != null && _currentTrigger.MainOptionsDialogue != null)
            PlayDialogue(_currentTrigger.MainOptionsDialogue, _speakerTransform, _currentTrigger);
        else
            EndDialogue();
    }

    private DialogueOption[] BuildOptionsWithEnd(DialogueOption[] original)
    {
        foreach (var o in original)
            if (o.isEndOption) return original;

        var result = new DialogueOption[original.Length + 1];
        original.CopyTo(result, 0);
        result[result.Length - 1] = new DialogueOption
        {
            optionText  = _currentEndLabel,
            isEndOption = true
        };
        return result;
    }
}