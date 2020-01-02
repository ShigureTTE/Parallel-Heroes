﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : DontDestroySingleton<Game> {

    public GameState State { get; private set; }
    public GameObject CameraContainer { get; private set; }
    public LevelSet CurrentLevelSet { get; set; }

    private void Awake() {
        State = GameState.Walk;
        CameraContainer = GameObject.FindGameObjectWithTag(NYRA.Tag.CameraContainer);
    }

    public void SetEncounter() {
        State = GameState.Encounter;
    }

    public void ProcessEncounter() {
        switch (CurrentLevelSet.Type) {
            case LevelType.Battle:
                BattleEncounter();
                break;
            case LevelType.Exit:
                break;
            case LevelType.BranchingPath:
                break;
            case LevelType.SafeZone:
                break;
            case LevelType.TreasureChest:
                break;
            case LevelType.Mimic:
                break;
            case LevelType.MagicFountain:
                break;
            case LevelType.StorageRoom:
                break;
            case LevelType.Shop:
                break;
            case LevelType.Character:
                break;
            case LevelType.Trap:
                break;
            default:
                break;
        }
    }

    private void BattleEncounter() {
        State = GameState.Battle;
        BattleSystem.Instance.NewBattle();
    }
}
