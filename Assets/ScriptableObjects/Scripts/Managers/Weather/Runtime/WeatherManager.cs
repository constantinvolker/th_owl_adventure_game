using System;
using UnityEngine;
using AdventureGame.TimeSystem;

namespace AdventureGame.WeatherSystem
{
    /// <summary>
    /// Manages the in-game weather system, including automatic weather rolling and state persistence.
    /// Implements a Singleton pattern.
    /// </summary>
    public class WeatherManager : MonoBehaviour
    {
        /// <summary>
        /// The globally accessible Singleton instance of the WeatherManager.
        /// </summary>
        public static WeatherManager Instance { get; private set; }

        [Header("Settings")]
        [SerializeField]
        private WeatherSettingsSO settings;

        [Header("Runtime State")]
        [SerializeField]
        private WeatherType currentWeather;

        [SerializeField]
        private int lastChangedDay;

        [SerializeField]
        private int lastChangedHour;

        [SerializeField]
        private int lastChangedMinute;

        [SerializeField]
        private int lastRollSlot = -1;

        private bool isSubscribedToTime;

        /// <summary>
        /// Event triggered whenever the current weather changes.
        /// </summary>
        public event Action<WeatherType> OnWeatherChanged;

        /// <summary>
        /// Event triggered whenever a weather snapshot is created/updated.
        /// </summary>
        public event Action<WeatherSnapshot> OnWeatherSnapshotChanged;

        /// <summary>
        /// The currently active weather type.
        /// </summary>
        public WeatherType CurrentWeather => currentWeather;

        /// <summary>
        /// Gets a snapshot of the current weather state to use externally without retaining references.
        /// </summary>
        public WeatherSnapshot CurrentSnapshot
        {
            get
            {
                return new WeatherSnapshot(
                    currentWeather,
                    lastChangedDay,
                    lastChangedHour,
                    lastChangedMinute,
                    lastRollSlot
                );
            }
        }

        /// <summary>
        /// The number of times the weather is rolled per in-game day.
        /// </summary>
        private int RollsPerIngameDay
        {
            get
            {
                if (settings == null)
                {
                    return 4;
                }

                return Mathf.Max(1, settings.rollsPerIngameDay);
            }
        }

        /// <summary>
        /// Initializes the Singleton instance and secures the Manager to persist across scenes.
        /// Also applies initial settings.
        /// </summary>
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);

