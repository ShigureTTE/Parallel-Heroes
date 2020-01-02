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
    public GeneratedLevelSet normalLevel;
    public GeneratedLevelSet battle;
    public GeneratedLevelSet exit;
    public GeneratedLevelSet branchingPath;
    public GeneratedLevelSet safeZone;
    public GeneratedLevelSet treasure;
    public GeneratedLevelSet magicFountain;
    public GeneratedLevelSet storageRoom;
    public GeneratedLevelSet shop;
    public GeneratedLevelSet character;
    public List<GeneratedLevelSet> traps;
}
