using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChooseLaneButton : MonoBehaviour {

    [SerializeField] private MoveCharacterToLane battleField;

    private Lane lane;
    private BattleSystem battleSystem;

    private void Awake() {
        battleSystem = BattleSystem.Instance;
    }

    public void SetNewLane(string newLane) {
        CharacterBase currentChar = battleSystem.CurrentTurn;
        Enum.TryParse(newLane, out lane);
        List<CharacterBase> factionList = currentChar.Faction == Faction.Player ? battleSystem.player : battleSystem.enemy;

        battleField.SetCharacterToLane(currentChar, lane, factionList);
    }
}
