using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InformationBox : MonoBehaviour {

    [SerializeField] private BattleSystem battleSystem;
    [SerializeField] private TextMeshProUGUI infoBox;
    [SerializeField] private float pauseBetweenInfos;

    private Tweener alphaTweener;

    private void Start() {
        alphaTweener = infoBox.GetComponent<Tweener>();
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
}