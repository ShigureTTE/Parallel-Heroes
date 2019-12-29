using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup), typeof(Tweener))]
public class ScreenFade : MonoBehaviourSingleton<ScreenFade> {

    [SerializeField] private float awakeWaitTime;

    private CanvasGroup canvasGroup;
    private Tweener tweener;

    private void Awake() {
        canvasGroup = GetComponent<CanvasGroup>();
        tweener = GetComponent<Tweener>();

        canvasGroup.alpha = 1;
        StartCoroutine(WaitFadeOnAwake());
    }

    private IEnumerator WaitFadeOnAwake() {
        yield return new WaitForSecondsRealtime(awakeWaitTime);
        FadeIn();
    }

    public void FadeIn() {
        DoFade(true);
    }

    public void FadeOut() {
        DoFade(false);
    }

    public void FadeIn(Action onComplete) {
        DoFade(true);
        onComplete();
    }

    public void FadeOut(Action onComplete) {
        DoFade(false);
        onComplete();
    }

    private void DoFade(bool fadeIn) {
        if (fadeIn) {
            tweener.PlayTweenReversed();
        }
        else {
            tweener.PlayTween();
        }
    }
}
