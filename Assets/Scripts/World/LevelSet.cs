using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSet : MonoBehaviour {
    public GameObject foreground;
    public GameObject antiVoid;
    public GameObject background;

    private LevelGenerator generator;

    private void OnEnable() {
        generator = LevelGenerator.Instance;
    }

    public void TriggerEnter(Collider col) {
        generator.TriggerCalled(this);
    }
}
