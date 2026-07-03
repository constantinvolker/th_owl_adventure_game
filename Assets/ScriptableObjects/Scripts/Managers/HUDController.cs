using TMPro;
using UnityEngine;
using UnityEngine.UI;
using AdventureGame.TimeSystem;
using AdventureGame.WeatherSystem;

public class HUDController : MonoBehaviour
{
    public static HUDController Instance;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private Image weatherImage;

    [Header("Weather Sprites")]
    [SerializeField] private Sprite sunnySprite;
    [SerializeField] private Sprite cloudySprite;
    [SerializeField] private Sprite rainSprite;
    [SerializeField] private Sprite stormSprite;


    private void Awake()
    {
        // 🔒 Singleton-Schutz
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // 🌍 bleibt über Szenen hinweg bestehen
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // Initiale Werte setzen
        UpdateTime(TimeManager.Instance.CurrentHour, TimeManager.Instance.CurrentMinute);
        UpdateWeather(WeatherManager.Instance.CurrentWeather);

        // Events abonnieren
        TimeManager.Instance.OnMinuteChanged += UpdateTime;
        WeatherManager.Instance.OnWeatherChanged += UpdateWeather;
    }

    private void OnDestroy()
    {
        if (TimeManager.Instance != null)
            TimeManager.Instance.OnMinuteChanged -= UpdateTime;

        if (WeatherManager.Instance != null)
            WeatherManager.Instance.OnWeatherChanged -= UpdateWeather;
    }

    private void UpdateTime(int hour, int minute)
    {
        timeText.text = $"{hour:00}:{minute:00}";
    }

    private void UpdateWeather(WeatherType weather)
    {
        switch (weather)
        {
            case WeatherType.Sunny:
                weatherImage.sprite = sunnySprite;
                break;

            case WeatherType.Cloudy:
                weatherImage.sprite = cloudySprite;
                break;

            case WeatherType.Rain:
                weatherImage.sprite = rainSprite;
                break;

            case WeatherType.Storm:
                weatherImage.sprite = stormSprite;
                break;

        }
    }
}