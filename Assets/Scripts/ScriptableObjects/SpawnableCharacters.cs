using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterList", menuName = "New Character List")]
public class SpawnableCharacters : ScriptableObject {
    public List<CharacterBase> characters;
}