            ApplyStartSettings();
        }

        private void Start()
        {
            TrySubscribeToTimeManager();

            if (settings != null && settings.rollWeatherOnStart)
            {
                RollWeather();
            }
            else
            {
                NotifyWeatherChanged();
            }
        }

        private void OnDestroy()
        {
            UnsubscribeFromTimeManager();

            if (Instance == this)
            {
                Instance = null;
            }
        }

        private void ApplyStartSettings()
        {
            if (settings == null)
            {
                Debug.LogWarning("WeatherManager: No WeatherSettingsSO assigned. Using Sunny as default weather.");
                currentWeather = WeatherType.Sunny;
                return;
            }

            currentWeather = settings.startWeather;
        }

        private void TrySubscribeToTimeManager()
        {
            if (isSubscribedToTime)
            {
                return;
            }

            if (TimeManager.Instance == null)
            {
                Debug.LogWarning("WeatherManager: No TimeManager found. Weather will not change automatically.");
                return;
            }

            TimeManager.Instance.OnTimeChanged += HandleTimeChanged;
            isSubscribedToTime = true;

            TimeSnapshot time = TimeManager.Instance.CurrentSnapshot;
            lastRollSlot = CalculateRollSlot(time);
        }

        private void UnsubscribeFromTimeManager()
        {
            if (!isSubscribedToTime)
            {
                return;
            }

            if (TimeManager.Instance != null)
            {
                TimeManager.Instance.OnTimeChanged -= HandleTimeChanged;
            }

            isSubscribedToTime = false;
        }

        private void HandleTimeChanged(TimeSnapshot timeSnapshot)
        {
            int currentRollSlot = CalculateRollSlot(timeSnapshot);

            if (currentRollSlot == lastRollSlot)
            {
                return;
            }

            lastRollSlot = currentRollSlot;
            RollWeather();
        }

        private int CalculateRollSlot(TimeSnapshot timeSnapshot)
        {
            int slotInCurrentDay = Mathf.FloorToInt(timeSnapshot.DayProgress * RollsPerIngameDay);

            if (slotInCurrentDay >= RollsPerIngameDay)
            {
                slotInCurrentDay = RollsPerIngameDay - 1;
            }

            return timeSnapshot.CompletedDays * RollsPerIngameDay + slotInCurrentDay;
        }

        /// <summary>
        /// Forces a new random weather roll based on the configured weights.
        /// </summary>
        public void RollWeather()
        {
            WeatherType newWeather = PickRandomWeather();

            SetWeather(newWeather);
        }

        /// <summary>
        /// Manually sets a specific weather type and notifies listeners.
        /// </summary>
        /// <param name="newWeather">The desired weather type.</param>
        public void SetWeather(WeatherType newWeather)
        {
            if (newWeather == currentWeather)
            {
                NotifyWeatherChanged();
                return;
            }

            currentWeather = newWeather;

            if (TimeManager.Instance != null)
            {
                TimeSnapshot time = TimeManager.Instance.CurrentSnapshot;
                lastChangedDay = time.CompletedDays;
                lastChangedHour = time.Hour;
                lastChangedMinute = time.Minute;
            }

            NotifyWeatherChanged();
        }

        private WeatherType PickRandomWeather()
        {
            if (settings == null || settings.weatherChances == null || settings.weatherChances.Length == 0)
            {
                return WeatherType.Sunny;
            }

            int totalWeight = CalculateTotalWeight();

            if (totalWeight <= 0)
            {
                return currentWeather;
            }

            int randomValue = UnityEngine.Random.Range(0, totalWeight);
            int currentWeightSum = 0;

            foreach (WeatherChance chance in settings.weatherChances)
            {
                if (chance == null)
                {
                    continue;
                }

                int weight = Mathf.Max(0, chance.weight);

                if (settings.preventImmediateRepeat && chance.weatherType == currentWeather)
                {
                    weight = 0;
                }

                currentWeightSum += weight;

                if (randomValue < currentWeightSum)
                {
                    return chance.weatherType;
                }
            }

            return currentWeather;
        }

        private int CalculateTotalWeight()
        {
            int totalWeight = 0;

            foreach (WeatherChance chance in settings.weatherChances)
            {
                if (chance == null)
                {
                    continue;
                }

                if (settings.preventImmediateRepeat && chance.weatherType == currentWeather)
                {
                    continue;
                }

                totalWeight += Mathf.Max(0, chance.weight);
            }

            return totalWeight;
        }

        private void NotifyWeatherChanged()
        {
            OnWeatherChanged?.Invoke(currentWeather);
            OnWeatherSnapshotChanged?.Invoke(CurrentSnapshot);
        }

        /// <summary>
        /// Checks if the current weather matches the given type.
        /// </summary>
        /// <param name="weatherType">The weather type to check against.</param>
        /// <returns>True if the current weather is the given type.</returns>
        public bool IsWeather(WeatherType weatherType)
        {
            return currentWeather == weatherType;
        }

        /// <summary>
        /// Checks if the current weather is considered a rainy state (e.g., Rain or Storm).
        /// </summary>
        /// <returns>True if the weather is Rain or Storm.</returns>
        public bool IsRainyWeather()
        {
            return currentWeather == WeatherType.Rain
                || currentWeather == WeatherType.Storm;
        }

        /// <summary>
        /// Bundles the internal state into a save data object.
        /// </summary>
        /// <returns>A new <c>WeatherSaveData</c> representing the current runtime state.</returns>
        public WeatherSaveData GetSaveData()
        {
            return new WeatherSaveData
            {
                currentWeather = currentWeather,
                lastChangedDay = lastChangedDay,
                lastChangedHour = lastChangedHour,
                lastChangedMinute = lastChangedMinute,
                lastRollSlot = lastRollSlot
            };
        }

        /// <summary>
        /// Restores the manager's state from a provided save data object.
        /// </summary>
        /// <param name="data">The serialization data to load from.</param>
        public void LoadFromSaveData(WeatherSaveData data)
        {
            if (data == null)
            {
                Debug.LogWarning("WeatherManager: Tried to load null save data.");
                return;
            }

            currentWeather = data.currentWeather;
            lastChangedDay = Mathf.Max(0, data.lastChangedDay);
            lastChangedHour = Mathf.Clamp(data.lastChangedHour, 0, 23);
            lastChangedMinute = Mathf.Clamp(data.lastChangedMinute, 0, 59);
            lastRollSlot = Mathf.Max(-1, data.lastRollSlot);

            NotifyWeatherChanged();
        }

        /// <summary>
        /// Resets the weather system to its starting state defined in the settings.
        /// Useful when starting a new game from the main menu.
        /// </summary>
        public void ResetWeather()
        {
            lastChangedDay = 0;
            lastChangedHour = 0;
            lastChangedMinute = 0;
            lastRollSlot = -1;
            
            ApplyStartSettings();
            
            if (TimeManager.Instance != null && isSubscribedToTime)
            {
                TimeSnapshot time = TimeManager.Instance.CurrentSnapshot;
                lastRollSlot = CalculateRollSlot(time);
            }
            
            if (settings != null && settings.rollWeatherOnStart)
            {
                RollWeather();
            }
            else
            {
                NotifyWeatherChanged();
            }
        }
    }
}