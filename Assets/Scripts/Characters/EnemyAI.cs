using System.Collections.Generic;

public class EnemyAI {

    private static AIAction action;

    public static AIAction GetAIAction(CharacterBase currentTurn, Party enemyParty, Party playerParty) {
        CharacterStats stats = currentTurn.stats;

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
                if (Calculator.GetCounterAttackers(action.target).Count > 0) {
                    action = new AIAction();
                }
            }
        }

        //Have I found a target at already?
        if (action.target != null) {
            FindLane(currentTurn, playerParty);
        }

        return null;
    }

    private static void FindLane(CharacterBase currentTurn, Party playerParty) {
        CharacterStats stats = currentTurn.stats;

        //Do I like to use my preferred lane?
        if (stats.likesPreferredLane) {
            if (Calculator.GetAvailableTargets(action.attack.preferredLane, playerParty.characters).Contains(action.target)) {
                //I can attack my preferred target from my preferred lane
                action.lane = action.attack.preferredLane;
            }
        }
        //No lane found just yet. HIER GEBLEVEN!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
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