using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Controls multiple SpriteControllerLists and applies configurations based on the current time period.
/// </summary>
public class SpriteControllerManager : MonoBehaviour
{
    public enum TimePeriod {Undefined, Night, Day}
    [Header("Temporary Time Period")]
    public TimePeriod currentTimePeriod;
    
    [Header("Sprite Controller Lists")]
    public SpriteControllerList[] spriteControllerLists;

    /// <summary>
    /// Initializes the manager and applies all configurations matching the current time period.
    /// </summary>
    void Start()
    {
        Init();
    }

    private void Init()
    {
        // check active configurations for gameState
        List<SpriteControllerConfig> configurations = new();
        foreach(var list in spriteControllerLists)
        {
            foreach(var config in list.configurations)
            {
                if (config.timePeriod != TimePeriod.Undefined && currentTimePeriod != TimePeriod.Undefined)
                    if (config.timePeriod != currentTimePeriod)
                        continue;
                list.ApplyConfiguration(config);
            }
        }
    }
}
