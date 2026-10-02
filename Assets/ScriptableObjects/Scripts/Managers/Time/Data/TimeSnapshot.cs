namespace AdventureGame.TimeSystem
{
    public readonly struct TimeSnapshot
    {
        public readonly float DayProgress;
        public readonly int CompletedDays;
        public readonly int Hour;
        public readonly int Minute;
        public readonly string FormattedTime;
        public readonly bool IsPaused;

        public TimeSnapshot(
            float dayProgress,
            int completedDays,
            int hour,
            int minute,
            string formattedTime,
            bool isPaused)
        {
            DayProgress = dayProgress;
            CompletedDays = completedDays;
            Hour = hour;
            Minute = minute;
            FormattedTime = formattedTime;
            IsPaused = isPaused;
        }
    }
}