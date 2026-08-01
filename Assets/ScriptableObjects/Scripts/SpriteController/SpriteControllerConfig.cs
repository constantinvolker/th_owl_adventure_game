using UnityEngine;
using System;
namespace AdventureGame.SpriteManagement
{
    /// <summary>
    /// Configuration settings defining how a SpriteControllerList behaves during a specific time period.
    /// </summary>
    [Serializable]
    public class SpriteControllerConfig
    {
        [Serializable]
        public class MinMaxRange
        {
            public int min;
            public int max;
        }
        // statt TimePeriod soll hier int startHour, int startMinute, int endHour, int endMinute eingegeben werden koennen
        [Header("Time Period when Configuration is active")]
        public SpriteControllerManager.TimePeriod timePeriod;

        public enum Activation {ActivateAll, DeactivateAll, ActivateRandom, ActivateLow, ActivateMedium, ActivateHigh}
        [Header("How many active sprites (Activate All/ Deactivate All/ Activation based on activity level)")]
        public Activation activation;

        [Header("Random Sprite Setting (From All Sprites OR From Sprite Index Range)")]
        public bool fromAllSprites;
        public MinMaxRange SpriteIndexRange;

    }
}
