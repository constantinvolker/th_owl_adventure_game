using UnityEngine;

namespace AdventureGame.WeatherSystem
{
    [CreateAssetMenu(
        fileName = "WeatherSettings",
        menuName = "Adventure/Weather/Weather Settings"
    )]
    public class WeatherSettingsSO : ScriptableObject
    {
        [Header("Start Weather")]
        public WeatherType startWeather = WeatherType.Sunny;

        [Tooltip("If true, the weather is randomized immediately when the WeatherManager starts.")]
        public bool rollWeatherOnStart = false;

        [Header("Weather Change Timing")]
        [Min(1)]
        public int rollsPerIngameDay = 4;

        [Header("Behaviour")]
        public bool preventImmediateRepeat = true;

        [Header("Weather Chances")]
        public WeatherChance[] weatherChances =
        {
            new WeatherChance { weatherType = WeatherType.Sunny, weight = 70 },
            new WeatherChance { weatherType = WeatherType.Cloudy, weight = 20 },
            new WeatherChance { weatherType = WeatherType.Rain, weight = 8 },
            new WeatherChance { weatherType = WeatherType.Storm, weight = 2 }
        };

        private void OnValidate()
        {
            rollsPerIngameDay = Mathf.Max(1, rollsPerIngameDay);

            if (weatherChances == null)
            {
                return;
            }

            foreach (WeatherChance chance in weatherChances)
            {
                if (chance != null)
                {
                    chance.weight = Mathf.Max(0, chance.weight);
                }
            }
        }
    }
}