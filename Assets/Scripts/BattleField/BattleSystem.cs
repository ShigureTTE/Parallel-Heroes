using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using DG.Tweening;

public class BattleSystem : MonoBehaviour {

    [Header("Parties")]
    [SerializeField] private Party playerParty;
    [SerializeField] private Party enemyParty;

    [Header("UI")]
    [SerializeField] private UIFiller filler;

    [Header("Enemy Arrow")]
    [SerializeField] private Transform arrow;
    [SerializeField] private Light arrowLight;
    [SerializeField] private float offset;
    [SerializeField] private float lightIntensity;

    [HideInInspector] public List<CharacterBase> player = new List<CharacterBase>();
    [HideInInspector] public List<CharacterBase> enemy = new List<CharacterBase>();

    private MoveCharacterToLane laneMover;
    private List<CharacterBase> turnOrder;
    private EnemySpawner enemySpawner;

    public CharacterBase CurrentTurn { get; private set; }

    private void Start() {
        laneMover = GetComponent<MoveCharacterToLane>();
        enemySpawner = GetComponent<EnemySpawner>();
        enemySpawner.SpawnNewFormation();
        NewBattle();
    }

    public void NewBattle() {
        List<CharacterBase> characters = new List<CharacterBase>();
        playerParty.ResetCharacters();
        enemyParty.ResetCharacters();

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

    public void ShowArrow(TextMeshProUGUI textObject) {
        string text = textObject.text;

        GameObject go = null;
        go = enemyParty.characters.Single(x => x.gameObject.name == text).gameObject;

        Bounds spriteBounds = go.GetComponentInChildren<SpriteRenderer>().bounds;
        arrow.transform.position = new Vector3(go.transform.position.x, go.transform.position.y + spriteBounds.size.y / 2 + offset, go.transform.position.z);

        arrow.GetComponent<Tweener>().PlayTween();
        arrowLight.DOIntensity(lightIntensity, .25f);
    }

    public void HideArrow() {
        arrow.GetComponent<Tweener>().PlayTweenReversed();
        arrowLight.DOIntensity(0, .25f);
    }

    private void RefreshLists() {
        player = new List<CharacterBase>();
        enemy = new List<CharacterBase>();
    }
}