using UnityEngine;
using UnityEngine.Rendering.Universal;
using AdventureGame.TimeSystem;    // Für den TimeManager
using AdventureGame.WeatherSystem; // Für den WeatherManager und WeatherType

public class AtmosphereVisualsManager : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Ziehe hier dein 2D Licht (z.B. Freeform für den Hof) rein.")]
    public Light2D targetLight;

    [Tooltip("Ziehe hier dein Partikelsystem für den Regen rein.")]
    public ParticleSystem rainParticleSystem;

    [Header("Base Day/Night Settings")]
    public Gradient dailyColor;
    public AnimationCurve dailyIntensity;

    [Header("Weather Modifiers (Rain)")]
    [Range(0f, 1f)] public float rainIntensityMultiplier = 0.6f; // Macht das Licht auf 60% dunkler
    public Color rainColorTint = new Color(0.7f, 0.75f, 0.85f); // Leichtes Grau-Blau
    public float rainEmissionRate = 60f; // Wie viele Tropfen pro Sekunde

    [Header("Weather Modifiers (Storm)")]
    [Range(0f, 1f)] public float stormIntensityMultiplier = 0.4f; // Macht das Licht auf 40% dunkler
    public Color stormColorTint = new Color(0.5f, 0.55f, 0.65f); // Dunkleres, stürmisches Grau-Blau
    public float stormEmissionRate = 150f;

    [Header("Transitions")]
    [Tooltip("Wie schnell wechselt das Wetter visuell (höher = schneller)")]
    public float weatherTransitionSpeed = 2f;

    // Interne Zielwerte für das sanfte Einblenden des Wetters
    private float targetWeatherIntensityMod = 1f;
    private Color targetWeatherColorMod = Color.white;

    // Aktuelle Übergangswerte
    private float currentWeatherIntensityMod = 1f;
    private Color currentWeatherColorMod = Color.white;

    private void OnEnable()
    {
        // Wir abonnieren das Wetter-Event
        if (WeatherManager.Instance != null)
        {
            WeatherManager.Instance.OnWeatherChanged += HandleWeatherChanged;
            // Direkt den aktuellen Startwert setzen
            ApplyWeatherVisualState(WeatherManager.Instance.CurrentWeather, instant: true);
        }
    }

    private void OnDisable()
    {
        if (WeatherManager.Instance != null)
        {
            WeatherManager.Instance.OnWeatherChanged -= HandleWeatherChanged;
        }
    }

    private void Start()
    {
        // Fallback, falls beim Aktivieren der Manager noch nicht bereit war
        if (WeatherManager.Instance != null)
        {
            ApplyWeatherVisualState(WeatherManager.Instance.CurrentWeather, instant: true);
        }
    }

    private void HandleWeatherChanged(WeatherType newWeather)
    {
        // Wenn das Event feuert, passen wir die visuellen Ziele an
        ApplyWeatherVisualState(newWeather, instant: false);
    }

    private void ApplyWeatherVisualState(WeatherType weather, bool instant)
    {
        // 1. Partikel-Effekte steuern
        if (rainParticleSystem != null)
        {
            if (weather == WeatherType.Rain || weather == WeatherType.Storm)
            {
                if (!rainParticleSystem.isPlaying) rainParticleSystem.Play();

                // Emissionsrate dynamisch anpassen (Sturm heftiger als Regen)
                var emission = rainParticleSystem.emission;
                emission.rateOverTime = (weather == WeatherType.Storm) ? stormEmissionRate : rainEmissionRate;
            }
            else
            {
                if (rainParticleSystem.isPlaying) rainParticleSystem.Stop();
            }
        }

        // 2. Licht-Modifikatoren basierend auf dem Enum bestimmen
        switch (weather)
        {
            case WeatherType.Rain:
                targetWeatherIntensityMod = rainIntensityMultiplier;
                targetWeatherColorMod = rainColorTint;
                break;

            case WeatherType.Storm:
                targetWeatherIntensityMod = stormIntensityMultiplier;
                targetWeatherColorMod = stormColorTint;
                break;

            case WeatherType.Sunny:
            default:
                targetWeatherIntensityMod = 1f;
                targetWeatherColorMod = Color.white; // Keine Veränderung bei Sonne
                break;
        }

        // Wenn es sofort passieren soll (z.B. beim Szenenstart/Laden)
        if (instant)
        {
            currentWeatherIntensityMod = targetWeatherIntensityMod;
            currentWeatherColorMod = targetWeatherColorMod;
        }
    }

    void Update()
    {
        if (targetLight == null) return;

        // --- 1. TAG/NACHT BASIS BERECHNEN ---
        float timePercent = TimeManager.Instance != null ? TimeManager.Instance.DayProgress : 0.5f;
        Color baseColor = dailyColor.Evaluate(timePercent);
        float baseIntensity = dailyIntensity.Evaluate(timePercent);

        // --- 2. WETTER INTEGRATION (SANFTER ÜBERGANG) ---
        // Lerp sorgt dafür, dass das Licht nicht schlagartig dunkel wird, wenn es anfängt zu regnen
        currentWeatherIntensityMod = Mathf.MoveTowards(currentWeatherIntensityMod, targetWeatherIntensityMod, Time.deltaTime * weatherTransitionSpeed);
        currentWeatherColorMod = Color.Lerp(currentWeatherColorMod, targetWeatherColorMod, Time.deltaTime * weatherTransitionSpeed);

        // --- 3. KOMBINATION ANWENDEN ---
        // Wir multiplizieren die Tag-Nacht-Farbe mit dem Wetter-Farbton
        targetLight.color = baseColor * currentWeatherColorMod;
        // Wir multiplizieren die Tag-Nacht-Helligkeit mit dem Wetter-Dimmfaktor
        targetLight.intensity = baseIntensity * currentWeatherIntensityMod;
    }
}