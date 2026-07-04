using UnityEngine;
using UnityEngine.Audio;


public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; }

    [Header("Audio")]
    [SerializeField] private AudioMixer mainMixer;

    private const string MasterVolumeParam = "MasterVolume";
    private const string MasterVolumePrefKey = "MasterVolume";
    private const string MusicVolumeParam = "MusicVolume";
    private const string MusicVolumePrefKey = "MusicVolume";
    private const string SFXVolumeParam = "SFXVolume";
    private const string SFXVolumePrefKey = "SFXVolume";
    private const string MutePrefKey = "IsMuted";
    private const string FullscreenPrefKey = "IsFullscreen";

    private bool isMuted = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        LoadSettings();        
    }


    public void SetMasterVolume(float value)
    {
        SetMixerVolume(MasterVolumeParam, value);
        PlayerPrefs.SetFloat(MasterVolumePrefKey, value);
        PlayerPrefs.Save();
    }

    public float GetMasterVolume()
    {
        return GetSavedVolume(MasterVolumePrefKey);
    }

    public void SetMusicVolume(float value)
    {
        SetMixerVolume(MusicVolumeParam, value);
        PlayerPrefs.SetFloat(MusicVolumePrefKey, value);
        PlayerPrefs.Save();
    }

    public float GetMusicVolume()
    {
        return GetSavedVolume(MusicVolumePrefKey);
    }

    public void SetSFXVolume(float value)
    {
        SetMixerVolume(SFXVolumeParam, value);
        PlayerPrefs.SetFloat(SFXVolumePrefKey, value);
        PlayerPrefs.Save();
    }

    public float GetSFXVolume()
    {
        return GetSavedVolume(SFXVolumePrefKey);
    }

    public void SetMuted(bool muted)
    {
        isMuted = muted;

        if (muted)
        {
            mainMixer.SetFloat(MasterVolumeParam, -80f);
        }
        else
        {
            SetMasterVolume(GetMasterVolume());
        }

        PlayerPrefs.SetInt(MutePrefKey, muted ? 1:0);
        PlayerPrefs.Save();
    }

    public bool IsMuted()
    {
        return PlayerPrefs.GetInt(MutePrefKey, 0) == 1;
    }

    public void SetFullscreen(bool fullscreen)
    {
        Screen.fullScreen = fullscreen;
        PlayerPrefs.SetInt(FullscreenPrefKey, fullscreen ? 1: 0);
        PlayerPrefs.Save();
    }

    public bool IsFullscreen()
    {
        return PlayerPrefs.GetInt(FullscreenPrefKey, 1) == 1;
    }

    public void LoadSettings()
    {
        SetMasterVolume(GetMasterVolume());
        SetMusicVolume(GetMusicVolume());
        SetSFXVolume(GetSFXVolume());

        bool muted = IsMuted();
        isMuted = muted;
        if (muted)
        {
            mainMixer.SetFloat(MasterVolumeParam, -80f);
        }

         Screen.fullScreen = IsFullscreen();
    }

    private void SetMixerVolume(string parameterName, float value)
    {
        //Avoid log10(0)
        float clampedValue = Mathf.Clamp(value, 0.0001f, 1f);
        float dB = Mathf.Log10(clampedValue)*20f;
        mainMixer.SetFloat(parameterName, dB);
    }

    private float GetSavedVolume(string key)
    {
        float saved = PlayerPrefs.GetFloat(key, 1f);
        if (saved <= 0f)
            saved = 1f;
        return saved;
    }
}
