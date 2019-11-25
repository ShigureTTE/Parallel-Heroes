using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UINavigator : MonoBehaviour {

    [SerializeField] private EventSystem eventSystem;

    private CanvasGroup[] registeredBackMenu = new CanvasGroup[4];
    private Button[] registeredButton = new Button[4];
    private int layer = 0;

    public void IncrementLayer() {
        layer++;
    }
    
    private void ReductLayer() {
        layer--;
        if (layer < 0) layer = 0;
    }

    public void RegisterBackMenu(CanvasGroup menu) {
        registeredBackMenu[layer] = menu;
    }

    public void RegisterButton(Button button) {
        registeredButton[layer] = button;
    }

    public void PreviousMenu(Tweener menuTweener) {
        if (menuTweener.IsPlaying) return;

        ReductLayer();
        TweenObject to = menuTweener.tweenObjects[0];
        to.onCompleteTweenReverseEvent.RemoveAllListeners();

        to.onCompleteTweenReverseEvent.AddListener(() => {
            registeredBackMenu[layer].interactable = true;
            registeredButton[layer].Select();
            registeredButton[layer].GetComponent<EventTrigger>().enabled = true;
        });

        menuTweener.PlayTweenReversed();
        menuTweener.GetComponentInParent<CanvasGroup>().interactable = false;
    }
}
