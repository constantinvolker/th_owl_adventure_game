using System;
using UnityEngine;

namespace AdventureGame.WeatherSystem
{
    [Serializable]
    public class WeatherChance
    {
        public WeatherType weatherType;

        [Min(0)]
        public int weight = 1;
    }
}