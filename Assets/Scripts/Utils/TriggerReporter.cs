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
    public class TriggerEnter : UnityEvent<Collider> { }
    public TriggerEnter onTriggerEnterEvent;

    [Serializable]
    public class TriggerExit : UnityEvent<Collider> { }
    public TriggerExit onTriggerExitEvent;

    private void OnEnable() {
        if (!resetOnDisableObject) return;

        hasTriggered = false;
    }

    private void OnTriggerEnter(Collider other) {
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

    private void OnTriggerExit(Collider other) {
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
