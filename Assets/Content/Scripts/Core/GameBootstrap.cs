using UnityEngine;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-99999)]
public class GameBootstrapper : MonoBehaviour
{
    [Header("Service Locator")]
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private SettingsManager settingsManager;
    [SerializeField] private InputManager inputManager;
    [SerializeField] private InputState inputState;
    [Space]
    [SerializeField] private string startScene = "Game Scene";

    private static GameBootstrapper instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        ValidateDependencies();
        RegisterServices();
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        string activeScene = SceneManager.GetActiveScene().name;

        if (!string.IsNullOrEmpty(startScene) && activeScene != startScene)
        {
            SceneManager.LoadScene(startScene);
        }
    }

    private void ValidateDependencies()
    {
        if (audioManager == null) Debug.LogError($"[{nameof(GameBootstrapper)}] AudioManager is not assigned.", this);
        if (settingsManager == null) Debug.LogError($"[{nameof(GameBootstrapper)}] SettingsManager is not assigned.", this);
        if (inputManager == null) Debug.LogError($"[{nameof(GameBootstrapper)}] InputManager is not assigned.", this);
        if (inputState == null) Debug.LogError($"[{nameof(GameBootstrapper)}] InputState is not assigned.", this);
    }

    private void RegisterServices()
    {
        ServiceLocator.Register<ISettingsManager>(settingsManager);
        ServiceLocator.Register<IAudioManager>(audioManager);
        ServiceLocator.Register<IInputManager>(inputManager);
        ServiceLocator.Register<IInputState>(inputState);
    }

    private void OnDestroy()
    {
        if (instance != this) return;

        instance = null;
        ServiceLocator.Clear();
    }
}
