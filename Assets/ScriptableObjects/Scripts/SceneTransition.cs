using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// Handles fade-to-black transitions between scenes.
/// Lives on PERSISTOBJECTS under a dedicated FadeCanvas.
public class SceneTransition : MonoBehaviour
{
    public static SceneTransition Instance { get; private set; }

    [Header("Fade")]
    [SerializeField] private CanvasGroup fadeCanvasGroup;
    [SerializeField] private float defaultFadeDuration = 0.4f;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        // Start fully transparent
        if (fadeCanvasGroup != null)
            fadeCanvasGroup.alpha = 0f;
    }

    // ── Public API ────────────────────────────────────────────────────────────

    /// Fade out (to black), run action, then fade back in.
    public IEnumerator FadeTransition(System.Action midAction, float duration = -1f)
    {
        float d = duration > 0 ? duration : defaultFadeDuration;

        yield return StartCoroutine(Fade(0f, 1f, d));
        midAction?.Invoke();
        yield return StartCoroutine(Fade(1f, 0f, d));
    }

    /// Just fade to black (no fade-in). Use before scene load.
    public IEnumerator FadeOut(float duration = -1f)
    {
        float d = duration > 0 ? duration : defaultFadeDuration;
        yield return StartCoroutine(Fade(0f, 1f, d));
    }

    /// Just fade in from black. Use after scene load.
    public IEnumerator FadeIn(float duration = -1f)
    {
        float d = duration > 0 ? duration : defaultFadeDuration;
        yield return StartCoroutine(Fade(1f, 0f, d));
    }

    /// Set alpha immediately without fading — useful on scene start.
    public void SetAlpha(float alpha)
    {
        if (fadeCanvasGroup != null)
            fadeCanvasGroup.alpha = alpha;
    }

    // ── Private ───────────────────────────────────────────────────────────────

    private IEnumerator Fade(float from, float to, float duration)
    {
        if (fadeCanvasGroup == null) yield break;

        float elapsed = 0f;
        fadeCanvasGroup.alpha = from;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime; // unscaled so it works when paused
            fadeCanvasGroup.alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }

        fadeCanvasGroup.alpha = to;
    }
}