using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class UIFiller : MonoBehaviour {

    [Header("Party")]
    [SerializeField] private Party party;

    [Header("UI Elements: Actions")]
    [SerializeField] private TextMeshProUGUI characterName;
    [SerializeField] private TextMeshProUGUI firstSpell;
    [SerializeField] private TextMeshProUGUI secondSpell;
    [SerializeField] private TextMeshProUGUI spellCost;

    [Header("UI Elements: Health / MP")]
    [SerializeField] private List<CharacterSlot> slots;

    private CharacterStats stats;

    private readonly string hpText = " HP";
    private readonly string mpText = " MP";

    void Start() {
        FillCurrentTurn();
        FillWithStats();
    }

    public void FillWithStats() {
        for (int i = 0; i < party.characters.Count; i++) {
            if (i >= slots.Count) break;

            CharacterSlot slot = slots[i];
            CharacterBase character = party.characters[i];

            slot.AssignedCharacter = character;
            slot.characterName.text = character.stats.characterName;
            UpdateHealth(slot);
            UpdateMana(slot);
        }
    }

    public void UpdateHealth(CharacterSlot slot) {
        Image image = slot.healthImage;
        float fill = (float)slot.AssignedCharacter.CurrentHealth / (float)Calculator.GetStat(slot.AssignedCharacter.stats.maximumHealth, party.Level, true);
        slot.healthText.text = slot.AssignedCharacter.CurrentHealth.ToString() + hpText;
        image.DOFillAmount(fill, 0.75f);
    }

    public void UpdateMana(CharacterSlot slot) {
        Image image = slot.manaImage;
        float fill = (float)slot.AssignedCharacter.CurrentMP / (float)Calculator.GetStat(slot.AssignedCharacter.stats.maximumMP, party.Level, true);
        slot.manaText.text = slot.AssignedCharacter.CurrentMP.ToString() + mpText;
        image.DOFillAmount(fill, 0.75f);
    }

    public void FillCurrentTurn() {
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


[System.Serializable]
public class CharacterSlot {
    public TextMeshProUGUI characterName;
    public Image healthImage;
    public TextMeshProUGUI healthText;
    public Image manaImage;
    public TextMeshProUGUI manaText;
    public CharacterBase AssignedCharacter { get; set; }
}