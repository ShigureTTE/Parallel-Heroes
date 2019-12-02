using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Formation", menuName = "Battle/New Formation")]
public class BattleFormation : ScriptableObject {

    [Header("Party Settings")]
    public int level;
    public List<EnemyFormation> enemies;
}

[System.Serializable]
public struct EnemyFormation {
    public ObjectPool enemy;
    public Lane lane;
}