using TMPro;
using UnityEngine;
using UnityEngine.Events;
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

    private ISettingsManager settingsManager;

    private void Start()
    {
        settingsManager = ServiceLocator.Get<ISettingsManager>();

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
            var resParam = settingsManager.GetParameter<IndexedParameter>("ScreenResolution");
            screenResolutionDropdown.ClearOptions();
            for (int i = 0; i < resParam.Count; i++)
                screenResolutionDropdown.options.Add(new TMP_Dropdown.OptionData(resParam.Options[i]));
            screenResolutionDropdown.RefreshShownValue();
        }

        if (screenModeDropdown)
        {
            var modeParam = settingsManager.GetParameter<IndexedParameter>("ScreenMode");
            screenModeDropdown.ClearOptions();
            for (int i = 0; i < modeParam.Count; i++)
                screenModeDropdown.options.Add(new TMP_Dropdown.OptionData(modeParam.Options[i]));
            screenModeDropdown.RefreshShownValue();
        }
    }

    private void RebuildLanguageOptions()
    {
        if (!languageDropdown) return;
        var langParam = settingsManager.GetParameter<IndexedParameter>("Language");
        languageDropdown.ClearOptions();
        for (int i = 0; i < langParam.Count; i++)
            languageDropdown.options.Add(new TMP_Dropdown.OptionData(langParam.Options[i]));
        languageDropdown.RefreshShownValue();
    }

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
        settingsManager.GetParameter<IndexedParameter>("Language").Set(index);
    }

    private void OnScreenResolutionDropdownChanged(int index)
    {
        settingsManager.GetParameter<IndexedParameter>("ScreenResolution").Set(index);
    }

    private void OnScreenModeDropdownChanged(int index)
    {
        settingsManager.GetParameter<IndexedParameter>("ScreenMode").Set(index);
    }

    private void OnSensitivitySliderChanged(float value)
    {
        settingsManager.GetParameter<SettingsParameter<float>>("Sensitivity").Set(value);
    }

    private void OnMasterVolumeSliderChanged(float value)
    {
        settingsManager.GetParameter<SettingsParameter<float>>("MasterVolume").Set(value);
    }

    private void OnMusicVolumeSliderChanged(float value)
    {
        settingsManager.GetParameter<SettingsParameter<float>>("MusicVolume").Set(value);
    }

    private void OnSFXVolumeSliderChanged(float value)
    {
        settingsManager.GetParameter<SettingsParameter<float>>("SFXVolume").Set(value);
    }

    private void OnUIVolumeSliderChanged(float value)
    {
        settingsManager.GetParameter<SettingsParameter<float>>("UIVolume").Set(value);
    }

    private void OnFpsLimitSliderChanged(float value)
    {
        settingsManager.GetParameter<SettingsParameter<int>>("FpsStepIndex").Set(Mathf.RoundToInt(value));
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

    private void RefreshDropdown(TMP_Dropdown dropdown, UnityAction<int> callback, int index)
    {
        if (!dropdown) return;
        dropdown.onValueChanged.RemoveListener(callback);
        dropdown.value = index;
        dropdown.RefreshShownValue();
        dropdown.onValueChanged.AddListener(callback);
    }

    private static void SetSliderValueWithoutNotify(Slider slider, float value, UnityAction<float> callback)
    {
        if (!slider) return;
        slider.onValueChanged.RemoveListener(callback);
        slider.value = value;
        slider.onValueChanged.AddListener(callback);
    }

    private static void SetText(TextMeshProUGUI label, string value)
    {
        if (label) label.text = value;
    }

    private static string ToPercent(float value) => Mathf.RoundToInt(value * 100f) + "%";

    private void RefreshLanguage()
    {
        RefreshDropdown(languageDropdown, OnLanguageDropdownChanged, settingsManager.GetParameter<IndexedParameter>("Language").Index);
    }

    private void RefreshSensitivity()
    {
        float val = settingsManager.GetParameter<SettingsParameter<float>>("Sensitivity").Value;
        SetSliderValueWithoutNotify(sensitivity.slider, val, OnSensitivitySliderChanged);
        SetText(sensitivity.valueText, val.ToString("F2"));
    }

    private void RefreshMasterVolume()
    {
        float val = settingsManager.GetParameter<SettingsParameter<float>>("MasterVolume").Value;
        SetSliderValueWithoutNotify(masterVolume.slider, val, OnMasterVolumeSliderChanged);
        SetText(masterVolume.valueText, ToPercent(val));
    }

    private void RefreshMusicVolume()
    {
        float val = settingsManager.GetParameter<SettingsParameter<float>>("MusicVolume").Value;
        SetSliderValueWithoutNotify(musicVolume.slider, val, OnMusicVolumeSliderChanged);
        SetText(musicVolume.valueText, ToPercent(val));
    }

    private void RefreshSFXVolume()
    {
        float val = settingsManager.GetParameter<SettingsParameter<float>>("SFXVolume").Value;
        SetSliderValueWithoutNotify(sfxVolume.slider, val, OnSFXVolumeSliderChanged);
        SetText(sfxVolume.valueText, ToPercent(val));
    }

    private void RefreshUIVolume()
    {
        float val = settingsManager.GetParameter<SettingsParameter<float>>("UIVolume").Value;
        SetSliderValueWithoutNotify(uiVolume.slider, val, OnUIVolumeSliderChanged);
        SetText(uiVolume.valueText, ToPercent(val));
    }

    private void RefreshScreenResolution()
    {
        RefreshDropdown(screenResolutionDropdown, OnScreenResolutionDropdownChanged, settingsManager.GetParameter<IndexedParameter>("ScreenResolution").Index);
    }

    private void RefreshScreenMode()
    {
        RefreshDropdown(screenModeDropdown, OnScreenModeDropdownChanged, settingsManager.GetParameter<IndexedParameter>("ScreenMode").Index);
    }

    private void RefreshFpsLimit()
    {
        int stepIndex = settingsManager.FpsStepIndex;
        SetSliderValueWithoutNotify(fpsLimit.slider, stepIndex, OnFpsLimitSliderChanged);
        SetText(fpsLimit.valueText, settingsManager.FpsLimit.ToString());
    }

    private void RefreshVSync()
    {
        int val = settingsManager.GetParameter<SettingsParameter<int>>("VSync").Value;
        SetText(vSync.valueText, val == 1 ? "On" : "Off");
    }
}