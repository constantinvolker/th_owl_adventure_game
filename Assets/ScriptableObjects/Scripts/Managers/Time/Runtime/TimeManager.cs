using System;
using UnityEngine;

namespace AdventureGame.TimeSystem
{


    /// <summary>
    /// Manages the in-game time, day progression, and time-related events.
    /// Implements a Singleton pattern and persists across scenes.
    /// </summary>
    public class TimeManager : MonoBehaviour
    {
    /// <summary>
    /// The globally accessible Singleton instance of the TimeManager.
    /// </summary>
    public static TimeManager Instance { get; private set; }

    [Header("Settings")]
    [SerializeField]
    private TimeSettingsSO settings;

    [Header("Runtime State")]
    [SerializeField, Range(0f, 0.9999f)]
    private float dayProgress;

    [SerializeField]
    private bool isPaused;

    [SerializeField]
    private int completedDays;

    private int lastMinute = -1;
    private int lastHour = -1;

    /// <summary>
    /// Event triggered whenever the in-game minute changes. Provides the current hour and minute.
    /// </summary>
    public event Action<int, int> OnMinuteChanged;
    
    /// <summary>
    /// Event triggered whenever the in-game hour changes. Provides the current hour.
    /// </summary>
    public event Action<int> OnHourChanged;
    
    /// <summary>
    /// Event triggered when a full in-game day concludes. Provides the total number of completed days.
    /// </summary>
    public event Action<int> OnDayCompleted;
    
    /// <summary>
    /// Event triggered on any significant time update. Provides a current snapshot of the time state.
    /// </summary>
    public event Action<TimeSnapshot> OnTimeChanged;

    /// <summary>
    /// Represents the current day's progress as a normalized value between 0.0 and 1.0.
    /// </summary>
    public float DayProgress => dayProgress;

    /// <summary>
    /// Indicates whether the in-game time is currently paused.
    /// </summary>
    public bool IsPaused => isPaused;

    /// <summary>
    /// The total number of fully completed days since the tracker started.
    /// </summary>
    public int CompletedDays => completedDays;

    /// <summary>
    /// The current in-game hour (0-23).
    /// </summary>
    public int CurrentHour => TotalMinutes / 60;

    /// <summary>
    /// The current in-game minute (0-59).
    /// </summary>
    public int CurrentMinute => TotalMinutes % 60;

    /// <summary>
    /// The current time formatted as a standard HH:mm string.
    /// </summary>
    public string FormattedTime => $"{CurrentHour:00}:{CurrentMinute:00}";

    /// <summary>
    /// Gets a snapshot of the current time state to use externally without retaining references.
    /// </summary>
    public TimeSnapshot CurrentSnapshot
    {
        get
        {
            return new TimeSnapshot(
                dayProgress,
                completedDays,
                CurrentHour,
                CurrentMinute,
                FormattedTime,
                isPaused
            );
        }
    }

    /// <summary>
    /// Parses the raw day progress into total elapsed minutes for the current day.
    /// </summary>
    private int TotalMinutes
    {
        get
        {
            int minutes = Mathf.FloorToInt(dayProgress * 1440f);
            return Mathf.Clamp(minutes, 0, 1439);
        }
    }

    /// <summary>
    /// Calculates the number of real-world seconds it takes to complete one in-game day.
    /// Falls back to a default value if settings are missing.
    /// </summary>
    private float RealSecondsPerIngameDay
    {
        get
        {
            if (settings == null)
            {
                return 1200f;
            }

            return Mathf.Max(1f, settings.realSecondsPerIngameDay);
        }
    }

    /// <summary>
    /// Determines whether to use unscaled time based on the active settings.
    /// </summary>
    private bool UseUnscaledTime
    {
        get
        {
            return settings != null && settings.useUnscaledTime;
        }
    }

    /// <summary>
    /// Initializes the Singleton instance and secures the Manager to persist across scenes.
    /// Also applies initial settings if appropriate.
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
        NotifyTimeChanged(true);
    }

    /// <summary>
    /// Updates the time progress each frame depending on the selected scale mode and pause state.
    /// </summary>
    private void Update()
    {
        if (isPaused)
        {
            return;
        }

        float deltaTime = UseUnscaledTime
            ? Time.unscaledDeltaTime
            : Time.deltaTime;

        AdvanceTime(deltaTime);
    }

    /// <summary>
    /// Advances in-game time based on the elapsed real seconds.
    /// Handles day completion logic when the daily progress reaches or exceeds 1.0.
    /// </summary>
    /// <param name="realSeconds">Elapsed real-world seconds since the last frame.</param>
    private void AdvanceTime(float realSeconds)
    {
        if (realSeconds <= 0f)
        {
            return;
        }

        dayProgress += realSeconds / RealSecondsPerIngameDay;

        while (dayProgress >= 1f)
        {
            dayProgress -= 1f;
            completedDays++;
            OnDayCompleted?.Invoke(completedDays);
        }

        NotifyTimeChanged(false);
    }

    /// <summary>
    /// Applies the default starting time and pause state configured in the <c>TimeSettingsSO</c>.
    /// Provides fallback defaults if no settings object is assigned.
    /// </summary>
    private void ApplyStartSettings()
    {
        if (settings == null)
        {
            Debug.LogWarning("TimeManager: No TimeSettingsSO assigned. Using default start time 08:00.");
            SetProgressFromTime(8, 0);
            isPaused = false;
            return;
        }

        SetProgressFromTime(settings.startHour, settings.startMinute);
        isPaused = settings.startPaused;
    }

