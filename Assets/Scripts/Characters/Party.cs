using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Party : MonoBehaviour {

    [SerializeField] private MoveCharacterToLane battlefield;
    private List<CharacterBase> characters;

    private void Start() {
        characters = GetComponentsInChildren<CharacterBase>().ToList();

        foreach (var character in characters) {
            battlefield.RegisterCharacterToBattle(character);
        }
    }
}
