using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Party : MonoBehaviour {

    [SerializeField] private MoveCharacterToLane battlefield;
    [HideInInspector] public List<CharacterBase> characters;

    public CharacterBase currentTurn;

    public int expNeededForLevel = 100;
    public int Level { get; private set; }
    private int experiencePoints = 0;

    private void Awake() {
        characters = GetComponentsInChildren<CharacterBase>().ToList();
        Level = 5;

        foreach (var character in characters) {
            battlefield.RegisterCharacterToBattle(character); //remove this
            character.SetHealth(Calculator.GetStat(character.stats.maximumHealth, Level, true));
            character.SetMP(Calculator.GetStat(character.stats.maximumMP, Level, true));
        }
    }

    public void GainExperience(int amount) {
        experiencePoints += amount;

        if (experiencePoints >= expNeededForLevel) {
            int leftovers = experiencePoints - expNeededForLevel;
            experiencePoints = leftovers;
            Level++;
        }
    }
}
