using UnityEngine;
using AdventureGame.WeatherSystem;

public class WeatherAudioManager : MonoBehaviour
{
    [Header("References")]
    [Tooltip("AudioSource für die Hintergrundmusik")]
    public AudioSource audioSource;

    [Header("Weather Audio Clips")]
    [Tooltip("Musik für sonniges Wetter")]
    public AudioClip sunnyClip;

    [Tooltip("Musik für bewölktes Wetter")]
    public AudioClip cloudyClip;

    [Tooltip("Musik für Regen")]
    public AudioClip rainClip;

    [Tooltip("Musik für Sturm")]
    public AudioClip stormClip;

    [Header("Audio Settings")]
    [Range(0f, 1f)] public float musicVolume = 0.7f;
    [Tooltip("Wie schnell der Musikübergang stattfindet (höher = schneller)")]
    public float crossfadeSpeed = 2f;

    private AudioClip targetClip;
    private float targetVolume = 0f;
    private float currentVolume = 0f;
    private bool isSubscribedToWeather = false;

    private void OnEnable()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                Debug.LogError("[WeatherAudioManager] Keine AudioSource gefunden! Bitte zuweisen.");
                return;
            }
        }

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
            Debug.LogWarning("[WeatherAudioManager] WeatherManager.Instance ist noch nicht verfügbar.");
            return;
        }

        WeatherManager.Instance.OnWeatherChanged += HandleWeatherChanged;
        isSubscribedToWeather = true;

        // Aktuellen Wetterzustand anwenden
        ApplyWeatherAudioState(WeatherManager.Instance.CurrentWeather, instant: true);
        Debug.Log($"[WeatherAudioManager] Erfolgreich abonniert. Aktuelles Wetter: {WeatherManager.Instance.CurrentWeather}");
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
        Debug.Log($"[WeatherAudioManager] Wetter geändert zu: {newWeather}");
        ApplyWeatherAudioState(newWeather, instant: false);
    }

    private void ApplyWeatherAudioState(WeatherType weather, bool instant)
    {
        // Bestimme den Audioclip basierend auf dem Wetter
        targetClip = weather switch
        {
            WeatherType.Sunny => sunnyClip,
            WeatherType.Cloudy => cloudyClip,
            WeatherType.Rain => rainClip,
            WeatherType.Storm => stormClip,
            _ => null
        };

        if (targetClip != null)
        {
            targetVolume = musicVolume;

            if (instant)
            {
                audioSource.clip = targetClip;
                audioSource.volume = targetVolume;
                audioSource.Play();
                currentVolume = targetVolume;
                Debug.Log($"[WeatherAudioManager] Musik sofort gestartet: {targetClip.name}");
            }
            else
            {
                Debug.Log($"[WeatherAudioManager] Musik wird übergeblendet zu: {targetClip.name}");
            }
        }
        else
        {
            targetVolume = 0f;
            Debug.LogWarning($"[WeatherAudioManager] Kein AudioClip für Wetter '{weather}' zugewiesen!");
        }
    }

    private void Update()
    {
        if (audioSource == null)
            return;

        // Versuche zu abonnieren, falls noch nicht geschehen
        if (!isSubscribedToWeather)
        {
            TrySubscribeToWeatherManager();
        }

        // Sanfte Volumen-Anpassung
        currentVolume = Mathf.MoveTowards(currentVolume, targetVolume, Time.deltaTime * crossfadeSpeed);
        audioSource.volume = currentVolume;

        // Wenn der Clip wechseln soll und das Volumen leise genug ist
        if (audioSource.clip != targetClip && currentVolume < 0.01f)
        {
            if (targetClip != null)
            {
                audioSource.clip = targetClip;
                audioSource.Play();
                Debug.Log($"[WeatherAudioManager] Clip gewechselt zu: {targetClip.name}");
            }
            else
            {
                audioSource.Stop();
            }
        }
    }
}