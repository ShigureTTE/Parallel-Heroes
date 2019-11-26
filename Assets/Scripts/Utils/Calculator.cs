using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Calculator : MonoBehaviour {

    public static int GetStat(int baseStat, int level, bool healthOrMP = false) {
        int statPoints = healthOrMP ? ((2 * baseStat * level) / 100) + level + 12 : ((2 * baseStat * level) / 100) + 5;

        return statPoints;
    }

    public static int GetDamage(Party attackerParty, CharacterBase defender, Attack attack, int defenderLevel) {
        CharacterStats stats = attackerParty.currentTurn.stats;
        int attackStat = GetStat(stats.attack, attackerParty.Level);
        int skillStat = GetStat(stats.skill, attackerParty.Level);
        int defenseStat = attack.element == Element.Normal ? GetStat(defender.stats.defense, defenderLevel) : GetStat(defender.stats.resistance, defenderLevel);

        float levelSkillBonus = (((float)attackerParty.Level + (float)skillStat) / 4f) + 1f;
        float damageFloat = (levelSkillBonus * ((float)attackStat / (float)defenseStat) * (float)attack.basePower) / 40f;
        //TODO: WeaponModifier
        float randomModifier = 1f * Random.Range(0.9f, 1f);
        float laneModifier = attackerParty.currentTurn.Lane == attack.preferredLane ? 1.1f : 0.9f;
        float criticalModifier = Random.Range(0, 50) == 0 ? 1.5f : 1f;

        damageFloat = damageFloat * randomModifier * laneModifier * criticalModifier;

        int damage = Mathf.RoundToInt(damageFloat);

        return damage;
    }
}
