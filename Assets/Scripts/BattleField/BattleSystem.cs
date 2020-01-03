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
    [SerializeField] private Tweener healthMenu;

    [Header("Enemy Arrow")]
    [SerializeField] private Transform arrow;
    [SerializeField] private Light arrowLight;
    [SerializeField] private float offset;
    [SerializeField] private float lightIntensity;

    [Header("Field Graphics")]
    [SerializeField] private GameObject lines;

    [Header("Start Battle Settings")]
    [SerializeField] private float enemySpawnOffset;
    [SerializeField] private float waitBetweenEnter;

    [Header("End Battle Settings")]
    [SerializeField] private ObjectPool coinStack;
    [SerializeField] private Vector3 coinOrigin;
    [SerializeField] private float waitBetweenActions;
    [SerializeField] private float waitBetweenText;
    [SerializeField] private float coinFallTime;
    [SerializeField] private float coinToPartyTime;
    [SerializeField] private float jumpStrength;
    [SerializeField] private Ease coinToPartyEase;

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

        SetupPartyPlayer();
        filler.FillWithStats();
    }

    private void SetupPartyPlayer() {
        playerParty.ResetCharacters();

        foreach (CharacterBase character in playerParty.characters) {
            int level = character.Faction == Faction.Player ? playerParty.Level : enemyParty.Level;

            character.SetHealth(Calculator.GetStat(character.stats.maximumHealth, level, true));
            character.SetMP(Calculator.GetStat(character.stats.maximumMP, level, true));
        }
    }

    private void SetupPartyEnemy() {
        enemyParty.ResetCharacters();

        foreach (CharacterBase character in enemyParty.characters) {
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
        player = new List<CharacterBase>();
        enemy = new List<CharacterBase>();
        CurrentTurn = null;

        StartCoroutine(StartBattleCoroutine());
    }

    private IEnumerator StartBattleCoroutine() {
        Game.Instance.CameraContainer.GetComponent<Tweener>().PlayTween();
        infoBox.ClearText();
        ShowHideLines(true);
        healthMenu.PlayTween();

        foreach (CharacterBase character in playerParty.characters) {
            AddCharacterToBattle(character);
        }

        enemySpawner.SpawnNewFormation(Vector3.right * enemySpawnOffset);
        SetupPartyEnemy();
        yield return new WaitForSecondsRealtime(waitBetweenEnter / 2);

        foreach (CharacterBase character in enemyParty.characters) {
            AddCharacterToBattle(character);
            infoBox.EnemyEnterText(character);
            yield return new WaitForSecondsRealtime(waitBetweenEnter);
        }     

        List<CharacterBase> characters = GetCharacterList();

        turnOrder = new List<CharacterBase>();
        turnOrder = characters.OrderByDescending(x => Calculator.GetStat(x.stats.speed, x.Faction == Faction.Player ? playerParty.Level : enemyParty.Level)).ToList();
        turnIndex = 0;
        NextTurn(true);

        filler.FillCurrentTurn();
        yield return null;
    }

    private void ShowHideLines(bool show) {
        foreach (SpriteRenderer sprite in lines.GetComponentsInChildren<SpriteRenderer>()) {
            sprite.DOColor(new Color(sprite.color.r, sprite.color.g, sprite.color.b, show ? 1f : 0f), 0.5f);
        }
    }

    public void NextTurn(bool firstTurn = false) {
        if (PlayerLost()) {
            infoBox.DebugText("Game Over!");
            return;
        }
        else if (EnemyLost()) {
            StartCoroutine(BattleWon());
            return;
        }

        if (firstTurn == false) {
            AdvanceTurnIndex();
        }

        List<CharacterBase> characterList = GetCharacterList();

        foreach (CharacterBase character in characterList) {
            laneMover.SetCharacterToLane(character, character.Lane, character.Faction == Faction.Player ? player : enemy);
        }
        
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

    public bool PlayerLost() {
        foreach (CharacterBase character in player) {
            if (character.IsDead == false) return false;
        }
        return true;
    }

    public bool EnemyLost() {
        foreach (CharacterBase character in enemy) {
            if (character.IsDead == false) return false;
        }
        return true;
    }

    private IEnumerator BattleWon() {
        ClearEnemyParty();

        GameObject stack = ObjectPooler.Instance.SpawnFromPool(coinStack.tag, playerParty.transform, coinOrigin, Quaternion.identity);
        stack.transform.DOScale(Vector3.one, 0);

        Tween coinFall = stack.transform.DOLocalMoveY(0, coinFallTime).SetEase(Ease.OutBounce);
        yield return coinFall.WaitForCompletion();

        yield return new WaitForSecondsRealtime(waitBetweenActions);

        Transform currentLeader = null;

        for (int i = 0; i < playerParty.characters.Count; i++) {
            if (playerParty.characters[i].IsDead == false) {
                currentLeader = playerParty.characters[i].transform;
                break;
            }
        }

        Tween coinToLeader = stack.transform.DOLocalJump(currentLeader.localPosition, jumpStrength, 1, coinToPartyTime).SetEase(coinToPartyEase);
        Tween coinScale = stack.transform.DOScale(Vector3.zero, coinToPartyTime).SetEase(coinToPartyEase);
        yield return coinToLeader.WaitForCompletion();

        int coins = enemySpawner.GetCoins();
        int experience = enemySpawner.GetExperience();

        infoBox.GainGoldText(coins);
        yield return new WaitForSecondsRealtime(waitBetweenText);

        infoBox.GainExperienceText(experience);
        yield return new WaitForSecondsRealtime(waitBetweenText);

        ShowHideLines(false);
        healthMenu.PlayTweenReversed();
        Game.Instance.SetWalking();
    }

    private void ClearEnemyParty() {
        foreach(CharacterBase character in enemy) {
            ObjectPooler.Instance.Despawn(character.gameObject);
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