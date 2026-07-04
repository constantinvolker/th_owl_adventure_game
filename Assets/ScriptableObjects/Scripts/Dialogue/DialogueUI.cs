using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Zeigt Dialog-Texte und -Optionen an.
///
/// Unterstützt ZWEI Modi (umschaltbar zur Laufzeit oder im Inspector):
///
///   1) AboveSpeaker (alt)
///        Sprechblase folgt dem Speaker im Welt-Raum. Klein, dezent.
///
///   2) TopBox (neu)
///        Feste, deutlich sichtbare Dialog-Box im oberen Drittel des
///        Bildschirms (unterhalb der Inventar-Leiste). Heller Rahmen
///        zur klaren Abgrenzung. Empfohlen für Story-/NPC-Dialoge.
///
/// Beide Modi nutzen denselben Code-Pfad — es wird nur unterschiedlich
/// gerendert. So musst du dich nicht zwischen "alt" und "neu" entscheiden;
/// du kannst über das Feld <see cref="dialogueMode"/> jederzeit wechseln.
/// </summary>
public class DialogueUI : MonoBehaviour
{
    public enum DialogueMode
    {
        AboveSpeaker, // alte Sprechblase über dem Charakter
        TopBox        // neue große Box oben
    }

    public static DialogueUI Instance { get; private set; }

    // ── Modus ────────────────────────────────────────────────────────────────

    [Header("Dialogue Mode")]
    [Tooltip("Welcher Dialog-Stil soll verwendet werden?")]
    [SerializeField] private DialogueMode dialogueMode = DialogueMode.TopBox;

    // ── Speech Panel (alter Stil — folgt NPC) ────────────────────────────────

    [Header("Above-Speaker Panel (Mode = AboveSpeaker)")]
    [SerializeField] private RectTransform   canvasRect;
    [SerializeField] private RectTransform   speechPanelRect;
    [SerializeField] private TextMeshProUGUI speakerNameText;
    [SerializeField] private TextMeshProUGUI dialogueText;

    [Header("Above-Speaker Follow Settings")]
    [SerializeField] private Vector2 screenOffset = new Vector2(0f, 40f);
    [SerializeField] private Camera  worldCamera;

    // ── Top Box (neuer Stil — feste Box oben) ────────────────────────────────

    [Header("Top Box Panel (Mode = TopBox)")]
    [Tooltip("Container der Top-Box. Unterhalb der Inventar-Bar, im oberen Drittel.")]
    [SerializeField] private RectTransform   topBoxPanel;
    [SerializeField] private TextMeshProUGUI topBoxSpeakerNameText;
    [SerializeField] private TextMeshProUGUI topBoxDialogueText;

    // ── Options Panel (gemeinsam für beide Modi) ─────────────────────────────

    [Header("Options Panel (shared)")]
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private GameObject optionButtonPrefab;
    [SerializeField] private Transform  optionsContainer;

    private Transform _currentSpeaker;
    private bool      _showingOptions;
    private bool      _inputBlocked;

