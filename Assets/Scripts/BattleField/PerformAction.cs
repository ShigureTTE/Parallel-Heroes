﻿using DG.Tweening;
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
    [SerializeField] private InformationBox infoBox;

    [Header("Action Settings")]
    [SerializeField] private float attackPause;
    [SerializeField] private float blockPause;
    [SerializeField] private float attackTweenTime;
    [SerializeField] private float returnDelay;
    [SerializeField] private Ease easeType;
    [SerializeField] private GameObject hitEffect;
    [SerializeField] private GameObject blockEffect;

    private CharacterBase main;
    private CharacterBase other;
    private Attack attack;
    private BattleSystem battleSystem;

    private void Start() {
        battleSystem = GetComponent<BattleSystem>();
    }

    public void NormalAttack(CharacterBase attacker, CharacterBase defender) {
        RevertAllTweens();
        main = attacker;
        other = defender;
        attack = main.stats.normalAttack;

        StartCoroutine(attack.rangeType == RangeType.Melee ? MeleeAttackCoroutine() : RangedAttackCoroutine());
    }

    public void SpellAttack(CharacterBase attacker, CharacterBase defender, int spellIndex) {
        RevertAllTweens();
        main = attacker;
        other = defender;
        attack = main.stats.spells[spellIndex];

        StartCoroutine(attack.rangeType == RangeType.Melee ? MeleeAttackCoroutine() : RangedAttackCoroutine());
    }

    public void Block(CharacterBase blocker) {
        RevertAllTweens();
        main = blocker;

        StartCoroutine(BlockingCoroutine());
    }

    public IEnumerator MeleeAttackCoroutine() {
        infoBox.AttackText(main, other);
        yield return new WaitForSecondsRealtime(attackPause);
        
        Vector3 oldPosition = main.transform.position;
        Vector3 targetPosition = new Vector3(other.transform.position.x, other.transform.position.y, other.transform.position.z + 0.05f);

        Tween tween = main.transform.DOMove(targetPosition, attackTweenTime).SetEase(easeType);
        yield return tween.WaitForCompletion();

        Instantiate(hitEffect, other.transform.position, hitEffect.transform.rotation, other.transform);
        int damage = Calculator.GetDamage(main.Party, other, attack, other.Party.Level);
        other.SubstractHealth(damage);
        infoBox.DamageText(main, other, damage);
        yield return new WaitForSecondsRealtime(returnDelay);

        tween = main.transform.DOMove(oldPosition, attackTweenTime).SetEase(easeType);
        yield return tween.WaitForCompletion();
        battleSystem.NextTurn();
    }

    public IEnumerator RangedAttackCoroutine() {
        infoBox.AttackText(main, other);
        yield return new WaitForSecondsRealtime(attackPause);

        Vector3 oldPosition = main.transform.position;
        Vector3 targetPosition = new Vector3(transform.position.x, other.transform.position.y, transform.position.z);

        Tween tween = main.transform.DOMove(targetPosition, attackTweenTime).SetEase(easeType);
        yield return tween.WaitForCompletion();

        Instantiate(hitEffect, other.transform.position, hitEffect.transform.rotation, other.transform);
        int damage = Calculator.GetDamage(main.Party, other, attack, other.Party.Level);
        other.SubstractHealth(damage);
        infoBox.DamageText(main, other, damage);
        yield return new WaitForSecondsRealtime(returnDelay);

        tween = main.transform.DOMove(oldPosition, attackTweenTime).SetEase(easeType);
        yield return tween.WaitForCompletion();
        battleSystem.NextTurn();
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
