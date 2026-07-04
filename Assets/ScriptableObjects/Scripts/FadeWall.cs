using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class FadeWall : MonoBehaviour
{
    [Range(0f, 1f)] public float visibleAlpha  = 1f;
    [Range(0f, 1f)] public float hiddenAlpha   = 0f;
    public float fadeDuration = 0.25f;

    private SpriteRenderer _sr;
    private Coroutine      _fadeCoroutine;

    void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
        GetComponent<Collider2D>().isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        Fade(hiddenAlpha);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        Fade(visibleAlpha);
    }

    private void Fade(float targetAlpha)
    {
        if (!gameObject.activeInHierarchy)
        {
            // Can't run coroutine — set alpha directly
            Color c = _sr.color;
            c.a = targetAlpha;
            _sr.color = c;
            return;
        }

        if (_fadeCoroutine != null) StopCoroutine(_fadeCoroutine);
        _fadeCoroutine = StartCoroutine(FadeRoutine(targetAlpha));
    }

    private IEnumerator FadeRoutine(float targetAlpha)
    {
        float startAlpha = _sr.color.a;
        float elapsed    = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            Color c = _sr.color;
            c.a = Mathf.Lerp(startAlpha, targetAlpha, Mathf.Clamp01(elapsed / fadeDuration));
            _sr.color = c;
            yield return null;
        }

        Color final = _sr.color;
        final.a = targetAlpha;
        _sr.color = final;
    }
}