using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;

public class MoveCharacterToLane : MonoBehaviour {

    [Header("Positions")]

    [SerializeField] private LanePositions closeRange;
    [SerializeField] private LanePositions midRange;
    [SerializeField] private LanePositions longRange;

    [Header("Settings")]
    [SerializeField] private float moveDuration;
    [SerializeField] private Ease easeType;

    public List<CharacterBase> Player { get; private set; }
    public List<CharacterBase> Enemy { get; private set; }

    private void Start() {
        RefreshLists();
    }

    public void RegisterCharacterToBattle(CharacterBase character) {
        if (character.Faction == Faction.Player) Player.Add(character);
        else Enemy.Add(character);
    }

    private void RefreshLists() {
        Player = new List<CharacterBase>();
        Enemy = new List<CharacterBase>();
    }

    public void SetCharacterToLane(CharacterBase activeCharacter, Lane targetLane) {
        Lane oldLane = activeCharacter.Lane;
        activeCharacter.Lane = targetLane;

        List<CharacterBase> factionList = activeCharacter.Faction == Faction.Player ? Player : Enemy;
        List<CharacterBase> charactersToMove = new List<CharacterBase>();
        foreach (CharacterBase characterBase in factionList) {
            if (characterBase.Lane == targetLane || characterBase.Lane == oldLane) {
                charactersToMove.Add(characterBase);
            }
        }

        CharacterBase[] oldLaneChars = charactersToMove.Where(x => x.Lane == oldLane).ToArray();
        CharacterBase[] targetLaneChars = charactersToMove.Where(x => x.Lane == targetLane).ToArray();

        MoveInLane(oldLaneChars, oldLane);
        MoveInLane(targetLaneChars, targetLane);
    }

    private void MoveInLane(CharacterBase[] laneCharacters, Lane lane) {
        if (laneCharacters.Length == 0) return;
     
        LanePositions positions = GetLanePositionsObject(lane);

        bool evenAmountOfCharacters = IsIntEven(laneCharacters.Length);
        for (int i = 0; i < laneCharacters.Length; i++) {
            Transform targetPosition = null;

            if (evenAmountOfCharacters) {
                targetPosition = positions.evenPositions[i];
            }
            else {
                targetPosition = positions.oddPositions[i];
            }

            Transform characterTransform = laneCharacters[i].transform;

            switch (laneCharacters[i].Faction) {
                case Faction.Player:
                    characterTransform.localScale = new Vector3(1f, 1f, 1f);
                    break;
                case Faction.Enemy:
                    characterTransform.localScale = new Vector3(-1f, 1f, 1f);
                    break;
            }
           
            characterTransform.DOMove(laneCharacters[i].Faction == Faction.Player ?
                targetPosition.position : new Vector3(targetPosition.position.x - (targetPosition.position.x * 2),targetPosition.position.y,targetPosition.position.z),
                moveDuration).SetEase(easeType);
        }
    }

    private bool IsIntEven(int integer) {
        return integer % 2 == 0;
    }

    private LanePositions GetLanePositionsObject(Lane lane) {
        LanePositions lanePositions = null;
        switch (lane) {
            case Lane.Close:
                lanePositions = closeRange;
                break;
            case Lane.Mid:
                lanePositions = midRange;
                break;
            case Lane.Long:
                lanePositions = longRange;
                break;
        }

        return lanePositions;
    }
}

[System.Serializable]
public class LanePositions {
    [Header("Odd Characters")]
    public Transform[] oddPositions = new Transform[3];

    [Header("Even Characters")]
    public Transform[] evenPositions = new Transform[4];
}
