using UnityEngine;
using System;

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
    [Header("Time Period when Configuration is active")]
    public SpriteControllerManager.TimePeriod timePeriod;

    public enum Activation {ActivateAll, DeactivateAll, ActivateRandom, ActivateLow, ActivateMedium, ActivateHigh}
    [Header("How many active sprites (Activate All/ Deactivate All/ Activation based on activity level)")]
    public Activation activation;

    [Header("Random Sprite Setting (From All Sprites OR From Sprite Index Range)")]
    
    public bool fromAllSprites;
    public MinMaxRange SpriteIndexRange;
    
}