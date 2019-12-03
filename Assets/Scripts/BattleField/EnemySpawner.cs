using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {

    [SerializeField] private BattleFormation formation;
    [SerializeField] private Party enemyParty;

    ObjectPooler pooler;

    private readonly string letters = "ABCDEFGHIJK";

    private void Awake() {
        pooler = ObjectPooler.Instance;
    }

    public void SpawnNewFormation() {
        enemyParty.ForceLevel(formation);

        for (int i = 0; i < formation.enemies.Count; i++) {
            GameObject go = pooler.SpawnFromPool(formation.enemies[i].enemy.tag, enemyParty.transform, Vector3.zero, Quaternion.identity);
            go.GetComponent<CharacterBase>().Lane = formation.enemies[i].lane;
            go.name = formation.enemies[i].enemy.prefab.GetComponent<CharacterBase>().stats.characterName + " " + letters[i];
        }
    }

}
