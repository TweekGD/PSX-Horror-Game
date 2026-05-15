using FMODUnity;
using System.Collections;
using UnityEngine;

public class DoorSystem : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform rootDoor;
    [SerializeField] private Transform soundPos;

    [SerializeField] private Vector3 startRot;
    [SerializeField] private Vector3 endRot;

    [SerializeField] private float rotSpeed = 1f;

    [SerializeField] private EventReference doorOpenSound;
    [SerializeField] private EventReference doorCloseSound;

    [SerializeField] private bool startOpened;

    private bool _isOpen;

    private Coroutine rotateDoorCoroutine;
    public bool IsOpen => _isOpen;

    private IInputManager inputManager;
    private IAudioManager audioManager;

    private void Awake()
    {
        inputManager = ServiceLocator.Get<IInputManager>();
        audioManager = ServiceLocator.Get<IAudioManager>();
    }

    private void Start()
    {
        _isOpen = startOpened;

        rootDoor.localRotation = startOpened ? Quaternion.Euler(endRot) : Quaternion.Euler(startRot);
    }

    public void Interact(GameObject player)
    {
        if (inputManager != null)
        {
            if (inputManager.GetInput<bool>("InteractInput"))
            {
                ToggleDoor();
            }
        }
    }

    private void ToggleDoor()
    {
        if (rotateDoorCoroutine != null) return;

        _isOpen = !_isOpen;

        if (rotateDoorCoroutine != null)
        {
            StopCoroutine(rotateDoorCoroutine);
            rotateDoorCoroutine = null;
        }

        Vector3 targetRot = _isOpen ? endRot : startRot;
        rotateDoorCoroutine = StartCoroutine(RotateDoor(targetRot, _isOpen));
    }

    private IEnumerator RotateDoor(Vector3 targetRotation, bool isOpen)
    {
        if (isOpen && !doorOpenSound.IsNull)
        {
            audioManager.PlayOneShot(doorOpenSound, rootDoor.position);
        }

        Quaternion startRotation = rootDoor.localRotation;
        Quaternion targetRotQuat = Quaternion.Euler(targetRotation);

        float progress = 0f;

        while (progress < 1f)
        {
            progress = Mathf.Clamp01(progress + Time.deltaTime * rotSpeed);
            rootDoor.localRotation = Quaternion.Lerp(startRotation, targetRotQuat, progress);
            yield return null;
        }

        rootDoor.localRotation = targetRotQuat;

        if (!isOpen && !doorCloseSound.IsNull)
        {
            Vector3 playPos = soundPos != null ? soundPos.position : rootDoor.position;
            audioManager.PlayOneShot(doorCloseSound, playPos);
        }

        rotateDoorCoroutine = null;
    }

    public void OnInteractedStart(GameObject player = null) { }
    public void OnInteractedEnd(GameObject player = null) { }
}