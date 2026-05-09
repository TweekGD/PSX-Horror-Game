using DG.Tweening;
using FMODUnity;
using UnityEngine;
using UnityEngine.EventSystems;

public class HighlightUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private GameObject targetObject;
    [SerializeField] private float targetScale = 1.1f;
    [SerializeField] private float animDuration = 0.5f;
    [SerializeField] private EventReference clickSound;
    [SerializeField] private EventReference highlightSound;

    private float startScale;
    private Tween tweenAnimation;

    private IAudioManager audioManager;
    private void Awake()
    {
        audioManager = ServiceLocator.Get<IAudioManager>();

        if (targetObject != null) { startScale = targetObject.transform.localScale.x; }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (audioManager != null && !clickSound.IsNull)
            audioManager.PlayOneShot(highlightSound, transform.position);

        if (targetObject == null) { return; }

        tweenAnimation = targetObject.transform.DOScale(targetScale, animDuration).SetEase(Ease.OutSine);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (targetObject == null) { return; }

        tweenAnimation = targetObject.transform.DOScale(startScale, animDuration).SetEase(Ease.OutSine);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (audioManager != null && !clickSound.IsNull)
            audioManager.PlayOneShot(clickSound, transform.position);
    }
    private void OnDestroy()
    {
        tweenAnimation?.Kill();
    }
    private void OnDisable()
    {
        tweenAnimation?.Kill();
    }
}