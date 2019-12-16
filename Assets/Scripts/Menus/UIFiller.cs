using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;
using System;

public class UIFiller : MonoBehaviour {

    [Header("Party")]
    [SerializeField] private Party playerParty;
    [SerializeField] private Party enemyParty;

    [Header("Battle System")]
    [SerializeField] private BattleSystem battleSystem;

    [Header("UI Elements: Actions")]
    [SerializeField] private TextMeshProUGUI characterName;
    [SerializeField] private TextMeshProUGUI firstSpell;
    [SerializeField] private TextMeshProUGUI secondSpell;
    [SerializeField] private TextMeshProUGUI spellCost;

    [Header("UI Elements: Health / MP / Level")]
    [SerializeField] private List<CharacterSlot> slots;
    [SerializeField] private TextMeshProUGUI level;

    [Header("UI Elements: Health / MP")]
    [SerializeField] private List<TextMeshProUGUI> enemySlots = new List<TextMeshProUGUI>();

    [Header("UI Elements: Lane Buttons")]
    [SerializeField] private Button closeRange;
    [SerializeField] private Button midRange;
    [SerializeField] private Button longRange;

    private CharacterStats stats;

    private readonly string hpText = " HP";
    private readonly string mpText = " MP";
    private readonly string letters = "ABCDEFGHIJK";

    public void FillAll() {
        FillCurrentTurn();
        FillWithStats();
    }

    public void SetInteractableLanes() {
        if (Calculator.GetAvailableTargets(Lane.Close, enemyParty.characters).Count == 0) closeRange.interactable = false;
        if (Calculator.GetAvailableTargets(Lane.Mid, enemyParty.characters).Count == 0) midRange.interactable = false;
        if (Calculator.GetAvailableTargets(Lane.Long, enemyParty.characters).Count == 0) longRange.interactable = false;
    }

    public void FillEnemies() {
        foreach (var item in enemySlots) {
            item.GetComponent<Button>().interactable = false;
            item.text = "";
        }

        List<CharacterBase> targets = Calculator.GetAvailableTargets(battleSystem.CurrentTurn.Lane, enemyParty.characters);
        for (int i = 0; i < targets.Count; i++) {
            if (i >= enemySlots.Count) break;

            TextMeshProUGUI tmp = enemySlots[i];
            CharacterBase character = targets[i];

            tmp.text = character.gameObject.name;
            tmp.GetComponent<Button>().interactable = true;
        }
    }

    public void FillWithStats() {
        for (int i = 0; i < playerParty.characters.Count; i++) {
            if (i >= slots.Count) break;
            if (playerParty.characters[i].IsDead) continue;

            CharacterSlot slot = slots[i];
            CharacterBase character = playerParty.characters[i];

            slot.AssignedCharacter = character;
            slot.characterName.text = character.stats.characterName;
            UpdateHealth(slot);
            UpdateMana(slot);
        }

        level.text = playerParty.Level.ToString();
    }

    public void UpdateHealth(CharacterSlot slot) {
        Image image = slot.healthImage;
        float fill = (float)slot.AssignedCharacter.CurrentHealth / (float)Calculator.GetStat(slot.AssignedCharacter.stats.maximumHealth, playerParty.Level, true);
        slot.healthText.text = slot.AssignedCharacter.CurrentHealth.ToString() + hpText;
        image.DOFillAmount(fill, 0.75f);
    }

    public void UpdateMana(CharacterSlot slot) {
        Image image = slot.manaImage;
        float fill = (float)slot.AssignedCharacter.CurrentMP / (float)Calculator.GetStat(slot.AssignedCharacter.stats.maximumMP, playerParty.Level, true);
        slot.manaText.text = slot.AssignedCharacter.CurrentMP.ToString() + mpText;
        image.DOFillAmount(fill, 0.75f);
    }

    public void FillCurrentTurn() {
        if (battleSystem != null) stats = battleSystem.CurrentTurn.stats;

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