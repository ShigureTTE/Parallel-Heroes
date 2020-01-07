using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Area", menuName = "Level/New Area")]
public class Area : ScriptableObject {

    [Header("Area Settings")]
    public int minimumSets;
    public int maximumSets;

    [Header("Encounter Settings")]
    public int minimumEncounters;
    public int maximumEncounters;

    [Header("Level Sets")]
    public EncounterSet normalLevel;
    public List<EncounterSet> encounters;
}

[System.Serializable]
public class EncounterSet {
    public List<GeneratedLevelSet> levelSetObject;
    public LevelType type;
    [Range(0, 100)]public int probability = 0;
}