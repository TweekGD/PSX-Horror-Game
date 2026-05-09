using FMOD.Studio;
using FMODUnity;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour, IAudioManager
{
    [SerializeField] private float masterVolume = 1f;
    [SerializeField] private float musicVolume = 1f;
    [SerializeField] private float SFXVolume = 1f;
    [SerializeField] private float UIVolume = 1f;

    private Bus masterBus;
    private Bus musicBus;
    private Bus SFXBus;
    private Bus UIBus;

    private List<EventInstance> eventInstances;
    private List<StudioEventEmitter> eventEmitters;

    public EventInstance ambienceEventInstance;
    public EventInstance musicEventInstance;

    private ISettingsManager settingsManager;
    private void Awake()
    {
        eventInstances = new List<EventInstance>();
        eventEmitters = new List<StudioEventEmitter>();

        masterBus = RuntimeManager.GetBus("bus:/");
        SFXBus = RuntimeManager.GetBus("bus:/SFX");
        musicBus = RuntimeManager.GetBus("bus:/Music");
        UIBus = RuntimeManager.GetBus("bus:/UI");

        settingsManager = ServiceLocator.Get<ISettingsManager>();

        if (settingsManager != null) { settingsManager.OnParametersChanged += ChangeAllVolumeValue; }
    }

    private void OnDisable()
    {
        if (settingsManager != null) { settingsManager.OnParametersChanged -= ChangeAllVolumeValue; }
    }

    private void Start()
    {
        ChangeAllVolumeValue();
    }

    private void ChangeAllVolumeValue()
    {
        GetAllSavedVolumes();
        UpdateAllVolumeValue();
    }

    private void GetAllSavedVolumes()
    {
        if (settingsManager == null) { return; }

        masterVolume = settingsManager.GetParametersValue<float>("MasterVolume");
        musicVolume = settingsManager.GetParametersValue<float>("MusicVolume");
        SFXVolume = settingsManager.GetParametersValue<float>("SFXVolume");
        UIVolume = settingsManager.GetParametersValue<float>("UIVolume");
    }

    private void UpdateAllVolumeValue()
    {
        masterBus.setVolume(masterVolume);
        musicBus.setVolume(musicVolume);
        SFXBus.setVolume(SFXVolume);
        UIBus.setVolume(UIVolume);
    }

    public void InitializeAmbience(EventReference ambienceEventReference)
    {
        ambienceEventInstance = CreateInstance(ambienceEventReference);
        ambienceEventInstance.start();
    }

    public void InitializeMusic(EventReference musicEventReference)
    {
        musicEventInstance = CreateInstance(musicEventReference);
        musicEventInstance.start();
    }

    public void SetAmbienceParameter(string parameterName, float parameterValue, bool ignoreSpeed = false)
    {
        ambienceEventInstance.setParameterByName(parameterName, parameterValue, ignoreSpeed);
    }

    public void PlayOneShot(EventReference sound, Vector3 worldPos)
    {
        RuntimeManager.PlayOneShot(sound, worldPos);
    }

    public EventInstance CreateInstance(EventReference eventReference)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
        eventInstances.Add(eventInstance);
        return eventInstance;
    }

    public StudioEventEmitter InitializeEventEmitter(EventReference eventReference, GameObject emitterGameObject)
    {
        StudioEventEmitter emitter = emitterGameObject.GetComponent<StudioEventEmitter>();
        emitter.EventReference = eventReference;
        eventEmitters.Add(emitter);
        return emitter;
    }

    public void CleanUp()
    {
        foreach (EventInstance eventInstance in eventInstances)
        {
            eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            eventInstance.release();
        }

        foreach (StudioEventEmitter emitter in eventEmitters)
        {
            emitter.Stop();
        }
    }

    private void OnDestroy()
    {
        CleanUp();
    }
}