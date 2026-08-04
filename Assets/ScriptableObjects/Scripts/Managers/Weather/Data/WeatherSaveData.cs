using System;

namespace AdventureGame.WeatherSystem
{
    [Serializable]
    public class WeatherSaveData
    {
        public WeatherType currentWeather;
        public int lastChangedDay;
        public int lastChangedHour;
        public int lastChangedMinute;
        public int lastRollSlot;
    }
}