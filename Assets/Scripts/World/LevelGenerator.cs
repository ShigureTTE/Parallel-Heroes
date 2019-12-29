using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LevelGenerator : MonoBehaviourSingleton<LevelGenerator> {

    [SerializeField] private GameObject emptyPrefab;
    [SerializeField] private GeneratedLevelSet setObject;
    [SerializeField] private int maximumSetsSpawned;
    [SerializeField] private int generateNewOnIndex;

    private List<LevelSet> generatedSets = new List<LevelSet>();

    private void Awake() {
        for (int i = 0; i < maximumSetsSpawned; i++) {
            Generate();
        }
    }

    public void TriggerCalled(LevelSet set) {
        int setIndex = generatedSets.IndexOf(set);
        Debug.Log("Index: " + setIndex);

        if (setIndex == generateNewOnIndex) {
            LevelSet indexZero = generatedSets[0];
            generatedSets.RemoveAt(0);
            Destroy(indexZero.gameObject);

            Generate();
        }
    }

    private void Generate() {
        GameObject go = Instantiate(emptyPrefab, transform);
        LevelSet set = go.GetComponent<LevelSet>();

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

            if (layer.scaleWithPositive && generatedSets.Count > 0) {
                parent.Translate(new Vector3(layer.amountPositive * setObject.groundOffset * generatedSets.Count, 0, 0), Space.Self);
            }
        }

        generatedSets.Add(set);

        int setIndex = generatedSets.IndexOf(set);

        if (setIndex != 0) {
            Transform previousSet = generatedSets[setIndex - 1].transform;
            set.transform.localPosition = new Vector3(previousSet.localPosition.x + setObject.groundOffset, previousSet.localPosition.y, previousSet.localPosition.z);
        }
    }
}
