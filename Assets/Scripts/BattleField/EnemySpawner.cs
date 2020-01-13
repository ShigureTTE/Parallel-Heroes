using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {

    [SerializeField] private Area currentArea;
    [SerializeField] private Party enemyParty;

    private ObjectPooler pooler;
    private BattleFormation formation;

    private readonly string letters = "ABCDEFGHIJK";

    private void Awake() {
        pooler = ObjectPooler.Instance;
        formation = currentArea.battleFormations.GetRandom();
    }

    public void SpawnNewFormation(Vector3 spawnLocation = new Vector3()) {
        formation = currentArea.battleFormations.GetRandom();

        int level = Game.Instance.PlayerParty.Level - formation.levelLowerThanPlayer;
        if (level <= 0) level = 1;
        enemyParty.ForceLevel(level);

        for (int i = 0; i < formation.enemies.Count; i++) {
            if (formation.enemies[i].minimumPlayerCharacters > Game.Instance.PlayerParty.characters.Count) continue;

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

    public void SetArea(Area area) {
        currentArea = area;
    }
}
