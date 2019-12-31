using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using DG.Tweening;
using System;

public class BattleSystem : MonoBehaviourSingleton<BattleSystem> {

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
    private AIAction action;

    public CharacterBase CurrentTurn { get; private set; }
    private int turnIndex;

    private void Start() {
        laneMover = GetComponent<MoveCharacterToLane>();
        enemySpawner = GetComponent<EnemySpawner>();
        performAction = GetComponent<PerformAction>();

        SetupParties();
        filler.FillWithStats();
        //enemySpawner.SpawnNewFormation();
        //NewBattle();
    }

    private void SetupParties() {
        List<CharacterBase> characters = new List<CharacterBase>();
        playerParty.ResetCharacters();
        enemyParty.ResetCharacters();

        characters = GetCharacterList();

        foreach (CharacterBase character in characters) {
            int level = character.Faction == Faction.Player ? playerParty.Level : enemyParty.Level;

            character.SetHealth(Calculator.GetStat(character.stats.maximumHealth, level, true));
            character.SetMP(Calculator.GetStat(character.stats.maximumMP, level, true));
        }
    }

    private List<CharacterBase> GetCharacterList() {
        List<CharacterBase> characters = new List<CharacterBase>();

        characters.AddRange(playerParty.characters);
        characters.AddRange(enemyParty.characters);

        return characters;
    }

    public void NewBattle() {
        SetupParties();

        List<CharacterBase> characters = GetCharacterList();

        foreach (CharacterBase character in characters) {
            AddCharacterToBattle(character);
        }

        turnOrder = new List<CharacterBase>();
        turnOrder = characters.OrderByDescending(x => Calculator.GetStat(x.stats.speed, x.Faction == Faction.Player ? playerParty.Level : enemyParty.Level)).ToList();
        turnIndex = 0;
        SetCurrentTurnCharacter();

        filler.FillAll();
    }

    public void NextTurn() {
        AdvanceTurnIndex();

        while (turnOrder[turnIndex].IsDead) {
            AdvanceTurnIndex();
        }

        SetCurrentTurnCharacter();

        if (CurrentTurn.Faction == Faction.Enemy) {
            action = EnemyAI.GetAIAction(CurrentTurn, enemyParty, playerParty);
            StartCoroutine(DecideEnemyAction());
            return;
        }
        else {
            filler.FillCurrentTurn();
            characterMenu.PlayTween();
        }
    }

    private IEnumerator DecideEnemyAction() {
        chosenAttack = action.attackType;
        selectedEnemy = action.target.gameObject;
        laneMover.SetCharacterToLane(CurrentTurn, action.lane, enemy);
        yield return new WaitForSecondsRealtime(1f);
        spellIndex = Array.IndexOf(CurrentTurn.stats.spells, action.attack);

        CompleteTurn();
    }

    private void AdvanceTurnIndex() {
        if (turnIndex + 1 >= turnOrder.Count) turnIndex = 0;
        else turnIndex++;
    }

    private void SetCurrentTurnCharacter() {
        CurrentTurn = turnOrder[turnIndex];
        infoBox.TurnText(CurrentTurn);
        CurrentTurn.IsBlocking = false;

        CurrentTurn.SelectedEffect.Play();
    }

    public void AddCharacterToBattle(CharacterBase character) {
        if (character.Faction == Faction.Player) player.Add(character);
        else enemy.Add(character);

        if (Game.Instance.State == GameState.Battle)
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
        CurrentTurn.SelectedEffect.Stop();

        switch (chosenAttack) {
            case AttackType.Normal:
                performAction.NormalAttack(CurrentTurn, selectedEnemy.GetComponent<CharacterBase>(), CurrentTurn.stats.normalAttack);
                break;
            case AttackType.Spell:
                performAction.SpellAttack(CurrentTurn, selectedEnemy.GetComponent<CharacterBase>(), CurrentTurn.stats.spells[spellIndex]);
                CurrentTurn.SubtractMP(CurrentTurn.stats.spells[spellIndex].mPCost);
                UpdateStats();
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

    public void UpdateStats() {
        filler.FillWithStats();
    }

    private void RefreshLists() {
        player = new List<CharacterBase>();
        enemy = new List<CharacterBase>();
    }
}