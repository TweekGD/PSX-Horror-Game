using UnityEngine;
public class InteractivePanelUI : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private PlayerInteract playerInteract;
    [SerializeField] private RectTransform panel;
    [SerializeField] private Canvas canvas;

    private void OnEnable()
    {
        playerInteract.OnInteracted += OnInteractedChanged;
    }

    private void OnDisable()
    {
        playerInteract.OnInteracted -= OnInteractedChanged;
    }

    private void OnInteractedChanged(bool isActive)
    {
        panel.gameObject.SetActive(isActive);
    }

    private void LateUpdate()
    {
        if (playerInteract.LastTarget == null) return;

        MonoBehaviour targetBehaviour = playerInteract.LastTarget as MonoBehaviour;

        if (targetBehaviour == null) return;

        Vector3 screenPoint = playerCamera.WorldToScreenPoint(targetBehaviour.transform.position);

        if (screenPoint.z < 0f) return;

        Camera uiCamera = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : playerCamera;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.GetComponent<RectTransform>(),
            new Vector2(screenPoint.x, screenPoint.y),
            uiCamera,
            out Vector2 localPoint
        );

        panel.anchoredPosition = localPoint;
    }
}