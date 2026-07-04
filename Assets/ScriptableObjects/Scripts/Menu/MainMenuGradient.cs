using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Vertikaler Gradient-Hintergrund für das MainMenu.
/// Funktioniert direkt im Editor – kein Play Mode nötig!
///
/// SETUP:
///   1. "Background" GameObject anklicken
///   2. Altes MainMenuGradient entfernen (falls vorhanden)
///   3. Dieses Skript per Add Component hinzufügen
///   4. Top Color / Bottom Color im Inspector wählen → sofort sichtbar
/// </summary>
[ExecuteAlways]
[RequireComponent(typeof(Image))]
public class MainMenuGradient : MonoBehaviour
{
    [Header("Gradient Farben")]
    public Color topColor    = new Color(0.08f, 0.15f, 0.30f, 1f);   // dunkles Nachtblau
    public Color bottomColor = new Color(0.25f, 0.52f, 0.72f, 1f);   // helles Himmelblau

    private Image      _baseImage;
    private GameObject _overlayGO;
    private Image      _overlayImage;

    void OnEnable()   => Rebuild();
    void OnValidate() => Rebuild();  // jede Inspector-Änderung triggert sofortiges Update

    void Rebuild()
    {
        // ── Basis-Image = Bottom Color ─────────────────────────────────────────
        _baseImage       = GetComponent<Image>();
        _baseImage.color = bottomColor;
        StretchToParent(_baseImage.rectTransform);

        // ── Overlay suchen oder anlegen ────────────────────────────────────────
        var existing = transform.Find("__GradientOverlay");
        if (existing != null)
        {
            _overlayGO    = existing.gameObject;
            _overlayImage = _overlayGO.GetComponent<Image>();
        }
        else
        {
            _overlayGO           = new GameObject("__GradientOverlay");
            _overlayGO.transform.SetParent(transform, false);
            _overlayGO.hideFlags = HideFlags.DontSave;
            _overlayImage        = _overlayGO.AddComponent<Image>();
        }

        // ── Overlay = Top Color, nach unten transparent ────────────────────────
        _overlayImage.color          = topColor;
        _overlayImage.raycastTarget  = false;
        _overlayImage.sprite         = MakeGradientSprite();
        _overlayImage.type           = Image.Type.Simple;
        _overlayImage.preserveAspect = false;

        StretchToParent(_overlayImage.rectTransform);
    }

    /// Erstellt eine 1×2-Pixel Textur: unten transparent, oben opak
    Sprite MakeGradientSprite()
    {
        var tex        = new Texture2D(1, 2, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Bilinear;
        tex.wrapMode   = TextureWrapMode.Clamp;
        tex.SetPixel(0, 0, new Color(1, 1, 1, 0));  // Pixel unten → transparent
        tex.SetPixel(0, 1, new Color(1, 1, 1, 1));  // Pixel oben  → opak
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, 1, 2), new Vector2(0.5f, 0.5f));
    }

    void StretchToParent(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }
}
