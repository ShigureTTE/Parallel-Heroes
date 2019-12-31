using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : DontDestroySingleton<Game> {

    public GameState State { get; private set; }

    private void Awake() {
        State = GameState.Walk;
    }
}
