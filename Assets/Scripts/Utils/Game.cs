using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : DontDestroySingleton<Game> {

    public GameState State { get; private set; }
    public GameObject CameraContainer { get; private set; }
    public Party PlayerParty { get; set; }
    public LevelSet CurrentLevelSet { get; set; }
    public Area CurrentArea { get; set; }

    private void Awake() {
        if (Instance != this) Destroy(gameObject);
        else DontDestroyOnLoad(gameObject);

        PlayerParty = GetComponentInChildren<Party>();
        CameraContainer = GameObject.FindGameObjectWithTag(NYRA.Tag.CameraContainer);
        State = GameState.Walk; //Why no exit?
    }

    private void OnLevelWasLoaded(int level) {        
        State = GameState.Walk;
    }

    public void SetEncounter() {
        State = GameState.Encounter;
    }

    public void SetWalking() {
        State = GameState.Walk;
    }

    public void ProcessEncounter() {
        switch (CurrentLevelSet.Type) {
            case LevelType.Battle:
                BattleEncounter();
                return;
            case LevelType.Exit:
                State = GameState.Exit;
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
                State = GameState.Character;
                break;
            case LevelType.Trap:
                break;
            default:
                break;
        }

        IEncounter encounter = CurrentLevelSet.encounterObjects.GetComponentInChildren<IEncounter>();
        encounter.Encounter();
    }

    private void BattleEncounter() {
        State = GameState.Battle;
        BattleSystem.Instance.NewBattle();
    }

    public void ChangeScene(string sceneName, LoadSceneMode mode) {
        SceneManager.LoadScene(sceneName, mode);
    }
}