    /// <summary>
    /// Calculates and sets the day progress required to reach a specific hour and minute.
    /// </summary>
    /// <param name="hour">The target hour (0-23).</param>
    /// <param name="minute">The target minute (0-59).</param>
    private void SetProgressFromTime(int hour, int minute)
    {
        hour = Mathf.Clamp(hour, 0, 23);
        minute = Mathf.Clamp(minute, 0, 59);

        int totalMinutes = hour * 60 + minute;
        dayProgress = totalMinutes / 1440f;
    }

    /// <summary>
    /// Checks if a time boundary has been crossed and triggers the corresponding events.
    /// </summary>
    /// <param name="force">If true, bypasses the change detection and forces the events to trigger.</param>
    private void NotifyTimeChanged(bool force)
    {
        int hour = CurrentHour;
        int minute = CurrentMinute;

        bool minuteChanged = force || minute != lastMinute;
        bool hourChanged = force || hour != lastHour;

        if (minuteChanged)
        {
            lastMinute = minute;
            OnMinuteChanged?.Invoke(hour, minute);
        }

        if (hourChanged)
        {
            lastHour = hour;
            OnHourChanged?.Invoke(hour);
        }

        if (force || minuteChanged || hourChanged)
        {
            OnTimeChanged?.Invoke(CurrentSnapshot);
        }
    }

    /// <summary>
    /// Pauses the progression of time. Will immediately invoke a time snapshot update.
    /// </summary>
    public void PauseTime()
    {
        isPaused = true;
        OnTimeChanged?.Invoke(CurrentSnapshot);
    }

    /// <summary>
    /// Resumes the progression of time. Will immediately invoke a time snapshot update.
    /// </summary>
    public void ResumeTime()
    {
        isPaused = false;
        OnTimeChanged?.Invoke(CurrentSnapshot);
    }

    /// <summary>
    /// Forces the in-game clock to a specific hour and minute. Does not increment the completed days.
    /// Use <see cref="AddMinutes(int)"/> if time should advance forward naturally.
    /// </summary>
    /// <param name="hour">Target hour (0-23).</param>
    /// <param name="minute">Target minute (0-59).</param>
    public void SetTime(int hour, int minute)
    {
        SetProgressFromTime(hour, minute);
        NotifyTimeChanged(true);
    }

    /// <summary>
    /// Skips time forward by a precise number of minutes. 
    /// Gracefully calculates day wraps if the increment skips past midnight.
    /// </summary>
    /// <param name="minutes">The number of in-game minutes to advance.</param>
    public void AddMinutes(int minutes)
    {
        int oldCompletedDays = completedDays;

        int currentAbsoluteMinutes = completedDays * 1440 + TotalMinutes;
        int newAbsoluteMinutes = Mathf.Max(0, currentAbsoluteMinutes + minutes);

        completedDays = newAbsoluteMinutes / 1440;

        int minutesInCurrentDay = newAbsoluteMinutes % 1440;
        dayProgress = minutesInCurrentDay / 1440f;

        if (completedDays > oldCompletedDays)
        {
            for (int day = oldCompletedDays + 1; day <= completedDays; day++)
            {
                OnDayCompleted?.Invoke(day);
            }
        }

        NotifyTimeChanged(true);
    }

    /// <summary>
    /// Evaluates if the current in-game time falls within a given time range.
    /// Handles ranges spanning past midnight (e.g. 22:00 to 06:00).
    /// </summary>
    /// <param name="startHour">Starting hour.</param>
    /// <param name="startMinute">Starting minute.</param>
    /// <param name="endHour">Ending hour.</param>
    /// <param name="endMinute">Ending minute.</param>
    /// <returns>True if the current time is inside the given bounds, otherwise false.</returns>
    public bool IsTimeBetween(int startHour, int startMinute, int endHour, int endMinute)
    {
        int current = CurrentHour * 60 + CurrentMinute;

        int start = Mathf.Clamp(startHour, 0, 23) * 60
                  + Mathf.Clamp(startMinute, 0, 59);

        int end = Mathf.Clamp(endHour, 0, 23) * 60
                + Mathf.Clamp(endMinute, 0, 59);

        if (start <= end)
        {
            return current >= start && current <= end;
        }

        return current >= start || current <= end;
    }

    /// <summary>
    /// Bundles the internal state into a save data object.
    /// </summary>
    /// <returns>A new <c>TimeSaveData</c> representing the current runtime state.</returns>
    public TimeSaveData GetSaveData()
    {
        return new TimeSaveData
        {
            dayProgress = dayProgress,
            isPaused = isPaused,
            completedDays = completedDays
        };
    }

    /// <summary>
    /// Restores the manager's state from a provided save data object. 
    /// Ensures values remain safely bounded.
    /// </summary>
    /// <param name="data">The serialization data to load from.</param>
    public void LoadFromSaveData(TimeSaveData data)
    {
        if (data == null)
        {
            Debug.LogWarning("TimeManager: Tried to load null save data.");
            return;
        }

        dayProgress = Mathf.Repeat(data.dayProgress, 1f);
        isPaused = data.isPaused;
        completedDays = Mathf.Max(0, data.completedDays);

        NotifyTimeChanged(true);
    }

    /// <summary>
    /// Resets the time back to the default start state defined in settings.
    /// Resets completed days and clears tracking variables.
    /// </summary>
    public void ResetTime()
    {
        completedDays = 0;
        lastMinute = -1;
        lastHour = -1;
        ApplyStartSettings();
        NotifyTimeChanged(true);
    }

    /// <summary>
    /// Validates inspector values and restricts impossible states.
    /// </summary>
    private void OnValidate()
    {
        dayProgress = Mathf.Repeat(dayProgress, 1f);
        completedDays = Mathf.Max(0, completedDays);
    }
}


}