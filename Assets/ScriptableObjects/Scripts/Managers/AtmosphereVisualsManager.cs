using UnityEngine;
using UnityEngine.Rendering.Universal;
using AdventureGame.TimeSystem;
using AdventureGame.WeatherSystem;

public class AtmosphereVisualsManager : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Ziehe hier dein 2D Licht (z.B. Freeform für den Hof) rein.")]
    public Light2D targetLight;

    [Tooltip("Ziehe hier dein Partikelsystem für den Regen rein.")]
    public ParticleSystem rainParticleSystem;

    [Header("Audio")]
    [Tooltip("Ziehe hier die AudioSource rein (die am besten auf dem Regen-Partikelsystem liegt).")]
    public AudioSource weatherAudioSource;
    public AudioClip rainSound;
    public AudioClip stormSound;
    [Range(0f, 1f)] public float maxWeatherVolume = 1f;

    [Header("Base Day/Night Settings")]
    public Gradient dailyColor;
    public AnimationCurve dailyIntensity;

    [Header("Night Lights (Laternen, Fenster, etc.)")]
    [Tooltip("Ziehe hier alle 2D Lichter rein, die nachts an sein sollen.")]
    public Light2D[] nightLights;
    [Tooltip("Kurve für Laternen: 0 (Mitternacht) = hoch, 0.5 (Mittag) = null, 1 (Mitternacht) = hoch.")]
    public AnimationCurve nightLightsCurve;
    [Tooltip("Wie hell sollen die Laternen maximal leuchten?")]
    public float maxNightLightIntensity = 1f;

    [Header("Weather Modifiers (Cloudy)")]
    [Range(0f, 1f)] public float cloudyIntensityMultiplier = 0.8f; // Leicht dunkler als sonnig
    public Color cloudyColorTint = new Color(0.8f, 0.8f, 0.85f); // Leichtes Grau

    [Header("Weather Modifiers (Rain)")]
    [Range(0f, 1f)] public float rainIntensityMultiplier = 0.6f; // Macht das Licht auf 60% dunkler
    public Color rainColorTint = new Color(0.7f, 0.75f, 0.85f); // Leichtes Grau-Blau
    public float rainEmissionRate = 60f; // Wie viele Tropfen pro Sekunde

    [Header("Weather Modifiers (Storm)")]
    [Range(0f, 1f)] public float stormIntensityMultiplier = 0.4f;
    public Color stormColorTint = new Color(0.5f, 0.55f, 0.65f);
    public float stormEmissionRate = 150f;

    [Header("Transitions")]
    [Tooltip("Wie schnell wechselt das Wetter visuell (höher = schneller)")]
    public float weatherTransitionSpeed = 2f;

    private float targetWeatherIntensityMod = 1f;
    private Color targetWeatherColorMod = Color.white;
    private float targetAudioVolume = 0f; // Ziel-Lautstärke für den Sound

    private float currentWeatherIntensityMod = 1f;
    private Color currentWeatherColorMod = Color.white;

    private bool isSubscribedToWeather = false;

    private void OnEnable()
    {
        Debug.Log($"[AtmosphereVisualsManager] OnEnable - WeatherManager.Instance: {(WeatherManager.Instance != null ? "EXISTS" : "NULL")}");
        Debug.Log($"[AtmosphereVisualsManager] rainParticleSystem assigned: {(rainParticleSystem != null ? "YES" : "NO")}");
        Debug.Log($"[AtmosphereVisualsManager] targetLight assigned: {(targetLight != null ? "YES" : "NO")}");

        // AudioSource sicherstellen, dass sie zu Beginn stumm ist
        if (weatherAudioSource != null) weatherAudioSource.volume = 0f;

        TrySubscribeToWeatherManager();
    }

    private void OnDisable()
    {
        UnsubscribeFromWeatherManager();
    }

    private void Start()
    {
        // Fallback für den Fall, dass OnEnable vor WeatherManager.Awake() aufgerufen wurde
        TrySubscribeToWeatherManager();
    }

    private void TrySubscribeToWeatherManager()
    {
        if (isSubscribedToWeather)
            return;

        if (WeatherManager.Instance == null)
        {
            Debug.LogWarning("[AtmosphereVisualsManager] WeatherManager.Instance ist noch nicht verfügbar. Wird in Start() versucht.");
            return;
        }

        WeatherManager.Instance.OnWeatherChanged += HandleWeatherChanged;
        isSubscribedToWeather = true;

        // Direkt den aktuellen Wetterzustand anwenden
        ApplyWeatherVisualState(WeatherManager.Instance.CurrentWeather, instant: true);
        Debug.Log($"[AtmosphereVisualsManager] Erfolgreich abonniert. Aktuelles Wetter: {WeatherManager.Instance.CurrentWeather}");
    }

    private void UnsubscribeFromWeatherManager()
    {
        if (!isSubscribedToWeather || WeatherManager.Instance == null)
            return;

        WeatherManager.Instance.OnWeatherChanged -= HandleWeatherChanged;
        isSubscribedToWeather = false;
    }

    private void HandleWeatherChanged(WeatherType newWeather)
    {
        Debug.Log($"[AtmosphereVisualsManager] Wetter geändert zu: {newWeather}");
        ApplyWeatherVisualState(newWeather, instant: false);
    }

    private void ApplyWeatherVisualState(WeatherType weather, bool instant)
    {
        Debug.Log($"[AtmosphereVisualsManager] Applying weather state: {weather}, rainParticleSystem is {(rainParticleSystem != null ? "assigned" : "NULL")}");

        // 1. Partikel-Effekte steuern (nur bei Regen/Sturm)
        if (rainParticleSystem != null)
        {
            if (weather == WeatherType.Rain || weather == WeatherType.Storm)
            {
                Debug.Log($"[AtmosphereVisualsManager] Starting rain particles with rate: {(weather == WeatherType.Storm ? stormEmissionRate : rainEmissionRate)}");
                if (!rainParticleSystem.isPlaying)
                    rainParticleSystem.Play();

                var emission = rainParticleSystem.emission;
                emission.rateOverTime = (weather == WeatherType.Storm) ? stormEmissionRate : rainEmissionRate;
            }
            else
            {
                Debug.Log("[AtmosphereVisualsManager] Stopping rain particles");
                if (rainParticleSystem.isPlaying)
                    rainParticleSystem.Stop();
            }
        }

        // 2. Licht- und Sound-Modifikatoren basierend auf dem Enum bestimmen
        switch (weather)
        {
            case WeatherType.Cloudy:
                targetWeatherIntensityMod = cloudyIntensityMultiplier;
                targetWeatherColorMod = cloudyColorTint;
                targetAudioVolume = 0f; // Kein Sound bei nur Bewölkung
                break;

            case WeatherType.Rain:
                targetWeatherIntensityMod = rainIntensityMultiplier;
                targetWeatherColorMod = rainColorTint;
                targetAudioVolume = maxWeatherVolume; // Sound an

                // Soundclip wechseln, falls nötig
                if (weatherAudioSource != null && weatherAudioSource.clip != rainSound)
                {
                    weatherAudioSource.clip = rainSound;
                    weatherAudioSource.Play();
                }
                break;

            case WeatherType.Storm:
                targetWeatherIntensityMod = stormIntensityMultiplier;
                targetWeatherColorMod = stormColorTint;
                targetAudioVolume = maxWeatherVolume; // Sound an

                // Soundclip wechseln, falls nötig
                if (weatherAudioSource != null && weatherAudioSource.clip != stormSound)
                {
                    weatherAudioSource.clip = stormSound;
                    weatherAudioSource.Play();
                }
                break;

            case WeatherType.Sunny:
            default:
                targetWeatherIntensityMod = 1f;
                targetWeatherColorMod = Color.white; // Keine Veränderung bei Sonne
                targetAudioVolume = 0f; // Sound aus
                break;
        }

        // Wenn es sofort passieren soll
        if (instant)
        {
            currentWeatherIntensityMod = targetWeatherIntensityMod;
            currentWeatherColorMod = targetWeatherColorMod;

            if (weatherAudioSource != null)
            {
                weatherAudioSource.volume = targetAudioVolume;
                if (targetAudioVolume > 0 && !weatherAudioSource.isPlaying)
                {
                    weatherAudioSource.Play();
                }
            }
        }
    }

    void Update()
    {
        if (targetLight == null)
            return;

        // Versuche zu abonnieren, falls noch nicht geschehen
        if (!isSubscribedToWeather)
        {
            TrySubscribeToWeatherManager();
        }

        // --- 1. TAG/NACHT BASIS BERECHNEN ---
        float timePercent = TimeManager.Instance != null ? TimeManager.Instance.DayProgress : 0.5f;
        Color baseColor = dailyColor.Evaluate(timePercent);
        float baseIntensity = dailyIntensity.Evaluate(timePercent);

        // --- 2. WETTER INTEGRATION (SANFTER ÜBERGANG) ---
        currentWeatherIntensityMod = Mathf.MoveTowards(currentWeatherIntensityMod, targetWeatherIntensityMod, Time.deltaTime * weatherTransitionSpeed);
        currentWeatherColorMod = Color.Lerp(currentWeatherColorMod, targetWeatherColorMod, Time.deltaTime * weatherTransitionSpeed);

        // --- 3. KOMBINATION ANWENDEN ---
        targetLight.color = baseColor * currentWeatherColorMod;
        targetLight.intensity = baseIntensity * currentWeatherIntensityMod;

        // --- 4. AUDIO FADE ---
        if (weatherAudioSource != null)
        {
            weatherAudioSource.volume = Mathf.MoveTowards(weatherAudioSource.volume, targetAudioVolume, Time.deltaTime * (weatherTransitionSpeed * 0.5f));

            if (weatherAudioSource.volume <= 0f && weatherAudioSource.isPlaying)
            {
                weatherAudioSource.Stop();
            }
            else if (weatherAudioSource.volume > 0f && !weatherAudioSource.isPlaying)
            {
                weatherAudioSource.Play();
            }
        }

        // --- 5. NACHTLICHTER (LATERNEN) STEUERN ---
        if (nightLights != null && nightLights.Length > 0)
        {
            // Berechne die aktuelle Helligkeit der Laternen anhand der Zeit-Kurve
            float currentNightIntensity = nightLightsCurve.Evaluate(timePercent) * maxNightLightIntensity;

            // Auch Laternen können bei Unwetter leicht schwanken/dunkler werden, 
            // wenn du das möchtest. Hier nehmen wir einfach die Basis-Intensität.
            foreach (Light2D light in nightLights)
            {
                if (light != null)
                {
                    light.intensity = currentNightIntensity;

                    // Performance-Optimierung: Lichtkomponente ganz ausschalten, wenn es Tag ist (Wert nahe 0)
                    light.enabled = currentNightIntensity > 0.01f;
                }
            }
        }
    }
}