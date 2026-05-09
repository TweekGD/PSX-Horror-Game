using DG.Tweening;
using System;
using UnityEngine;

public class FadeScreenEffect : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;

    private Tween animationTween;

    public void StartFadeEffect(float targetValue, float duration = 1f, Action onComplete = null)
    {
        float startAlpha = 1f - targetValue;

        animationTween = canvasGroup.DOFade(targetValue, duration).From(startAlpha).SetEase(Ease.OutSine).OnComplete(() => onComplete?.Invoke());
    }
    private void OnDisable()
    {
        animationTween?.Kill();
    }
    private void OnDestroy()
    {
        animationTween?.Kill();
    }
}