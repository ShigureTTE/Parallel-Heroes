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
    [SerializeField] private InformationBox infoBox;

    [Header("Action Settings")]
    [SerializeField] private float littlePause;
    [SerializeField] private float attackTweenTime;
    [SerializeField] private float returnDelay;
    [SerializeField] private Ease easeType;
    [SerializeField] private GameObject hitEffect;

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

        StartCoroutine(NormalAttackCoroutine());
    }

    public void SpellAttack(CharacterBase attacker, CharacterBase defender, int spellIndex) {
        RevertAllTweens();
        main = attacker;
        other = defender;
        attack = main.stats.spells[spellIndex];

        StartCoroutine(NormalAttackCoroutine());
    }

    public void Block(CharacterBase blocker) {
        RevertAllTweens();
        main = blocker;
    }

    public IEnumerator NormalAttackCoroutine() {
        infoBox.AttackText(main, other);
        yield return new WaitForSecondsRealtime(littlePause);
        
        Vector3 oldPosition = main.transform.position;
        Vector3 targetPosition = new Vector3(other.transform.position.x, other.transform.position.y, other.transform.position.z + 0.05f);

        Tween tween = main.transform.DOMove(targetPosition, attackTweenTime).SetEase(easeType);
        yield return tween.WaitForCompletion();

        Instantiate(hitEffect, other.transform.position, Quaternion.identity, other.transform);
        int damage = Calculator.GetDamage(main.Party, other, attack, other.Party.Level);
        other.SubstractHealth(damage);
        infoBox.DamageText(main, other, damage);
        yield return new WaitForSecondsRealtime(returnDelay);

        tween = main.transform.DOMove(oldPosition, attackTweenTime).SetEase(easeType);
        yield return tween.WaitForCompletion();
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
