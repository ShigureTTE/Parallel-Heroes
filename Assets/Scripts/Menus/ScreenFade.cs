using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup), typeof(Tweener))]
public class ScreenFade : MonoBehaviourSingleton<ScreenFade> {

    [SerializeField] private float awakeWaitTime;

    private CanvasGroup canvasGroup;
    private Tweener tweener;

    public float WaitTime { get; private set; }

    private void Awake() {
        canvasGroup = GetComponent<CanvasGroup>();
        tweener = GetComponent<Tweener>();

        canvasGroup.alpha = 1;
        WaitTime = tweener.tweenObjects[0].duration + tweener.tweenObjects[0].durationReverse;
        StartCoroutine(WaitFadeOnAwake());
    }

    private IEnumerator WaitFadeOnAwake() {
        yield return new WaitForSecondsRealtime(awakeWaitTime);
        FadeIn();
    }

    public void FadeIn() {
        StartCoroutine(DoFade(true));
    }

    public void FadeOut() {
        StartCoroutine(DoFade(false));
    }

    public void FadeIn(Action onComplete) {
        StartCoroutine(DoFade(true, onComplete));

    }

    public void FadeOut(Action onComplete) {
        StartCoroutine(DoFade(false, onComplete));
    }

    private IEnumerator DoFade(bool fadeIn, Action onComplete = null) {
        if (fadeIn) {
            tweener.PlayTweenReversed();
        }
        else {
            tweener.PlayTween();
        }

        yield return new WaitForSecondsRealtime(WaitTime);
        onComplete?.Invoke();
    }
}
