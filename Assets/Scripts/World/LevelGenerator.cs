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

    private void Awake() {
        maxSets = Random.Range(areaObject.minimumSets, areaObject.maximumSets + 1);
        maxEncounters = Random.Range(areaObject.minimumEncounters, areaObject.maximumEncounters + 1);
        generatedSets = 0;
        DecideEncounters();

        for (int i = 0; i < maximumSetsSpawned; i++) {
            Generate();
        }
    }

    private void DecideEncounters() {
        encounters = new int[maxEncounters];

        for (int i = 0; i < maxEncounters; i++) {
            int encounter = Random.Range(2, maxSets);

            while (encounters.Contains(encounter)) {
                encounter++;
                if (encounter > maxSets) {
                    encounter = 0;
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

        foreach (LevelLayer layer in layers) {
            Transform parent = null;

            if (setObject.foreground.Any(x => x == layer)) parent = set.foreground.transform;
            else if (setObject.antiVoid.Any(x => x == layer)) parent = set.antiVoid.transform;
            else if (setObject.background.Any(x => x == layer)) parent = set.background.transform;

            for (int i = layer.amountNegative * -1; i <= layer.amountPositive; i++) {
                GameObject obj = Instantiate(RandomFromList.Get(layer.prefabs), parent);
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
        GeneratedLevelSet levelSet = areaObject.normalLevel;

        if (encounters.Contains(generatedSets)) {
            //GENERATE AN ENCOUNTER. ONLY BATTLES FOR NOW.

            type = LevelType.Battle;

            switch (type) {
                case LevelType.Battle:
                    levelSet = areaObject.battle;
                    break;
                case LevelType.Exit:
                    levelSet = areaObject.exit;
                    break;
                case LevelType.BranchingPath:
                    levelSet = areaObject.branchingPath;
                    break;
                case LevelType.SafeZone:
                    levelSet = areaObject.safeZone;
                    break;
                case LevelType.TreasureChest:
                    levelSet = areaObject.treasure;
                    break;
                case LevelType.Mimic:
                    levelSet = areaObject.treasure;
                    break;
                case LevelType.MagicFountain:
                    levelSet = areaObject.magicFountain;
                    break;
                case LevelType.StorageRoom:
                    levelSet = areaObject.storageRoom;
                    break;
                case LevelType.Shop:
                    levelSet = areaObject.shop;
                    break;
                case LevelType.Character:
                    levelSet = areaObject.character;
                    break;
                case LevelType.Trap:
                    levelSet = RandomFromList.Get(areaObject.traps);
                    break;
                default:
                    break;
            }
        }

        set.Type = type;
        return levelSet;
    }
}
