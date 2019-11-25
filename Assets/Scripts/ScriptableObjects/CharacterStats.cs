using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "Characters/New Character")]
public class CharacterStats : ScriptableObject {
    public string tag;
    public GameObject prefab;
    public int size;
}