    // ── Lifecycle ────────────────────────────────────────────────────────────

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        if (!worldCamera) worldCamera = Camera.main;
        if (canvasRect == null && transform.parent != null)
        {
            var canvas = GetComponentInParent<Canvas>();
            if (canvas != null) canvasRect = canvas.GetComponent<RectTransform>();
        }
        Hide();
    }

    void Update()
    {
        // AboveSpeaker-Modus: Sprechblase nachführen
        if (dialogueMode == DialogueMode.AboveSpeaker
            && speechPanelRect != null
            && speechPanelRect.gameObject.activeSelf
            && _currentSpeaker != null)
        {
            PositionAboveSpeaker(_currentSpeaker);
        }

        if (_inputBlocked)
        {
            _inputBlocked = false;
            return;
        }

        if (!_showingOptions && DialogueManager.Instance.IsPlaying)
        {
            if (Input.GetMouseButtonDown(0) &&
                !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            {
                DialogueManager.Instance.Advance();
            }
        }
    }

    // ── Public API ───────────────────────────────────────────────────────────

    public void ShowLine(DialogueLine line, Transform speaker)
    {
        StopAllCoroutines();
        StartCoroutine(ShowLineRoutine(line, speaker));
    }

    public void ShowOptions(DialogueOption[] options, Transform speaker)
    {
        StopAllCoroutines();
        StartCoroutine(ShowOptionsRoutine(options, speaker));
    }

    public void Hide()
    {
        StopAllCoroutines();
        SetSpeechPanelActive(false);
        SetTopBoxActive(false);
        if (optionsPanel) optionsPanel.SetActive(false);
        if (speakerNameText)       speakerNameText.text       = "";
        if (dialogueText)          dialogueText.text          = "";
        if (topBoxSpeakerNameText) topBoxSpeakerNameText.text = "";
        if (topBoxDialogueText)    topBoxDialogueText.text    = "";
        ClearOptions();
        _currentSpeaker = null;
        _showingOptions = false;
        _inputBlocked   = false;
    }

    /// <summary>Modus zur Laufzeit wechseln (z.B. via Settings-Toggle).</summary>
    public void SetMode(DialogueMode mode)
    {
        dialogueMode = mode;
        // Aktiven Dialog ggf. neu darstellen, wenn gerade einer läuft
        SetSpeechPanelActive(false);
        SetTopBoxActive(false);
    }

    public DialogueMode GetMode() => dialogueMode;

    // ── Internal: Dialogue-Anzeige ───────────────────────────────────────────

    private IEnumerator ShowLineRoutine(DialogueLine line, Transform speaker)
    {
        _currentSpeaker = speaker;
        _showingOptions = false;
        _inputBlocked   = true;

        if (optionsPanel) optionsPanel.SetActive(false);
        ClearOptions();

        // Beide Panels erst ausblenden — danach das passende einblenden.
        SetSpeechPanelActive(false);
        SetTopBoxActive(false);

        yield return null;

        if (dialogueMode == DialogueMode.AboveSpeaker)
        {
            if (speakerNameText) { speakerNameText.text  = line.speakerName;  speakerNameText.color = line.speakerColor; }
            if (dialogueText)    { dialogueText.text     = line.text;         dialogueText.color    = line.speakerColor; }
            SetSpeechPanelActive(true);
            yield return null;
            PositionAboveSpeaker(speaker);
        }
        else // TopBox
        {
            if (topBoxSpeakerNameText) { topBoxSpeakerNameText.text  = line.speakerName; topBoxSpeakerNameText.color = line.speakerColor; }
            if (topBoxDialogueText)    { topBoxDialogueText.text     = line.text; }
            SetTopBoxActive(true);
        }
    }

    private IEnumerator ShowOptionsRoutine(DialogueOption[] options, Transform speaker)
    {
        _currentSpeaker = speaker;
        _showingOptions = true;
        _inputBlocked   = true;

        ClearOptions();
        if (optionsPanel) optionsPanel.SetActive(false);

        yield return null;

        if (optionsPanel) optionsPanel.SetActive(true);

        foreach (var option in options)
        {
            var btnGO  = Instantiate(optionButtonPrefab, optionsContainer);
            var label  = btnGO.GetComponentInChildren<TextMeshProUGUI>();
            var button = btnGO.GetComponent<Button>();
            if (label) label.text = option.optionText;
            var captured = option;
            button.onClick.AddListener(() =>
            {
                _inputBlocked = true;
                DialogueManager.Instance.ChooseOption(captured);
            });
        }
    }

    // ── Internal: Helpers ────────────────────────────────────────────────────

    private void SetSpeechPanelActive(bool active)
    {
        if (speechPanelRect != null)
            speechPanelRect.gameObject.SetActive(active);
    }

    private void SetTopBoxActive(bool active)
    {
        if (topBoxPanel != null)
            topBoxPanel.gameObject.SetActive(active);
    }

    private void PositionAboveSpeaker(Transform speaker)
    {
        if (worldCamera == null || speaker == null || canvasRect == null || speechPanelRect == null) return;

        Vector3 worldTop = GetSpeakerTopWorld(speaker);
        Vector3 viewport = worldCamera.WorldToViewportPoint(worldTop);

        Vector2 canvasSize = canvasRect.sizeDelta;
        Vector2 localPoint = new Vector2(
            (viewport.x - 0.5f) * canvasSize.x,
            (viewport.y - 0.5f) * canvasSize.y
        );

        speechPanelRect.pivot            = new Vector2(0.5f, 0f);
        speechPanelRect.anchoredPosition = localPoint + screenOffset;
    }

    private Vector3 GetSpeakerTopWorld(Transform speaker)
    {
        var sr = speaker.GetComponentInChildren<SpriteRenderer>();
        if (sr == null && speaker.parent != null)
            sr = speaker.parent.GetComponentInChildren<SpriteRenderer>();

        if (sr != null)
            return new Vector3(sr.bounds.center.x, sr.bounds.max.y + 0.1f, speaker.position.z);

        return speaker.position + Vector3.up * 0.6f;
    }

    private void ClearOptions()
    {
        if (optionsContainer == null) return;
        foreach (Transform child in optionsContainer)
            Destroy(child.gameObject);
    }
}
