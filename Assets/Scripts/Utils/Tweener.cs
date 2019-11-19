using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;
using System;
using UnityEngine.InputSystem.UI;

public class Tweener : MonoBehaviour {

    [SerializeField] private List<TweenObject> tweenObjects;

    public void PlayTween() {
        DOTween.KillAll(true);
        foreach (TweenObject to in tweenObjects) {
            Transform toTransform = to.objectToTween.transform;
            switch (to.type) {
                case TweenType.Position:
                    toTransform.DOLocalMove(to.endPosition, to.duration).SetEase(to.easeSetting).OnComplete(() => InvokeOnCompleteEvent(to));
                    break;
                case TweenType.Rotation:
                    toTransform.DORotate(to.endPosition, to.duration).SetEase(to.easeSetting).OnComplete(() => InvokeOnCompleteEvent(to)); 
                    break;
                case TweenType.Scale:
                    toTransform.DOScale(to.endPosition, to.duration).SetEase(to.easeSetting).OnComplete(() => InvokeOnCompleteEvent(to)); 
                    break;
                case TweenType.CanvasAlpha:
                    CanvasGroup canvas = to.objectToTween.GetComponent<CanvasGroup>();
                    if (canvas) {
                        canvas.DOFade(1f, to.duration).SetEase(to.easeSetting).OnComplete(() => InvokeOnCompleteEvent(to)); 
                    } else {
                        Debug.LogError("No canvas group found on: " + to.objectToTween);
                        continue;
                    }
                    break;
            }
        }
    }

    public void PlayTweenReversed() {
        DOTween.KillAll(true);
        foreach (TweenObject to in tweenObjects) {
            Transform toTransform = to.objectToTween.transform;
            switch (to.type) {
                case TweenType.Position:
                    toTransform.DOLocalMove(to.startPosition, to.durationReverse).SetEase(to.easeSettingReverse).OnComplete(() => InvokeOnCompleteEventReverse(to));
                    break;
                case TweenType.Rotation:
                    toTransform.DORotate(to.startPosition, to.durationReverse).SetEase(to.easeSettingReverse).OnComplete(() => InvokeOnCompleteEventReverse(to));
                    break;
                case TweenType.Scale:
                    toTransform.DOScale(to.startPosition, to.durationReverse).SetEase(to.easeSettingReverse).OnComplete(() => InvokeOnCompleteEventReverse(to));
                    break;
                case TweenType.CanvasAlpha:
                    CanvasGroup canvas = to.objectToTween.GetComponent<CanvasGroup>();
                    if (canvas) {
                        canvas.DOFade(0f, to.durationReverse).SetEase(to.easeSettingReverse).OnComplete(() => InvokeOnCompleteEventReverse(to));
                    } else {
                        Debug.LogError("No canvas group found on: " + to.objectToTween);
                        continue;
                    }
                    break;
            }
        }
    }

    public void InvokeOnCompleteEvent(TweenObject to) {
        to.onCompleteTweenEvent.Invoke();
    }

    public void InvokeOnCompleteEventReverse(TweenObject to) {
        to.onCompleteTweenReverseEvent.Invoke();
    }
}

[System.Serializable]
public class TweenObject {
    [Serializable] public class OnCompleteTween : UnityEvent { }

    [Header("ObjectSettings")]
    public GameObject objectToTween;
    public TweenType type = TweenType.Position;

    [Header("Tween Settings")]
    public Ease easeSetting;
    public Ease easeSettingReverse;
    public float duration;
    public float durationReverse;

    [Header("Positions")]
    public Vector3 startPosition;
    public Vector3 endPosition;

    [Header("Events")]
    public OnCompleteTween onCompleteTweenEvent;
    public OnCompleteTween onCompleteTweenReverseEvent;
    
    
}


