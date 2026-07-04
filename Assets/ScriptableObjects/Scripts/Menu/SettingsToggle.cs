using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SettingsToggle : MonoBehaviour
{
    public enum ToggleType
    {
        Mute,
        Fullscreen
    }

    [SerializeField] private ToggleType toggleType;

    private Toggle toggle;

    void Awake()
    {
        toggle = GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(OnToggleValueChanged);
    }

    void Start()
    {
        RefreshToggle();
    }

    void OnEnable()
    {
        RefreshToggle();
    }

    void OnDestroy()
    {
       toggle.onValueChanged.RemoveListener(OnToggleValueChanged); 
    }

    private void RefreshToggle()
    {
        if (SettingsManager.Instance == null || toggle == null) 
            return;

        switch (toggleType)
        {
            case ToggleType.Mute:
                toggle.SetIsOnWithoutNotify(SettingsManager.Instance.IsMuted());
                break;
            
            case ToggleType.Fullscreen:
                toggle.SetIsOnWithoutNotify(SettingsManager.Instance.IsFullscreen());
                break;
        } 
    }

    private void OnToggleValueChanged(bool value)
    {
        if (SettingsManager.Instance == null) 
            return;

        switch (toggleType)
        {
            case ToggleType.Mute:
                SettingsManager.Instance.SetMuted(value);
                break;
            
            case ToggleType.Fullscreen:
                SettingsManager.Instance.SetFullscreen(value);
                break;
        }
    }
}
