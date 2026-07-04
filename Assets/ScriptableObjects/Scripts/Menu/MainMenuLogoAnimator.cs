using UnityEngine;

/// <summary>
/// Attach to the Logo GameObject in the MainMenu.
/// On Start: scales from 0 → 1 with an elastic/bounce overshoot.
/// After intro: gently bobs up and down (sine wave).
/// </summary>
public class MainMenuLogoAnimator : MonoBehaviour
{
    // ── Inspector ─────────────────────────────────────────────────────────────

    [Header("Bounce-In")]
    [Tooltip("Duration of the scale-in animation in seconds")]
    public float bounceInDuration = 0.7f;

    [Tooltip("How much the logo overshoots before settling (0 = no overshoot, 0.3 = nice bounce)")]
    [Range(0f, 0.5f)]
    public float overshoot = 0.25f;

    [Header("Idle Float")]
    [Tooltip("Pixels the logo moves up and down")]
    public float floatAmplitude = 8f;

    [Tooltip("How many full cycles per second")]
    public float floatFrequency = 0.5f;

    // ── Private ───────────────────────────────────────────────────────────────

    private RectTransform _rt;
    private Vector3       _basePosition;
    private float         _elapsed = 0f;
    private bool          _introComplete = false;

    // ── Unity ─────────────────────────────────────────────────────────────────

    void Awake()
    {
        _rt = GetComponent<RectTransform>();
        _basePosition   = _rt.anchoredPosition;
        _rt.localScale  = Vector3.zero;
    }

    void Update()
    {
        _elapsed += Time.deltaTime;

        if (!_introComplete)
        {
            RunBounceIn();
        }
        else
        {
            RunIdleFloat();
        }
    }

    // ── Animations ────────────────────────────────────────────────────────────

    void RunBounceIn()
    {
        float t = Mathf.Clamp01(_elapsed / bounceInDuration);

        // Elastic ease-out: standard formula with configurable overshoot
        float scale = ElasticEaseOut(t, overshoot);
        _rt.localScale = new Vector3(scale, scale, 1f);

        if (t >= 1f)
        {
            _rt.localScale    = Vector3.one;   // snap to exact 1
            _elapsed          = 0f;            // reset timer for idle phase
            _introComplete    = true;
        }
    }

    void RunIdleFloat()
    {
        float offsetY = Mathf.Sin(_elapsed * floatFrequency * Mathf.PI * 2f) * floatAmplitude;
        _rt.anchoredPosition = _basePosition + new Vector3(0f, offsetY, 0f);
    }

    // ── Math helper ───────────────────────────────────────────────────────────

    /// Elastic ease-out — overshoots slightly then settles at 1.
    static float ElasticEaseOut(float t, float overshootAmount)
    {
        if (t <= 0f) return 0f;
        if (t >= 1f) return 1f;

        // Two-phase approach: fast rise then damped overshoot
        // Uses a simple spring-like formula that's easy to read.
        float s = 1f + overshootAmount;
        return 1f + s * Mathf.Pow(t - 1f, 3f) + overshootAmount * Mathf.Pow(t - 1f, 2f);
    }
}
