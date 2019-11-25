using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIFiller : MonoBehaviour {

    [Header("Party")]
    [SerializeField] private Party party;

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI characterName;
    [SerializeField] private TextMeshProUGUI firstSpell;
    [SerializeField] private TextMeshProUGUI secondSpell;
    [SerializeField] private TextMeshProUGUI spellCost;

    private CharacterStats stats;

    void Start() {
        FillWithStats();
    }

    public void FillWithStats() {
        if (party != null) stats = party.currentTurn.stats;

        characterName.text = stats.characterName;
        firstSpell.text = stats.spells[0].attackName;
        secondSpell.text = stats.spells[1].attackName;
        spellCost.text = stats.spells[0].mPCost.ToString();
    }

    public void UpdateSpellCost(int index) {
        spellCost.text = stats.spells[index].mPCost.ToString();
    }
}
