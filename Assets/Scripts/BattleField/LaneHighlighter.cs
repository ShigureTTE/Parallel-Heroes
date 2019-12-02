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

    private BattleSystem battleSystem;
    private Attack attack;

    private void Start() {
        battleSystem = GetComponent<BattleSystem>();
    }

    public void NoHighlight() {
        closeRange.DOColor(normalLaneColor, 0.5f);
        midRange.DOColor(normalLaneColor, 0.5f);
        longRange.DOColor(normalLaneColor, 0.5f);
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
                closeRange.DOColor(preferredLaneColor, 0.5f);
                break;
            case Lane.Mid:
                midRange.DOColor(preferredLaneColor, 0.5f);
                break;
            case Lane.Long:
                longRange.DOColor(preferredLaneColor, 0.5f);
                break;
        }
    }
}
