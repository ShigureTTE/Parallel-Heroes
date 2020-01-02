using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSet : MonoBehaviour {
    public GameObject foreground;
    public GameObject antiVoid;
    public GameObject background;

    private LevelGenerator generator;

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
}
