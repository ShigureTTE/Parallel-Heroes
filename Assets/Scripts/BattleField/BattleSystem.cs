using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BattleSystem : MonoBehaviour {

    [Header("Parties")]
    [SerializeField] private Party playerParty;
    [SerializeField] private Party enemyParty;

    [Header("UI")]
    [SerializeField] private UIFiller filler;

    [HideInInspector] public List<CharacterBase> player = new List<CharacterBase>();
    [HideInInspector] public List<CharacterBase> enemy = new List<CharacterBase>();

    private MoveCharacterToLane laneMover;
    private List<CharacterBase> turnOrder;

    public CharacterBase CurrentTurn { get; private set; }

    private void Start() {
        laneMover = GetComponent<MoveCharacterToLane>();
        NewBattle();
    }

    public void NewBattle() {
        List<CharacterBase> characters = new List<CharacterBase>();
        characters.AddRange(playerParty.characters);
        characters.AddRange(enemyParty.characters);

        foreach (CharacterBase character in characters) {
            AddCharacter(character);

            int level = character.Faction == Faction.Player ? playerParty.Level : enemyParty.Level;

            character.SetHealth(Calculator.GetStat(character.stats.maximumHealth, level, true));
            character.SetMP(Calculator.GetStat(character.stats.maximumMP, level, true));
        }

        turnOrder = new List<CharacterBase>();
        turnOrder = characters.OrderByDescending(x => Calculator.GetStat(x.stats.speed, x.Faction == Faction.Player ? playerParty.Level : enemyParty.Level)).ToList();
        CurrentTurn = turnOrder[0];

        filler.FillAll();
    }

    public void AddCharacter(CharacterBase character) {
        if (character.Faction == Faction.Player) player.Add(character);
        else enemy.Add(character);

        laneMover.SetCharacterToLane(character, character.Lane, character.Faction == Faction.Player ? player : enemy);
    }

    private void RefreshLists() {
        player = new List<CharacterBase>();
        enemy = new List<CharacterBase>();
    }
}