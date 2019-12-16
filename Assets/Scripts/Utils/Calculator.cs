using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Calculator : MonoBehaviour {

    public static int GetStat(int baseStat, int level, bool healthOrMP = false) {
        int statPoints = healthOrMP ? ((2 * baseStat * level) / 100) + level + 12 : ((2 * baseStat * level) / 100) + 5;

        return statPoints;
    }

    public static int GetDamage(CharacterBase attacker, CharacterBase defender, Attack attack, bool counterAttack = false) {
        CharacterStats stats = attacker.stats;
        int attackStat = GetStat(stats.attack, attacker.Party.Level);
        int skillStat = GetStat(stats.skill, attacker.Party.Level);
        int defenseStat = attack.element == Element.Normal ? GetStat(defender.stats.defense, defender.Party.Level) : GetStat(defender.stats.resistance, defender.Party.Level);

        float levelSkillBonus = (((float)attacker.Party.Level + (float)skillStat) / 4f) + 1f;
        float damageFloat = (levelSkillBonus * ((float)attackStat / (float)defenseStat) * (float)attack.basePower) / 40f;
        //TODO: WeaponModifier
        float randomModifier = 1f * Random.Range(0.9f, 1f);
        float prefLaneModifier = attacker.Lane == attack.preferredLane ? 1.1f : 0.9f;
        float criticalModifier = Random.Range(0, 50) == 0 ? 1.5f : 1f;
        float blockingModifier = defender.IsBlocking ? 0.5f : 1f;

        damageFloat = damageFloat * randomModifier * prefLaneModifier * criticalModifier;

        damageFloat *= blockingModifier;
        if (counterAttack) damageFloat *= 0.5f;

        int damage = Mathf.RoundToInt(damageFloat);

        return damage;
    }

    public static List<CharacterBase> GetAvailableTargets(Lane currentLane, List<CharacterBase> possibleTargets) {
        List<CharacterBase> targets = new List<CharacterBase>();
        Lane lanes;

        switch (currentLane) {
            case Lane.Close:
                lanes = Lane.Close | Lane.Mid | Lane.Long;
                break;
            case Lane.Mid:
                lanes = Lane.Close | Lane.Mid;
                break;
            case Lane.Long:
                lanes = Lane.Close;
                break;
            default:
                lanes = Lane.Close;
                break;
        }

        foreach (CharacterBase characterBase in possibleTargets) {
            if (characterBase.IsDead) continue;

            if ((characterBase.Lane & lanes) != 0) {
                targets.Add(characterBase);
            }
        }

        return targets;
    }

    public static List<CharacterBase> GetCounterAttackers(CharacterBase defender) {
        if (defender.Lane == Lane.Close) return null;

        List<CharacterBase> counterAttackers = new List<CharacterBase>();
        Lane lanes;

        switch (defender.Lane) {
            case Lane.Mid:
                lanes = Lane.Close;
                break;
            case Lane.Long:
                lanes = Lane.Close | Lane.Mid;
                break;
            default:
                lanes = Lane.Close;
                break;
        }

        foreach (CharacterBase characterBase in defender.Party.characters) {
            if (characterBase.IsDead) continue;

            if ((characterBase.Lane & lanes) != 0) {
                counterAttackers.Add(characterBase);
            }
        }

        if (counterAttackers.Count == 0) return null;
        return counterAttackers;
    }
}
