using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LaneHighlighter : MonoBehaviour {

    [Header("Lane Sprites & Colors")]
    [SerializeField] private SpriteRenderer closeRange;
    [SerializeField] private SpriteRenderer midRange;
    [SerializeField] private SpriteRenderer longRange;
    [SerializeField] private Color normalLaneColor;
    [SerializeField] private Color preferredLaneColor;
    [SerializeField] private float fadeTimer;
    [SerializeField] private Ease easeType;

    [Header("Range Type Sprites")]
    [SerializeField] private Sprite meleeSprite;
    [SerializeField] private Sprite rangedSprite;

    private BattleSystem battleSystem;
    private Attack attack;

    private void Start() {
        battleSystem = GetComponent<BattleSystem>();
    }

    public void NoHighlight() {
        closeRange.DOColor(normalLaneColor, fadeTimer).SetEase(easeType);
        midRange.DOColor(normalLaneColor, fadeTimer).SetEase(easeType);
        longRange.DOColor(normalLaneColor, fadeTimer).SetEase(easeType);
    }

    public void NoRangeType() {
        battleSystem.CurrentTurn.RangeSprite.transform.DOScaleX(0, fadeTimer).SetEase(easeType);
    }

    public void AttackHighlight() {
        attack = battleSystem.CurrentTurn.stats.normalAttack;

        HighlightPrefferedLane();
    }

    public void SpellHighlight(int index) {
        attack = battleSystem.CurrentTurn.stats.spells[index];

        HighlightPrefferedLane();
    }

    private void HighlightPrefferedLane() {
        if (attack == null) return;

        switch (attack.preferredLane) {
            case Lane.Close:
                closeRange.DOColor(preferredLaneColor, fadeTimer).SetEase(easeType);
                break;
            case Lane.Mid:
                midRange.DOColor(preferredLaneColor, fadeTimer).SetEase(easeType);
                break;
            case Lane.Long:
                longRange.DOColor(preferredLaneColor, fadeTimer).SetEase(easeType);
                break;
        }

        ShowAttackType();
    }

    private void ShowAttackType() {
        battleSystem.CurrentTurn.RangeSprite.sprite = attack.rangeType == RangeType.Melee ? meleeSprite : rangedSprite;
        battleSystem.CurrentTurn.RangeSprite.transform.DOScaleX(1, fadeTimer).SetEase(easeType);
    }
}
