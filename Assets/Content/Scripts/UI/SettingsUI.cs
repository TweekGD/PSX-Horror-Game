using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    [System.Serializable]
    private struct ArrowControl
    {
        public Button leftButton;
        public Button rightButton;
        public TextMeshProUGUI valueText;
    }

    [System.Serializable]
    private struct SliderControl
    {
        public Slider slider;
        public TextMeshProUGUI valueText;
    }

    [SerializeField] private TMP_Dropdown languageDropdown;
    [SerializeField] private SliderControl sensitivity;
    [SerializeField] private SliderControl masterVolume;
    [SerializeField] private SliderControl musicVolume;
    [SerializeField] private SliderControl sfxVolume;
    [SerializeField] private SliderControl uiVolume;
    [SerializeField] private TMP_Dropdown screenResolutionDropdown;
    [SerializeField] private TMP_Dropdown screenModeDropdown;
    [SerializeField] private SliderControl fpsLimit;
    [SerializeField] private ArrowControl vSync;
    [SerializeField] private Button resetButton;

    private SettingsManager settingsManager;
    private bool isInitializing;

    private void Start()
    {
        settingsManager = ServiceLocator.Get<ISettingsManager>() as SettingsManager;

        if (settingsManager == null)
        {
            Debug.LogError("SettingsManager is null.");
            return;
        }

        InitDropdowns();
        InitSliders();

        BindDropdowns();
        BindSliders();
        BindArrows();

        if (resetButton) resetButton.onClick.AddListener(OnReset);

        settingsManager.GetParameter<IndexedParameter>("Language").OnChanged += RefreshLanguage;
        settingsManager.GetParameter<SettingsParameter<float>>("Sensitivity").OnChanged += RefreshSensitivity;
        settingsManager.GetParameter<SettingsParameter<float>>("MasterVolume").OnChanged += RefreshMasterVolume;
        settingsManager.GetParameter<SettingsParameter<float>>("MusicVolume").OnChanged += RefreshMusicVolume;
        settingsManager.GetParameter<SettingsParameter<float>>("SFXVolume").OnChanged += RefreshSFXVolume;
        settingsManager.GetParameter<SettingsParameter<float>>("UIVolume").OnChanged += RefreshUIVolume;
        settingsManager.GetParameter<IndexedParameter>("ScreenResolution").OnChanged += RefreshScreenResolution;
        settingsManager.GetParameter<IndexedParameter>("ScreenMode").OnChanged += RefreshScreenMode;
        settingsManager.GetParameter<SettingsParameter<int>>("FpsStepIndex").OnChanged += RefreshFpsLimit;
        settingsManager.GetParameter<SettingsParameter<int>>("VSync").OnChanged += RefreshVSync;

        RefreshAll();
    }

    private void OnEnable()
    {
        if (settingsManager != null)
            RefreshAll();
    }

    private void OnDestroy()
    {
        if (settingsManager == null) return;

        settingsManager.GetParameter<IndexedParameter>("Language").OnChanged -= RefreshLanguage;
        settingsManager.GetParameter<SettingsParameter<float>>("Sensitivity").OnChanged -= RefreshSensitivity;
        settingsManager.GetParameter<SettingsParameter<float>>("MasterVolume").OnChanged -= RefreshMasterVolume;
        settingsManager.GetParameter<SettingsParameter<float>>("MusicVolume").OnChanged -= RefreshMusicVolume;
        settingsManager.GetParameter<SettingsParameter<float>>("SFXVolume").OnChanged -= RefreshSFXVolume;
        settingsManager.GetParameter<SettingsParameter<float>>("UIVolume").OnChanged -= RefreshUIVolume;
        settingsManager.GetParameter<IndexedParameter>("ScreenResolution").OnChanged -= RefreshScreenResolution;
        settingsManager.GetParameter<IndexedParameter>("ScreenMode").OnChanged -= RefreshScreenMode;
        settingsManager.GetParameter<SettingsParameter<int>>("FpsStepIndex").OnChanged -= RefreshFpsLimit;
        settingsManager.GetParameter<SettingsParameter<int>>("VSync").OnChanged -= RefreshVSync;
    }

    private void InitDropdowns()
    {
        if (languageDropdown)
            RebuildLanguageOptions();

        if (screenResolutionDropdown)
        {
            screenResolutionDropdown.ClearOptions();
            foreach (Resolution res in settingsManager.FilteredResolutions)
                screenResolutionDropdown.options.Add(new TMP_Dropdown.OptionData(res.width + "x" + res.height));
            screenResolutionDropdown.RefreshShownValue();
        }

        if (screenModeDropdown)
        {
            screenModeDropdown.ClearOptions();
            for (int i = 0; i < settingsManager.ScreenModeCount; i++)
                screenModeDropdown.options.Add(new TMP_Dropdown.OptionData(GetScreenModeLabel(i)));
            screenModeDropdown.RefreshShownValue();
        }
    }

    private void RebuildLanguageOptions()
    {
        if (!languageDropdown) return;
        languageDropdown.ClearOptions();
        string[] labels = { "English", "Russian", "Spain" };
        for (int i = 0; i < settingsManager.LanguageCount; i++)
            languageDropdown.options.Add(new TMP_Dropdown.OptionData(i < labels.Length ? labels[i] : i.ToString()));
        languageDropdown.RefreshShownValue();
    }

    private string GetScreenModeLabel(int index) => index switch
    {
        0 => "Fullscreen",
        1 => "Windowed",
        2 => "Borderless",
        _ => index.ToString()
    };

    private void InitSliders()
    {
        SetupSlider(sensitivity.slider, 0.05f, 1f, false);
        SetupSlider(masterVolume.slider, 0f, 2f, false);
        SetupSlider(musicVolume.slider, 0f, 2f, false);
        SetupSlider(sfxVolume.slider, 0f, 2f, false);
        SetupSlider(uiVolume.slider, 0f, 2f, false);
        SetupSlider(fpsLimit.slider, 0f, settingsManager.FpsStepsCount - 1, true);
    }

    private void SetupSlider(Slider slider, float min, float max, bool wholeNumbers)
    {
        if (!slider) return;
        slider.minValue = min;
        slider.maxValue = max;
        slider.wholeNumbers = wholeNumbers;
    }

    private void BindDropdowns()
    {
        if (languageDropdown)
            languageDropdown.onValueChanged.AddListener(OnLanguageDropdownChanged);

        if (screenResolutionDropdown)
            screenResolutionDropdown.onValueChanged.AddListener(OnScreenResolutionDropdownChanged);

        if (screenModeDropdown)
            screenModeDropdown.onValueChanged.AddListener(OnScreenModeDropdownChanged);
    }

    private void BindSliders()
    {
        if (sensitivity.slider)
            sensitivity.slider.onValueChanged.AddListener(OnSensitivitySliderChanged);

        if (masterVolume.slider)
            masterVolume.slider.onValueChanged.AddListener(OnMasterVolumeSliderChanged);

        if (musicVolume.slider)
            musicVolume.slider.onValueChanged.AddListener(OnMusicVolumeSliderChanged);

        if (sfxVolume.slider)
            sfxVolume.slider.onValueChanged.AddListener(OnSFXVolumeSliderChanged);

        if (uiVolume.slider)
            uiVolume.slider.onValueChanged.AddListener(OnUIVolumeSliderChanged);

        if (fpsLimit.slider)
            fpsLimit.slider.onValueChanged.AddListener(OnFpsLimitSliderChanged);
    }

    private void BindArrows()
    {
        BindArrow(vSync.leftButton, () => settingsManager.StepVSync(-1));
        BindArrow(vSync.rightButton, () => settingsManager.StepVSync(1));
    }

    private static void BindArrow(Button btn, System.Action action)
    {
        if (btn) btn.onClick.AddListener(() => action());
    }

    private void OnLanguageDropdownChanged(int index)
    {
        if (isInitializing) return;
        settingsManager.SetLanguageFromDropdown(index);
    }

    private void OnScreenResolutionDropdownChanged(int index)
    {
        if (isInitializing) return;
        settingsManager.SetScreenResolutionFromDropdown(index);
    }

    private void OnScreenModeDropdownChanged(int index)
    {
        if (isInitializing) return;
        settingsManager.SetScreenModeFromDropdown(index);
    }

    private void OnSensitivitySliderChanged(float value)
    {
        if (isInitializing) return;
        settingsManager.SetSensitivityFromSlider(value);
    }

    private void OnMasterVolumeSliderChanged(float value)
    {
        if (isInitializing) return;
        settingsManager.SetMasterVolumeFromSlider(value);
    }

    private void OnMusicVolumeSliderChanged(float value)
    {
        if (isInitializing) return;
        settingsManager.SetMusicVolumeFromSlider(value);
    }

    private void OnSFXVolumeSliderChanged(float value)
    {
        if (isInitializing) return;
        settingsManager.SetSFXVolumeFromSlider(value);
    }

    private void OnUIVolumeSliderChanged(float value)
    {
        if (isInitializing) return;
        settingsManager.SetUIVolumeFromSlider(value);
    }

    private void OnFpsLimitSliderChanged(float value)
    {
        if (isInitializing) return;
        settingsManager.SetFpsLimitFromSlider(value);
    }

    private void OnReset()
    {
        settingsManager.ResetToDefaults();
    }

    private void RefreshAll()
    {
        RefreshLanguage();
        RefreshSensitivity();
        RefreshMasterVolume();
        RefreshMusicVolume();
        RefreshSFXVolume();
        RefreshUIVolume();
        RefreshScreenResolution();
        RefreshScreenMode();
        RefreshFpsLimit();
        RefreshVSync();
    }

    private void RefreshDropdown(TMP_Dropdown dropdown, int index)
    {
        if (!dropdown) return;
        isInitializing = true;
        dropdown.value = index;
        dropdown.RefreshShownValue();
        isInitializing = false;
    }

    private void RefreshSlider(Slider slider, float value)
    {
        if (!slider) return;
        isInitializing = true;
        slider.value = value;
        isInitializing = false;
    }

    private static void SetText(TextMeshProUGUI label, string value)
    {
        if (label) label.text = value;
    }

    private static string ToPercent(float value) => Mathf.RoundToInt(value * 100f) + "%";

    private void RefreshLanguage()
    {
        RefreshDropdown(languageDropdown, settingsManager.LanguageIndex);
    }

    private void RefreshSensitivity()
    {
        RefreshSlider(sensitivity.slider, settingsManager.Sensitivity);
        SetText(sensitivity.valueText, settingsManager.Sensitivity.ToString("F2"));
    }

    private void RefreshMasterVolume()
    {
        RefreshSlider(masterVolume.slider, settingsManager.MasterVolume);
        SetText(masterVolume.valueText, ToPercent(settingsManager.MasterVolume));
    }

    private void RefreshMusicVolume()
    {
        RefreshSlider(musicVolume.slider, settingsManager.MusicVolume);
        SetText(musicVolume.valueText, ToPercent(settingsManager.MusicVolume));
    }

    private void RefreshSFXVolume()
    {
        RefreshSlider(sfxVolume.slider, settingsManager.SFXVolume);
        SetText(sfxVolume.valueText, ToPercent(settingsManager.SFXVolume));
    }

    private void RefreshUIVolume()
    {
        RefreshSlider(uiVolume.slider, settingsManager.UIVolume);
        SetText(uiVolume.valueText, ToPercent(settingsManager.UIVolume));
    }

    private void RefreshScreenResolution()
    {
        RefreshDropdown(screenResolutionDropdown, settingsManager.ScreenResolutionIndex);
    }

    private void RefreshScreenMode()
    {
        RefreshDropdown(screenModeDropdown, settingsManager.ScreenModeIndex);
    }

    private void RefreshFpsLimit()
    {
        RefreshSlider(fpsLimit.slider, settingsManager.FpsStepIndex);
        SetText(fpsLimit.valueText, ((int)settingsManager.FpsLimit).ToString());
    }

    private void RefreshVSync()
    {
        SetText(vSync.valueText, settingsManager.VSync == 1 ? "On" : "Off");
    }
}
