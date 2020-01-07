using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New LevelSet", menuName = "Level/New LevelSet")]
public class GeneratedLevelSet : ScriptableObject {

    [Header("General")]
    public GameObject groundPrefab;
    public float groundOffset;

    [Header("Decoration")]
    public List<LevelLayer> foreground;
    public List<LevelLayer> antiVoid;
    public List<LevelLayer> background;
    public List<LevelLayer> guaranteedSpawns;
}


[System.Serializable]
public class LevelLayer {
    public List<GameObject> prefabs;
    public Vector3 origin;
    public float offset;
    public int amountPositive;
    public int amountNegative;
    public bool scaleWithPositive = false;
}