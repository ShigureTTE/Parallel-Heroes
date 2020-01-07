using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomCharacter : MonoBehaviour {

    public SpawnableCharacters sc;
    public bool onAwake = false;

    void Awake() {
        if (onAwake) {
            SpawnCharacter();
        }        
    }

    public void SpawnCharacter() {

        BattleSystem.Instance.PlayerParty.ResetCharacters();
        CharacterBase character = sc.characters.GetUniqueCharacter(BattleSystem.Instance.PlayerParty.characters);

        GameObject go = Instantiate(character.gameObject, this.transform);
        go.transform.localPosition = Vector3.zero;
    }
}
