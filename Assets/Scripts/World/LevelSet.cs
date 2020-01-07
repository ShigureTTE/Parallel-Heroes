using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LevelSet : MonoBehaviour {
    public GameObject foreground;
    public GameObject antiVoid;
    public GameObject background;
    public GameObject encounterObjects;

    private LevelGenerator generator;

    [System.Serializable]
    public class OnRefresh : UnityEvent { }
    public OnRefresh onRefreshEvent;

    public LevelType Type { get; set; }

    private void OnEnable() {
        generator = LevelGenerator.Instance;
    }

    public void GenerateTriggerEnter(Collider col) {
        generator.GenerateTrigger(this);
    }

    public void SetEnter(Collider col) {
        generator.OnSetEnter(this);
    }

    public void RefreshEncounter() {
        onRefreshEvent.Invoke();
    }
}
