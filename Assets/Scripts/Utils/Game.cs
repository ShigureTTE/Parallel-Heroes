using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : DontDestroySingleton<Game> {

    public GameState State { get; private set; }
    public GameObject CameraContainer { get; private set; }

    private void Awake() {
        State = GameState.Walk;
        CameraContainer = GameObject.FindGameObjectWithTag(NYRA.Tag.CameraContainer);
    }

    public void BattleEncounter() {
        State = GameState.Battle;
        BattleSystem.Instance.NewBattle();
    }
}
