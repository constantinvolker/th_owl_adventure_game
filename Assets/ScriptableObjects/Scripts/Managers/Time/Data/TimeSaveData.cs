using System;

namespace AdventureGame.TimeSystem
{

    [Serializable]
    public class TimeSaveData
    {
        public float dayProgress;
        public bool isPaused;
        public int completedDays;
    }

}