using UnityEngine;

[DefaultExecutionOrder(-99999)]
public class GameBootstrapper : MonoBehaviour
{
    [Header("Service Locator")]
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private SettingsManager settingsManager;
    [SerializeField] private InputManager inputManager;
    [SerializeField] private InputState inputState;

    private void Awake()
    {
        ServiceLocator.Register<IAudioManager>(audioManager);
        ServiceLocator.Register<ISettingsManager>(settingsManager);
        ServiceLocator.Register<IInputManager>(inputManager);
        ServiceLocator.Register<IInputState>(inputState);

        DontDestroyOnLoad(gameObject);
    }

    private void OnDestroy()
    {
        ServiceLocator.Clear();
    }
}