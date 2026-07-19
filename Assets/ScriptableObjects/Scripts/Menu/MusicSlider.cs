using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MusicSlider : MonoBehaviour
{
    private Slider slider;

    void Awake()
    {
        slider = GetComponent<Slider>();
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.wholeNumbers = false;

        slider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    void OnEnable()
    {
        if (SettingsManager.Instance != null)
            slider.value = SettingsManager.Instance.GetMusicVolume();
        else
            slider.value = 1f;
    }

    void OnDestroy()
    {
        slider.onValueChanged.RemoveListener(OnSliderValueChanged);
    }

    private void OnSliderValueChanged(float value)
    {
        if (SettingsManager.Instance != null)
            SettingsManager.Instance.SetMusicVolume(value);
    }
}
