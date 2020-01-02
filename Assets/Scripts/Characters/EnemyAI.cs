using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyAI {

    private static AIAction action;

    public static AIAction GetAIAction(CharacterBase currentTurn, Party enemyParty, Party playerParty) {
        CharacterStats stats = currentTurn.stats;
        List<CharacterBase> characterList = new List<CharacterBase>();

        action = new AIAction();

        //See which spells I can use
        List<Attack> availableSpells = new List<Attack>();
        foreach (Attack spell in stats.spells) {
            if (spell.mPCost <= currentTurn.CurrentMP) availableSpells.Add(spell);
        }

        //Can I kill a character with any available attack?
        if (stats.goesForKill) {
            PredictCanKill(currentTurn, playerParty, availableSpells);
            //I found somebody I can possibly kill, but do I want to live?
            if (action.target != null && stats.wantsToLive) {
                if (PredictDieFromAttack(currentTurn)) {
                    //Discard current action, it is not safe.
                    action = new AIAction();
                }
            }
            //I hate  self damage, so I'll discard my action if I'll get hurt in any way.
            if (action.target != null && stats.hatesSelfDamage) {
                characterList = Calculator.GetCounterAttackers(action.target);
                if (characterList.Count > 0) {
                    action = new AIAction();
                }
            }
        }

        //I wasn't able to find anything, so let's try again.
        if (action.target == null) {
            action.target = playerParty.characters[Random.Range(0, playerParty.characters.Count)];

            if (stats.wantsToLive) {
                while (PredictDieFromAttack(currentTurn) || action.target.IsDead) {
                    action.target = playerParty.characters[IncrementNumber(playerParty, playerParty.characters.IndexOf(action.target))];
                }
            }

            if (stats.hatesSelfDamage) {
                characterList = Calculator.GetCounterAttackers(action.target);
                while (characterList.Count != 0 || action.target.IsDead) {
                    action.target = playerParty.characters[IncrementNumber(playerParty, playerParty.characters.IndexOf(action.target))];
                    characterList = Calculator.GetCounterAttackers(action.target);
                }
            }

            if (stats.hatesBlockers && !stats.hatesSelfDamage) {
                while (action.target.IsBlocking || action.target.IsDead) {
                    action.target = playerParty.characters[IncrementNumber(playerParty, playerParty.characters.IndexOf(action.target))];
                }
            }
        }

        //Target has been acquired. Just need an attack to use.
        if (action.attack == null) {
            if (availableSpells.Count > 0 && Random.Range(0, 101) >= (stats.spellUser ? 15 : 70)) {
                action.attack = availableSpells[Random.Range(0, availableSpells.Count)];
                action.attackType = AttackType.Spell;
            }
            else {
                action.attack = stats.normalAttack;
                action.attackType = AttackType.Normal;                
            }
        }

        if (Random.Range(0, 101) >= (stats.likesToBlock ? 5 : 70)) {
            action.attack = stats.normalAttack;
            action.attackType = AttackType.Block;
        }

        //I should have a target now, time to find a lane.
        if (action.target != null) {
            FindLane(currentTurn, playerParty, enemyParty);
        }

        return action;
    }

    private static int IncrementNumber(Party playerParty, int index) {
        if (index + 1 >= playerParty.characters.Count) index = 0;
        else index++;

        return index;
    }

    private static void FindLane(CharacterBase currentTurn, Party playerParty, Party enemyParty) {
        CharacterStats stats = currentTurn.stats;
        List<CharacterBase> characterList = new List<CharacterBase>();

        //I prefer to hide behind my friends.
        if (stats.hider && stats.likesPreferredLane && action.attackType != AttackType.Block) {
            characterList = Calculator.GetAvailableTargets(action.attack.preferredLane, playerParty.characters);
            if (characterList.Contains(action.target) &&
                !enemyParty.characters.Any(x => x.Lane.Equals(action.attack.preferredLane) && x.IsDead == false)) {
                action.lane = action.attack.preferredLane;
                return;
            }
        } //Do I like to use my preferred lane?
        if (stats.likesPreferredLane) {
            characterList = Calculator.GetAvailableTargets(action.attack.preferredLane, playerParty.characters);
            if (characterList.Contains(action.target)) {
                //I can attack my preferred target from my preferred lane
                action.lane = action.attack.preferredLane;
                return;
            }
            //I can't hit the target from my preferred lane, so let's try to find someplace else.
        }

        if (stats.hider) {
            characterList = Calculator.GetAvailableTargets(Lane.Close, playerParty.characters);
            if (characterList.Contains(action.target) &&
                !enemyParty.characters.Any(x => x.Lane.Equals(Lane.Close) && x.IsDead == false)) {
                action.lane = Lane.Close;
                return;
            }
            characterList = Calculator.GetAvailableTargets(Lane.Mid, playerParty.characters);
            if (characterList.Contains(action.target) &&
                !enemyParty.characters.Any(x => x.Lane.Equals(Lane.Mid) && x.IsDead == false)) {
                action.lane = Lane.Mid;
                return;
            }
            characterList = Calculator.GetAvailableTargets(Lane.Long, playerParty.characters);
            if (characterList.Contains(action.target) &&
                !enemyParty.characters.Any(x => x.Lane.Equals(Lane.Long) && x.IsDead == false)) {
                action.lane = Lane.Long;
                return;
            }
        }
        else {
            characterList = Calculator.GetAvailableTargets(Lane.Long, playerParty.characters);
            if (characterList.Contains(action.target)) {
                action.lane = Lane.Long;
                return;
            }
            characterList = Calculator.GetAvailableTargets(Lane.Mid, playerParty.characters);
            if (characterList.Contains(action.target)) {
                action.lane = Lane.Mid;
                return;
            }
            characterList = Calculator.GetAvailableTargets(Lane.Close, playerParty.characters);
            if (characterList.Contains(action.target)) {
                action.lane = Lane.Close;
                return;
            }
        }

        action.lane = Lane.Close;
    }

    private static bool PredictDieFromAttack(CharacterBase currentTurn) {
        List<CharacterBase> counterAttackers = Calculator.GetCounterAttackers(action.target);

        int damage = 0;
        foreach (CharacterBase character in counterAttackers) {
            damage += Calculator.GetDamage(currentTurn, action.target, action.target.stats.normalAttack, true);
        }

        return damage >= currentTurn.CurrentHealth;
    }

    private static void PredictCanKill(CharacterBase currentTurn, Party playerParty, List<Attack> availableSpells) {
        CharacterStats stats = currentTurn.stats;

        foreach (CharacterBase character in playerParty.characters) {
            if (character.IsDead) continue;

            int health = character.CurrentHealth;

            if (Calculator.GetDamage(currentTurn, character, stats.normalAttack) >= health) {
                action.target = character;
                action.attack = stats.normalAttack;
                action.attackType = AttackType.Normal;
                return;
            }

            foreach (Attack spell in availableSpells) {
                if (Calculator.GetDamage(currentTurn, character, spell) >= health) {
                    action.target = character;
                    action.attack = spell;
                    action.attackType = AttackType.Spell;
                    return;
                }
            }
        }
    }
}

public class AIAction {
    public Lane lane;
    public Attack attack;
    public AttackType attackType;
    public CharacterBase target;
}