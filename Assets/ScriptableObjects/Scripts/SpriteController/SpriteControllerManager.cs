using UnityEngine;
using System.Linq;
namespace AdventureGame.SpriteManagement
{
    /// <summary>
    /// Controls multiple SpriteControllerLists and applies configurations based on the current time period.
    /// </summary>
    public class SpriteControllerManager : MonoBehaviour
    {
        // statt TimePeriod soll TimeManager.Instance.CurrentHour, TimeManager.Instance.CurrentMinute genutzt werden
        public enum TimePeriod {Undefined, Night, Day}
        [Header("Temporary Time Period")]
        public TimePeriod currentTimePeriod;

        [Header("Sprite Controller Lists")]
        public SpriteControllerList[] spriteControllerLists;
        private SpriteControllerList[] updateListOnTimeChange;

        /// <summary>
        /// Initializes the manager and applies all configurations matching the current time period.
        /// </summary>
        void Awake()
        {
            ApplyActiveConfigurations(spriteControllerLists);
            updateListOnTimeChange = spriteControllerLists?
                .Where(list => list.ApplyOnStartOnly == false).ToArray();
        }
        void Start()
        {
            updateListOnTimeChange = spriteControllerLists?
                .Where(list => list.ApplyOnStartOnly == false).ToArray();

            // event abo
            // TimeManager.Instance.OnMinuteChanged += UpdateApply;
        }
        /// <summary>
        /// apply configurations for current time from updateListOnTimeChange where ApplyOnStartOnly is false.
        /// </summary>
        private void UpdateApply(int hour, int minute)
        {
            ApplyActiveConfigurations(updateListOnTimeChange);
        }
        /// <summary>
        /// apply configurations for current time from a given spriteControllerLists
        /// </summary>
        private void ApplyActiveConfigurations(SpriteControllerList[] spriteControllerLists)
        {
            foreach(var list in spriteControllerLists)
            {
                foreach(var config in list.configurations)
                {
                    // time is not in time range of config
                    if(!TimeInConfigTimeRange(config))
                        continue;
                    
                    // config is already active
                    if (list.ActiveConfig == config)
                        break;
                    
                    // apply the config 
                    list.ApplyConfiguration(config);
                }
            }
        }
        /// <summary>
        /// Returns whether the current time is within the defined time range of the configuration.
        /// </summary>
        private bool TimeInConfigTimeRange(SpriteControllerConfig config)
        {
            // statt TimePeriod verleichen, soll hier public bool TimeManager.IsTimeBetween(int startHour, int startMinute, int endHour, int endMinute) verwendet werden

            if (config.timePeriod == TimePeriod.Undefined || currentTimePeriod == TimePeriod.Undefined)
                return true;
            return config.timePeriod == currentTimePeriod;
        }
    }
}
