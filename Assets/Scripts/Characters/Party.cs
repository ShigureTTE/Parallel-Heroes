using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Party : MonoBehaviour {

    [SerializeField] private BattleSystem battlefield;

    [SerializeField] private Faction faction;
    [SerializeField] private int expNeededForLevel = 100;

    public CharacterBase CurrentTurn { get { return battlefield.CurrentTurn; } }
    [HideInInspector] public List<CharacterBase> characters;

    public int Level { get; private set; }
    public int Gold { get; private set; }
    private int experiencePoints = 0;

    private void Awake() {       
        Level = 5; //TODO: remove hard coded level setter
        ResetCharacters();
    }

    public void ResetCharacters() {
        characters = new List<CharacterBase>();
        characters = GetComponentsInChildren<CharacterBase>().ToList();
        foreach (CharacterBase character in characters) {
            character.Party = this;
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

    public void GainGold(int amount) {
        Gold += amount;
    }

    public void ForceLevel(int level) {
        Level = level;
    }
}
