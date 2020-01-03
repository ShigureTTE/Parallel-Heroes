using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class InformationBox : MonoBehaviour {

    [SerializeField] private BattleSystem battleSystem;
    [SerializeField] private TextMeshProUGUI infoBox;
    [SerializeField] private float pauseBetweenInfos;

    private Tweener alphaTweener;

    private void Awake() {
        alphaTweener = infoBox.GetComponent<Tweener>();
    }

    public void DefeatedText(CharacterBase dead) {
        string text = dead.stats.name + IBConstants.defeated;

        StartCoroutine(NewText(text));
    }

    public void TurnText(CharacterBase current) {
        string text = IBConstants.its +
            IBConstants.commaS +
            current.stats.characterName +
            IBConstants.commaS +
            IBConstants.turn +
            IBConstants.exclamation;

        StartCoroutine(NewText(text));
    }

    public void BlockText(CharacterBase blocker) {
        string gender = "";
        switch (blocker.stats.gender) {
            case Gender.Male:
                gender = IBConstants.himself;
                break;
            case Gender.Female:
                gender = IBConstants.herself;
                break;
            case Gender.NonBinary:
                gender = IBConstants.themselves;
                break;
        }

        string text = blocker.stats.characterName +
            IBConstants.braces +
            gender +
            IBConstants.exclamation;

        StartCoroutine(NewText(text));
    }

    public void AttackText(CharacterBase attacker, CharacterBase defender) {
        string text = attacker.stats.characterName +
            IBConstants.attacks + defender.stats.characterName +
            IBConstants.exclamation;

        StartCoroutine(NewText(text));
    }

    public void DamageText(CharacterBase attacker, CharacterBase defender, int damage) {
        string text = defender.stats.characterName +
            IBConstants.took +
            damage.ToString() +
            IBConstants.damage +
            IBConstants.exclamation;

        StartCoroutine(NewText(text));
    }

    public void CounterAttackText(CharacterBase attacker, CharacterBase defender) {
        string text = attacker.stats.name +
            IBConstants.counter +
            defender.stats.name +
            IBConstants.exclamation;

        StartCoroutine(NewText(text));
    }

    public void EnemyEnterText(CharacterBase enemy) {
        string text = enemy.gameObject.name +
            IBConstants.appeared;

        StartCoroutine(NewText(text));
    }

    public void GainGoldText(int amount) {
        string text = IBConstants.gained +
            amount.ToString() +
            IBConstants.gold;

        StartCoroutine(NewText(text));
    }

    public void GainExperienceText(int amount) {
        string text = IBConstants.gained +
            amount.ToString() +
            IBConstants.experience;

        StartCoroutine(NewText(text));
    }

    public void DebugText(string text) {
        StartCoroutine(NewText(text));
    }

    public void ClearText() {
        StartCoroutine(NewText(""));
    }

    private IEnumerator NewText(string text) {
        alphaTweener.PlayTweenReversed();

        while (alphaTweener.IsPlaying) {
            yield return null;
        }

        infoBox.text = text;
        alphaTweener.PlayTween();
    }    
}

public class IBConstants {
    public static readonly string damage = " damage";
    public static readonly string its = "It";
    public static readonly string commaS = "'s ";
    public static readonly string turn = "turn";
    public static readonly string took = " took ";
    public static readonly string attacks = " attacks ";
    public static readonly string missed = " missed";
    public static readonly string exclamation = "!";
    public static readonly string braces = " braces ";
    public static readonly string himself = "himself";
    public static readonly string herself = "herself";
    public static readonly string themselves = "themselves";
    public static readonly string defeated = " was defeated!";
    public static readonly string counter = " counters ";
    public static readonly string appeared = " appeared!";
    public static readonly string gained = "Gained ";
    public static readonly string gold = " gold!";
    public static readonly string experience = " experience!";
}