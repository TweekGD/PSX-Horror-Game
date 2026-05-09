using DG.Tweening;
using System;
using UnityEngine;

public class SmoothAnimationUI : MonoBehaviour
{
    private RectTransform uiElement;
    private Tween elementTween;

    private void Awake()
    {
        uiElement = GetComponent<RectTransform>();
    }


    public void StartAnimationElement(Vector2 targetPos, float duration,
                                        float delay = 0f,
                                        bool deactivateOnComplete = false,
                                        Action onComplete = null)
    {
        elementTween?.Kill();

        elementTween = uiElement.DOAnchorPos(targetPos, duration)
            .SetEase(Ease.OutSine)
            .SetDelay(delay)
            .OnComplete(() =>
            {
                if (deactivateOnComplete)
                    gameObject.SetActive(false);

                onComplete?.Invoke();
            });
    }


    private void OnDestroy() { elementTween?.Kill(); }
    private void OnDisable() { elementTween?.Kill(); }
}