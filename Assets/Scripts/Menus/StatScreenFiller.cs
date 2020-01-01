using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StatScreenFiller : MonoBehaviour {

    [Header("Stats")]
    [SerializeField] private TextMeshProUGUI characterName;
    [SerializeField] private TextMeshProUGUI role;
    [SerializeField] private TextMeshProUGUI weapon;
    [SerializeField] private TextMeshProUGUI currentHP;
    [SerializeField] private TextMeshProUGUI maxHP;
    [SerializeField] private TextMeshProUGUI currentMP;
    [SerializeField] private TextMeshProUGUI maxMP;
    [SerializeField] private TextMeshProUGUI attack;
    [SerializeField] private TextMeshProUGUI defense;
    [SerializeField] private TextMeshProUGUI resistance;
    [SerializeField] private TextMeshProUGUI skill;
    [SerializeField] private TextMeshProUGUI speed;
    [SerializeField] private TextMeshProUGUI description;

    [Header("Attacks")]
    [SerializeField] private StatScreenAttack normalAttack;
    [SerializeField] private StatScreenAttack firstSpell;
    [SerializeField] private StatScreenAttack secondSpell;

    private const string normalAttackSTring = "Normal Attack";

    public void Fill(CharacterStats character) {
        Debug.Log(character.characterName);
    }
}

[System.Serializable]
public class StatScreenAttack {
    [SerializeField] private TextMeshProUGUI attackName;
    [SerializeField] private TextMeshProUGUI spellCost;
    [SerializeField] private TextMeshProUGUI power;
    [SerializeField] private TextMeshProUGUI element;
    [SerializeField] private TextMeshProUGUI prefLane;
    [SerializeField] private TextMeshProUGUI type;
}