using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerformAction : MonoBehaviour {

    [Header("UI Tweeners")]
    [SerializeField] private Tweener characterMenu;
    [SerializeField] private Tweener spellMenu;
    [SerializeField] private Tweener laneMenu;
    [SerializeField] private Tweener laneBlockMenu;
    [SerializeField] private Tweener targetMenu;

    [Header("UI Scripts")]
    [SerializeField] private UINavigator navigator;
    [SerializeField] private UIFiller filler;
    [SerializeField] private InformationBox infoBox;

    [Header("Action Settings")]
    [SerializeField] private float attackPause;
    [SerializeField] private float blockPause;
    [SerializeField] private float attackTweenTime;
    [SerializeField] private float returnDelay;
    [SerializeField] private float deadTweenTime;
    [SerializeField] private Ease easeType;
    [SerializeField] private GameObject hitEffect;
    [SerializeField] private GameObject blockEffect;
    [SerializeField] private GameObject deadEffect;

    private CharacterBase main;
    private CharacterBase other;
    private Attack attack;
    private BattleSystem battleSystem;
    private List<CharacterBase> deadCharactersThisTurn;

    private void Start() {
        battleSystem = GetComponent<BattleSystem>();
    }

    public void NormalAttack(CharacterBase attacker, CharacterBase defender, Attack attack) {
        RevertAllTweens();
        main = attacker;
        other = defender;
        this.attack = attack;

        StartCoroutine(attack.rangeType == RangeType.Melee ? AttackCoroutine(new Vector3(other.transform.position.x, other.transform.position.y, other.transform.position.z + 0.05f), true)
            : AttackCoroutine(new Vector3(transform.position.x, other.transform.position.y, transform.position.z)));
    }

    public void SpellAttack(CharacterBase attacker, CharacterBase defender, Attack attack) {
        RevertAllTweens();
        main = attacker;
        other = defender;
        this.attack = attack;

        StartCoroutine(attack.rangeType == RangeType.Melee ? AttackCoroutine(new Vector3(other.transform.position.x, other.transform.position.y, other.transform.position.z + 0.05f), true)
            : AttackCoroutine(new Vector3(transform.position.x, other.transform.position.y, transform.position.z)));
    }

    public void Block(CharacterBase blocker) {
        RevertAllTweens();
        main = blocker;

        StartCoroutine(BlockingCoroutine());
    }

    public IEnumerator AttackCoroutine(Vector3 targetPosition, bool melee = false) {
        deadCharactersThisTurn = new List<CharacterBase>();
        infoBox.AttackText(main, other);
        yield return new WaitForSecondsRealtime(attackPause);

        Vector3 oldPosition = main.transform.position;

        Tween tween = main.transform.DOMove(targetPosition, attackTweenTime).SetEase(easeType);
        yield return tween.WaitForCompletion();

        Instantiate(hitEffect, other.transform.position, hitEffect.transform.rotation, other.transform);
        int damage = Calculator.GetDamage(main, other, attack);
        bool hasDied = other.SubtractHealth(damage);
        if (other.Faction == Faction.Player) battleSystem.UpdateStats();
        infoBox.DamageText(main, other, damage);
        yield return new WaitForSecondsRealtime(returnDelay);

        if (melee) {
            yield return StartCoroutine(CounterAttackCoroutine());
        }

        tween = main.transform.DOMove(oldPosition, attackTweenTime).SetEase(easeType);
        yield return tween.WaitForCompletion();

        if (hasDied) {
            deadCharactersThisTurn.Add(other);
        }

        yield return KillCharacterCoroutine();
        battleSystem.NextTurn();
    }

    public IEnumerator CounterAttackCoroutine() {
        List<CharacterBase> counterAttackers = Calculator.GetCounterAttackers(other);
        if (counterAttackers.Count == 0) yield break;
        bool hasDied = false;

        yield return new WaitForSecondsRealtime(returnDelay);
        foreach (CharacterBase character in counterAttackers) {
            infoBox.CounterAttackText(character, main);
            yield return new WaitForSecondsRealtime(attackPause);

            Vector3 oldPosition = character.transform.position;
            Tween tween = character.transform.DOMove(new Vector3(main.transform.position.x, main.transform.position.y, main.transform.position.z + 0.05f), attackTweenTime).SetEase(easeType);
            yield return tween.WaitForCompletion();

            Instantiate(hitEffect, main.transform.position, hitEffect.transform.rotation, main.transform);
            int damage = Calculator.GetDamage(character, main, character.stats.normalAttack, true);
            hasDied = main.SubtractHealth(damage);
            if (main.Faction == Faction.Player) battleSystem.UpdateStats();
            infoBox.DamageText(character, main, damage);
            yield return new WaitForSecondsRealtime(returnDelay);

            tween = character.transform.DOMove(oldPosition, attackTweenTime).SetEase(easeType);
            yield return tween.WaitForCompletion();
        }

        if (hasDied) {
            deadCharactersThisTurn.Add(main);
        }
    }

    public IEnumerator KillCharacterCoroutine() {
        foreach (CharacterBase character in deadCharactersThisTurn) {
            infoBox.DefeatedText(character);
            Instantiate(deadEffect, character.transform.position, deadEffect.transform.rotation, character.transform);
            character.IsDead = true;

            Tween tween = character.transform.DOScale(Vector3.zero, deadTweenTime).SetEase(Ease.InBack);
            yield return tween.WaitForCompletion();
            yield return new WaitForSecondsRealtime(blockPause);
        }
    }

    public IEnumerator BlockingCoroutine() {
        infoBox.BlockText(main);
        main.IsBlocking = true;
        Instantiate(blockEffect, main.transform.position, blockEffect.transform.rotation, main.transform);
        yield return new WaitForSecondsRealtime(blockPause);
        battleSystem.NextTurn();
    }


    private void RevertAllTweens() {
        characterMenu.PlayTweenReversed();
        spellMenu.PlayTweenReversed();
        laneBlockMenu.PlayTweenReversed();
        laneMenu.PlayTweenReversed();
        targetMenu.PlayTweenReversed();

        navigator.ResetMenus();
    }
}
