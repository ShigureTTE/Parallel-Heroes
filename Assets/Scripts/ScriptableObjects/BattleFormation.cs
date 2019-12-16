using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Formation", menuName = "Battle/New Formation")]
public class BattleFormation : ScriptableObject {

    [Header("Party / AI Settings")]
    public int level;

    [Header("Enemies")]
    public List<EnemyFormation> enemies;
}

[System.Serializable]
public struct EnemyFormation {
    public ObjectPool enemy;
    public Lane lane;
}