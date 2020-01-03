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

    public void SpawnNewFormation(Vector3 spawnLocation = new Vector3()) {
        enemyParty.ForceLevel(formation);

        for (int i = 0; i < formation.enemies.Count; i++) {
            GameObject go = pooler.SpawnFromPool(formation.enemies[i].enemy.tag, enemyParty.transform, spawnLocation, Quaternion.identity);
            CharacterBase cb = go.GetComponent<CharacterBase>();
            cb.Lane = formation.enemies[i].lane;
            cb.IsDead = false;
            cb.IsBlocking = false;
            go.GetComponent<Transform>().localScale = Vector3.one;
            go.name = formation.enemies[i].enemy.prefab.GetComponent<CharacterBase>().stats.characterName + " " + letters[i];
        }
    }

    public int GetCoins() {
        return Random.Range(formation.minCoins, formation.maxCoins + 1);
    }

    public int GetExperience() {
        return formation.experience;
    }

}
