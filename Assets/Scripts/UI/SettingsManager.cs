using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering.Universal;
using TMPro;

public class SettingsManager : MonoBehaviour
{
    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer audioMixer;

    [Header("Volume Settings")]
    [SerializeField] private int volumeStep = 10;

    [Header("Volume Text")]
    [SerializeField] private TMP_Text masterVolumeText;
    [SerializeField] private TMP_Text sfxVolumeText;
    [SerializeField] private TMP_Text ambientVolumeText;

    [Header("State Button Text")]
    [SerializeField] private TMP_Text fullscreenButtonText;
    [SerializeField] private TMP_Text screenShakeButtonText;
    [SerializeField] private TMP_Text crtButtonText;

    [Header("CRT")]
    [SerializeField] private ScriptableRendererData rendererData;

    [SerializeField]
    private string crtFeatureName =
        "Full Screen Pass Renderer Feature";


    private int masterVolume;
    private int sfxVolume;
    private int ambientVolume;

    private bool fullscreen;
    private bool screenShake;
    private bool crtEnabled;


    private void Start()
    {
        LoadSettings();

        ApplyAllSettings();

        UpdateAllTexts();
    }


    // MASTER VOLUME

    public void IncreaseMasterVolume()
    {
        masterVolume =
            Mathf.Clamp(
                masterVolume + volumeStep,
                0,
                100
            );

        ApplyMasterVolume();
    }


    public void DecreaseMasterVolume()
    {
        masterVolume =
            Mathf.Clamp(
                masterVolume - volumeStep,
                0,
                100
            );

        ApplyMasterVolume();
    }


    private void ApplyMasterVolume()
    {
        SetMixerVolume(
            "MasterVolume",
            masterVolume
        );

        PlayerPrefs.SetInt(
            "MasterVolume",
            masterVolume
        );

        UpdateMasterVolumeText();

        PlayerPrefs.Save();
    }


    // SFX VOLUME

    public void IncreaseSFXVolume()
    {
        sfxVolume =
            Mathf.Clamp(
                sfxVolume + volumeStep,
                0,
                100
            );

        ApplySFXVolume();
    }


    public void DecreaseSFXVolume()
    {
        sfxVolume =
            Mathf.Clamp(
                sfxVolume - volumeStep,
                0,
                100
            );

        ApplySFXVolume();
    }


    private void ApplySFXVolume()
    {
        SetMixerVolume(
            "SFXVolume",
            sfxVolume
        );

        PlayerPrefs.SetInt(
            "SFXVolume",
            sfxVolume
        );

        UpdateSFXVolumeText();

        PlayerPrefs.Save();
    }


    // AMBIENT VOLUME

    public void IncreaseAmbientVolume()
    {
        ambientVolume =
            Mathf.Clamp(
                ambientVolume + volumeStep,
                0,
                100
            );

        ApplyAmbientVolume();
    }


    public void DecreaseAmbientVolume()
    {
        ambientVolume =
            Mathf.Clamp(
                ambientVolume - volumeStep,
                0,
                100
            );

        ApplyAmbientVolume();
    }


    private void ApplyAmbientVolume()
    {
        SetMixerVolume(
            "AmbientVolume",
            ambientVolume
        );

        PlayerPrefs.SetInt(
            "AmbientVolume",
            ambientVolume
        );

        UpdateAmbientVolumeText();

        PlayerPrefs.Save();
    }


    // FULLSCREEN

    public void ToggleFullscreen()
    {
        fullscreen =
            !fullscreen;

        Screen.fullScreen =
            fullscreen;

        PlayerPrefs.SetInt(
            "Fullscreen",
            fullscreen ? 1 : 0
        );

        UpdateFullscreenText();

        PlayerPrefs.Save();
    }


    // SCREEN SHAKE

    public void ToggleScreenShake()
    {
        screenShake =
            !screenShake;

        PlayerPrefs.SetInt(
            "ScreenShake",
            screenShake ? 1 : 0
        );

        UpdateScreenShakeText();

        PlayerPrefs.Save();
    }


    // CRT

    public void ToggleCRT()
    {
        crtEnabled =
            !crtEnabled;

        ApplyCRT();

        PlayerPrefs.SetInt(
            "CRT",
            crtEnabled ? 1 : 0
        );

        UpdateCRTText();

        PlayerPrefs.Save();
    }


    private void ApplyCRT()
    {
        if (rendererData == null)
        {
            return;
        }


        foreach (
            ScriptableRendererFeature feature
            in rendererData.rendererFeatures
        )
        {
            if (
                feature != null &&
                feature.name == crtFeatureName
            )
            {
                feature.SetActive(
                    crtEnabled
                );

                return;
            }
        }


        Debug.LogWarning(
            "CRT Renderer Feature nije pronađen: "
            + crtFeatureName
        );
    }


    // AUDIO MIXER

    private void SetMixerVolume(
        string parameter,
        int volume)
    {
        if (audioMixer == null)
        {
            return;
        }


        if (volume <= 0)
        {
            audioMixer.SetFloat(
                parameter,
                -80f
            );

            return;
        }


        float normalizedVolume =
            volume / 100f;


        float decibels =
            Mathf.Log10(
                normalizedVolume
            ) * 20f;


        audioMixer.SetFloat(
            parameter,
            decibels
        );
    }


    // LOAD SAVED SETTINGS

    private void LoadSettings()
    {
        masterVolume =
            PlayerPrefs.GetInt(
                "MasterVolume",
                100
            );


        sfxVolume =
            PlayerPrefs.GetInt(
                "SFXVolume",
                100
            );


        ambientVolume =
            PlayerPrefs.GetInt(
                "AmbientVolume",
                60
            );


        fullscreen =
            PlayerPrefs.GetInt(
                "Fullscreen",
                1
            ) == 1;


        screenShake =
            PlayerPrefs.GetInt(
                "ScreenShake",
                1
            ) == 1;


        crtEnabled =
            PlayerPrefs.GetInt(
                "CRT",
                1
            ) == 1;
    }


    // APPLY SAVED SETTINGS

    private void ApplyAllSettings()
    {
        SetMixerVolume(
            "MasterVolume",
            masterVolume
        );


        SetMixerVolume(
            "SFXVolume",
            sfxVolume
        );


        SetMixerVolume(
            "AmbientVolume",
            ambientVolume
        );


        Screen.fullScreen =
            fullscreen;


        ApplyCRT();
    }


    // UPDATE UI

    private void UpdateAllTexts()
    {
        UpdateMasterVolumeText();

        UpdateSFXVolumeText();

        UpdateAmbientVolumeText();

        UpdateFullscreenText();

        UpdateScreenShakeText();

        UpdateCRTText();
    }


    private void UpdateMasterVolumeText()
    {
        masterVolumeText.text =
            masterVolume + "%";
    }


    private void UpdateSFXVolumeText()
    {
        sfxVolumeText.text =
            sfxVolume + "%";
    }


    private void UpdateAmbientVolumeText()
    {
        ambientVolumeText.text =
            ambientVolume + "%";
    }


    private void UpdateFullscreenText()
    {
        fullscreenButtonText.text =
            fullscreen
                ? "[ ON ]"
                : "[ OFF ]";
    }


    private void UpdateScreenShakeText()
    {
        screenShakeButtonText.text =
            screenShake
                ? "[ ON ]"
                : "[ OFF ]";
    }


    private void UpdateCRTText()
    {
        crtButtonText.text =
            crtEnabled
                ? "[ ON ]"
                : "[ OFF ]";
    }
}