namespace AdventureGame.WeatherSystem
{
    public struct WeatherSnapshot
    {
        public WeatherType CurrentWeather { get; }
        public int LastChangedDay { get; }
        public int LastChangedHour { get; }
        public int LastChangedMinute { get; }
        public int LastRollSlot { get; }

        public WeatherSnapshot(
            WeatherType currentWeather,
            int lastChangedDay,
            int lastChangedHour,
            int lastChangedMinute,
            int lastRollSlot)
        {
            CurrentWeather = currentWeather;
            LastChangedDay = lastChangedDay;
            LastChangedHour = lastChangedHour;
            LastChangedMinute = lastChangedMinute;
            LastRollSlot = lastRollSlot;
        }
    }
}