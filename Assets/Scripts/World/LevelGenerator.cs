using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LevelGenerator : MonoBehaviourSingleton<LevelGenerator> {

    [SerializeField] private GameObject emptyPrefab;
    [SerializeField] private Area areaObject;
    [SerializeField] private int maximumSetsSpawned;
    [SerializeField] private int generateNewOnIndex;

    private List<LevelSet> sets = new List<LevelSet>();
    private int maxSets;
    private int maxEncounters;
    private int generatedSets;
    private int[] encounters;
    private LevelTypeRange[] levelTypeRanges;
    private bool hasSpawnedCharacter = false;

    private void Awake() {
        maxSets = Random.Range(areaObject.minimumSets, areaObject.maximumSets + 1);
        maxEncounters = Random.Range(areaObject.minimumEncounters, areaObject.maximumEncounters + 1);
        generatedSets = 0;
        DecideEncounters();

        BattleSystem.Instance.PlayerParty.ResetCharacters();
        CreateLevelTypeRanges();

        for (int i = 0; i < maximumSetsSpawned; i++) {
            Generate();
        }
    }

    private void CreateLevelTypeRanges() {
        levelTypeRanges = new LevelTypeRange[areaObject.encounters.Count];
        for (int i = 0; i < areaObject.encounters.Count; i++) {

            if (areaObject.encounters[i].type == LevelType.Character && BattleSystem.Instance.PlayerParty.characters.Count >= 4) {
                continue;
            }

            levelTypeRanges[i] = new LevelTypeRange() {
                Type = areaObject.encounters[i].type,
                Weight = areaObject.encounters[i].probability
            };
        }
    }

    private void DecideEncounters() {
        encounters = new int[maxEncounters];

        for (int i = 0; i < maxEncounters; i++) {
            int encounter = Random.Range(2, maxSets);

            while (encounters.Contains(encounter)) {
                encounter++;
                if (encounter > maxSets) {
                    encounter = 2;
                }
            }

            encounters[i] = encounter;
        }
    }

    public void GenerateTrigger(LevelSet set) {
        int setIndex = sets.IndexOf(set);

        if (setIndex == generateNewOnIndex) {
            LevelSet indexZero = sets[0];
            sets.RemoveAt(0);
            Destroy(indexZero.gameObject);

            Generate();
        }
    }

    public void OnSetEnter(LevelSet set) {
        Game.Instance.CurrentLevelSet = set;

        if (set.Type != LevelType.Normal) {
            Game.Instance.SetEncounter();
        }
    }

    private void Generate() {
        if (generatedSets >= maxSets) return; //GENERATE EXIT

        generatedSets++;

        GameObject go = Instantiate(emptyPrefab, transform);
        LevelSet set = go.GetComponent<LevelSet>();
        GeneratedLevelSet setObject = GetLevelSetObject(set);
        
        GameObject ground = Instantiate(setObject.groundPrefab, set.transform);

        List<LevelLayer> layers = new List<LevelLayer>();
        layers.AddRange(setObject.foreground);
        layers.AddRange(setObject.antiVoid);
        layers.AddRange(setObject.background);
        layers.AddRange(setObject.guaranteedSpawns);

        foreach (LevelLayer layer in layers) {
            Transform parent = null;

            if (setObject.foreground.Any(x => x == layer)) parent = set.foreground.transform;
            else if (setObject.antiVoid.Any(x => x == layer)) parent = set.antiVoid.transform;
            else if (setObject.background.Any(x => x == layer)) parent = set.background.transform;
            else parent = set.transform;

            for (int i = layer.amountNegative * -1; i <= layer.amountPositive; i++) {
                GameObject obj = Instantiate(layer.prefabs.GetRandom(), parent);
                obj.transform.localPosition = new Vector3(layer.origin.x + (layer.offset * i), layer.origin.y, layer.origin.z);
            }

            if (layer.scaleWithPositive && sets.Count > 0) {
                parent.Translate(new Vector3(layer.amountPositive * setObject.groundOffset * sets.Count, 0, 0), Space.Self);
            }
        }

        sets.Add(set);

        int setIndex = sets.IndexOf(set);

        if (setIndex != 0) {
            Transform previousSet = sets[setIndex - 1].transform;
            set.transform.localPosition = new Vector3(previousSet.localPosition.x + setObject.groundOffset, previousSet.localPosition.y, previousSet.localPosition.z);
        }
    }

    private GeneratedLevelSet GetLevelSetObject(LevelSet set) {
        LevelType type = LevelType.Normal;
        GeneratedLevelSet levelSet = areaObject.normalLevel.levelSetObject.GetRandom();

        if (encounters.Contains(generatedSets)) {
            if (hasSpawnedCharacter == false && System.Array.IndexOf(encounters, generatedSets) == encounters.Length - 1 && BattleSystem.Instance.PlayerParty.characters.Count < 4) {
                type = LevelType.Character;
            }
            else {
                type = Probability.Range(levelTypeRanges);
            }
          
            levelSet = areaObject.encounters.Single(x => x.type == type).levelSetObject.GetRandom();

            CreateLevelTypeRanges();
        }

        if (type == LevelType.Character) {
            hasSpawnedCharacter = true;
        }

        set.Type = type;
        return levelSet;
    }

    public T RandomEnumValue<T>() {
        var v = System.Enum.GetValues(typeof(T));
        return (T)v.GetValue(Random.Range(1, v.Length));
    }
}
