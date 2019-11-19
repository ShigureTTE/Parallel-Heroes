using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChooseLaneButton : MonoBehaviour {

    [SerializeField] private Party party;
    [SerializeField] private MoveCharacterToLane battleField;

    private Lane lane;

    public void SetNewLane(string newLane) {
        CharacterBase currentChar = party.currentTurn;
        Enum.TryParse(newLane, out lane);

        battleField.SetCharacterToLane(currentChar, lane);
    }
}
