using UnityEngine;

public class PlayerCameraRotation : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform bodyTransform;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float maxAngleY = 90f;
    [SerializeField] private float smoothTime = 0.05f;
    public static Camera PlayerCamera { get; private set; }

    private Vector2 cameraRotation;
    private Vector2 currentInput;
    private Vector2 smoothedInput;
    private Vector2 inputVelocity;

    private IInputManager inputManager;
    private ISettingsManager settingsManager;
    private IInputState inputState;

    private void Awake()
    {
        inputManager = ServiceLocator.Get<IInputManager>();
        settingsManager = ServiceLocator.Get<ISettingsManager>();
        inputState = ServiceLocator.Get<IInputState>();

        inputState?.AddLock(InputState.LockType.Cursor, "InitPlayer");
    }

    private void Update()
    {
        CameraRotation();
    }

    private void CameraRotation()
    {
        if (inputManager == null) { return; }

        float sensitivityValue = settingsManager != null ? settingsManager.GetParametersValue<float>("Sensitivity") : 1f;

        if (inputState != null)
        {
            Cursor.lockState = inputState.GetLockState(InputState.LockType.Cursor) ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !inputState.GetLockState(InputState.LockType.Cursor);
        }

        if (!inputState.GetLockState(InputState.LockType.Camera))
        {
            currentInput = inputManager.GetInput<Vector2>("LookInput") * sensitivityValue;

            smoothedInput.x = Mathf.SmoothDamp(smoothedInput.x, currentInput.x, ref inputVelocity.x, smoothTime);
            smoothedInput.y = Mathf.SmoothDamp(smoothedInput.y, currentInput.y, ref inputVelocity.y, smoothTime);

            cameraRotation.x += smoothedInput.x;
            cameraRotation.y += smoothedInput.y;

            cameraRotation.y = Mathf.Clamp(cameraRotation.y, -maxAngleY, maxAngleY);
        }

        cameraTransform.localRotation = Quaternion.Euler(-cameraRotation.y, 0f, 0f);
        bodyTransform.rotation = Quaternion.Euler(0f, cameraRotation.x, 0f);
    }
}
