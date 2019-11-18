using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerReporter : MonoBehaviour {

    [SerializeField]
    private bool onlyTriggerOnce;
    private bool hasTriggered;

    [SerializeField]
    private bool disableAfter;
    [SerializeField]
    private bool resetOnDisableObject = false;

    [SerializeField]
    private LayerMask hitLayers;

    [Serializable]
    public class OnTriggerEnter : UnityEvent<Collider2D> { }
    public OnTriggerEnter onTriggerEnterEvent;

    [Serializable]
    public class OnTriggerExit : UnityEvent<Collider2D> { }
    public OnTriggerExit onTriggerExitEvent;

    private void OnEnable() {
        if (!resetOnDisableObject) return;

        hasTriggered = false;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (hitLayers != (hitLayers | (1 << other.gameObject.layer)))
        {
            return;
        }

        if (onlyTriggerOnce && hasTriggered) {
            return;
        }

        hasTriggered = true;

        if (onTriggerEnterEvent != null) {
            onTriggerEnterEvent.Invoke(other);
        }

        if (disableAfter) {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (hitLayers != (hitLayers | (1 << other.gameObject.layer)))
        {
            return;
        }

        if (onlyTriggerOnce && hasTriggered) {
            return;
        }


        if (onTriggerExitEvent != null) {
            onTriggerExitEvent.Invoke(other);
        }
    }
}
