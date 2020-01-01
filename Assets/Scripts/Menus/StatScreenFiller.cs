using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StatScreenFiller : MonoBehaviour {

    [Header("Stats")]
    [SerializeField] private TextMeshProUGUI characterName;
    [SerializeField] private TextMeshProUGUI level;
    [SerializeField] private TextMeshProUGUI role;
    [SerializeField] private TextMeshProUGUI weapon;
    [SerializeField] private TextMeshProUGUI currentHP;
    [SerializeField] private TextMeshProUGUI maxHP;
    [SerializeField] private TextMeshProUGUI currentMP;
    [SerializeField] private TextMeshProUGUI maxMP;
    [SerializeField] private TextMeshProUGUI attack;
    [SerializeField] private TextMeshProUGUI defense;
    [SerializeField] private TextMeshProUGUI resistance;
    [SerializeField] private TextMeshProUGUI skill;
    [SerializeField] private TextMeshProUGUI speed;
    [SerializeField] private TextMeshProUGUI description;

    [Header("Attacks")]
    [SerializeField] private StatScreenAttack normalAttack;
    [SerializeField] private StatScreenAttack firstSpell;
    [SerializeField] private StatScreenAttack secondSpell;

    private const string normalAttackString = "Normal Attack";

    public void Fill(CharacterBase character, int level) {
        CharacterStats stats = character.stats;

        characterName.text = stats.characterName;
        this.level.text = level.ToString();
        role.text = stats.role;
        weapon.text = stats.weapon;
        description.text = stats.description;

        currentHP.text = character.CurrentHealth.ToString();
        currentMP.text = character.CurrentMP.ToString();
        maxHP.text = Calculator.GetStat(stats.maximumHealth, level, true).ToString();
        maxMP.text = Calculator.GetStat(stats.maximumMP, level, true).ToString();
        attack.text = Calculator.GetStat(stats.attack, level).ToString();
        defense.text = Calculator.GetStat(stats.defense, level).ToString();
        resistance.text = Calculator.GetStat(stats.resistance, level).ToString();
        skill.text = Calculator.GetStat(stats.skill, level).ToString();
        speed.text = Calculator.GetStat(stats.speed, level).ToString();

        FillAttack(stats.normalAttack, normalAttack);
        FillAttack(stats.spells[0], firstSpell);
        FillAttack(stats.spells[1], secondSpell);
    }

    private void FillAttack(Attack attack, StatScreenAttack ssa) {
        if (attack.type == AttackType.Normal) {
            ssa.attackName.text = normalAttackString;
        }
        else {
            ssa.attackName.text = attack.attackName;
            ssa.spellCost.text = attack.mPCost.ToString();
        }

        ssa.power.text = attack.basePower.ToString();
        ssa.element.text = attack.element.ToString();
        ssa.prefLane.text = attack.preferredLane.ToString();
        ssa.type.text = attack.rangeType.ToString();
    }
}

[System.Serializable]
public class StatScreenAttack {
    public TextMeshProUGUI attackName;
    public TextMeshProUGUI spellCost;
    public TextMeshProUGUI power;
    public TextMeshProUGUI element;
    public TextMeshProUGUI prefLane;
    public TextMeshProUGUI type;
}