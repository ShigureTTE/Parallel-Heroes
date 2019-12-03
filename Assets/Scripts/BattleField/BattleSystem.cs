using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using DG.Tweening;
using System;

public class BattleSystem : MonoBehaviour {

    [Header("Parties")]
    [SerializeField] private Party playerParty;
    [SerializeField] private Party enemyParty;

    [Header("UI")]
    [SerializeField] private UIFiller filler;
    [SerializeField] private InformationBox infoBox;
    [SerializeField] private Tweener characterMenu;

    [Header("Enemy Arrow")]
    [SerializeField] private Transform arrow;
    [SerializeField] private Light arrowLight;
    [SerializeField] private float offset;
    [SerializeField] private float lightIntensity;

    [HideInInspector] public List<CharacterBase> player = new List<CharacterBase>();
    [HideInInspector] public List<CharacterBase> enemy = new List<CharacterBase>();

    private MoveCharacterToLane laneMover;
    private List<CharacterBase> turnOrder;
    private EnemySpawner enemySpawner;
    private PerformAction performAction;

    private GameObject selectedEnemy;

    private AttackType chosenAttack;
    private int spellIndex;

    public CharacterBase CurrentTurn { get; private set; }
    private int turnIndex;

    private void Start() {
        laneMover = GetComponent<MoveCharacterToLane>();
        enemySpawner = GetComponent<EnemySpawner>();
        performAction = GetComponent<PerformAction>();
        enemySpawner.SpawnNewFormation();
        NewBattle();
    }

    public void NewBattle() {
        List<CharacterBase> characters = new List<CharacterBase>();
        playerParty.ResetCharacters();
        enemyParty.ResetCharacters();

        characters.AddRange(playerParty.characters);
        characters.AddRange(enemyParty.characters);

        foreach (CharacterBase character in characters) {
            AddCharacter(character);

            int level = character.Faction == Faction.Player ? playerParty.Level : enemyParty.Level;

            character.SetHealth(Calculator.GetStat(character.stats.maximumHealth, level, true));
            character.SetMP(Calculator.GetStat(character.stats.maximumMP, level, true));
        }

        turnOrder = new List<CharacterBase>();
        turnOrder = characters.OrderByDescending(x => Calculator.GetStat(x.stats.speed, x.Faction == Faction.Player ? playerParty.Level : enemyParty.Level)).ToList();
        CurrentTurn = turnOrder[0];
        turnIndex = 0;
        infoBox.TurnText(CurrentTurn);

        filler.FillAll();
    }

    public void NextTurn() {
        if (turnIndex + 1 >= turnOrder.Count) turnIndex = 0;
        else turnIndex++;

        while (turnOrder[turnIndex].Faction == Faction.Enemy) { //REMOVE THIS WHILE LOOP WHEN ENEMY AI HAS BEEN IMPLEMENTED.
            if (turnIndex + 1 >= turnOrder.Count) turnIndex = 0;
            else turnIndex++;
        }

        CurrentTurn = turnOrder[turnIndex];
        filler.FillCurrentTurn();
        CurrentTurn.IsBlocking = false;
        infoBox.TurnText(CurrentTurn);
        characterMenu.PlayTween();
    }

    public void AddCharacter(CharacterBase character) {
        if (character.Faction == Faction.Player) player.Add(character);
        else enemy.Add(character);

        laneMover.SetCharacterToLane(character, character.Lane, character.Faction == Faction.Player ? player : enemy);
    }

    public void ShowArrow(TextMeshProUGUI textObject) {
        string text = textObject.text;

        selectedEnemy = enemyParty.characters.Single(x => x.gameObject.name == text).gameObject;

        Bounds spriteBounds = selectedEnemy.GetComponentInChildren<SpriteRenderer>().bounds;
        arrow.transform.position = new Vector3(selectedEnemy.transform.position.x, selectedEnemy.transform.position.y + spriteBounds.size.y / 2 + offset, selectedEnemy.transform.position.z);

        arrow.GetComponent<Tweener>().PlayTween();
        arrowLight.DOIntensity(lightIntensity, .25f);
    }

    public void HideArrow() {
        arrow.GetComponent<Tweener>().PlayTweenReversed();
        arrowLight.DOIntensity(0, .25f);
    }

    public void SetChosenAttackType(string attackType) {
        Enum.TryParse(attackType, out chosenAttack);
    }

    public void SetSpellIndex(int index) {
        spellIndex = index;
    }

    public void CompleteTurn() {
        switch (chosenAttack) {
            case AttackType.Normal:
                performAction.NormalAttack(CurrentTurn, selectedEnemy.GetComponent<CharacterBase>());
                break;
            case AttackType.Spell:
                performAction.SpellAttack(CurrentTurn, selectedEnemy.GetComponent<CharacterBase>(), spellIndex);
                break;
            case AttackType.Combo:
                break;
            case AttackType.Block:
                performAction.Block(CurrentTurn);
                break;
        }

        GetComponent<LaneHighlighter>().NoHighlight();
        GetComponent<LaneHighlighter>().NoRangeType();
    }

    private void RefreshLists() {
        player = new List<CharacterBase>();
        enemy = new List<CharacterBase>();
    }
}