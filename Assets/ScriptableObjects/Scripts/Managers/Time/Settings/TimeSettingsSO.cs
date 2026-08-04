using UnityEngine;

namespace AdventureGame.TimeSystem
{


[CreateAssetMenu(fileName = "TimeSettingsSO", menuName = "Adventure/Time/Time Settings")]
public class TimeSettingsSO : ScriptableObject
{
    [Header("Day Cycle")]
    [Min(1f)]
    public float realSecondsPerIngameDay = 1200f;

    [Header("Start Time")]
    [Range(0, 23)]
    public int startHour = 8;

    [Range(0, 59)]
    public int startMinute = 0;

    [Header("Behaviour")]
    public bool startPaused = false;

    public bool useUnscaledTime = false;
}


}