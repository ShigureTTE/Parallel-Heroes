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
    [SerializeField] private Party enemyParty;

    [Header("Battle System")]
    [SerializeField] private BattleSystem battleSystem;

    [Header("UI Elements: Actions")]
    [SerializeField] private TextMeshProUGUI characterName;
    [SerializeField] private TextMeshProUGUI firstSpell;
    [SerializeField] private TextMeshProUGUI secondSpell;
    [SerializeField] private TextMeshProUGUI spellCost;

    [Header("UI Elements: Spell Buttons")]
    [SerializeField] private List<Button> spells;
    [SerializeField] private Button spellButton;

    [Header("UI Elements: Health / MP / Level")]
    [SerializeField] private List<CharacterSlot> slots;
    [SerializeField] private TextMeshProUGUI level;

    [Header("UI Elements: Health / MP")]
    [SerializeField] private List<TextMeshProUGUI> enemySlots = new List<TextMeshProUGUI>();

    [Header("UI Elements: Lane Buttons")]
    [SerializeField] private Button closeRange;
    [SerializeField] private Button midRange;
    [SerializeField] private Button longRange;

    [Header("UI Elements: Party Menu")]
    [SerializeField] private List<TextMeshProUGUI> partySlots;
    [SerializeField] private StatScreenFiller statScreen;

    [Header("Remove Character Settings")]
    [SerializeField] private float fadeSpeed;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float moveAmount;
    [SerializeField] private Ease moveEase;

    private Party playerParty;
    private CharacterStats stats;

    private readonly string hpText = " HP";
    private readonly string mpText = " MP";
    private readonly string letters = "ABCDEFGHIJK";

    private void Start() {
        playerParty = Game.Instance.PlayerParty;
    }

    public void FillAll() {
        FillCurrentTurn();
        FillWithStats();
    }

    public void SetInteractableLanes() {
        if (Calculator.GetAvailableTargets(Lane.Close, enemyParty.characters).Count == 0) closeRange.interactable = false;
        if (Calculator.GetAvailableTargets(Lane.Mid, enemyParty.characters).Count == 0) midRange.interactable = false;
        if (Calculator.GetAvailableTargets(Lane.Long, enemyParty.characters).Count == 0) longRange.interactable = false;
    }

    public void SetInteractableSpells() {
        CharacterBase currentTurn = battleSystem.CurrentTurn;

        int counter = 0;
        for (int i = currentTurn.stats.spells.Length - 1; i >= 0; i--) {
            if (currentTurn.stats.spells[i].mPCost <= currentTurn.CurrentMP) {
                spells[i].interactable = true;
            }
            else {
                spells[i].interactable = false;
                counter++;
            }
        }

        if (counter < currentTurn.stats.spells.Length) {
            spellButton.interactable = true;
        }
        else {
            spellButton.interactable = false;
        }
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

    public void FillWithStats(bool snap = false) {
        if (playerParty == null) playerParty = Game.Instance.PlayerParty;

        for (int i = 0; i < slots.Count; i++) {
            if (i >= playerParty.characters.Count) {
                slots[i].slotGroup.alpha = 0;
                slots[i].AssignedCharacter = null;
                continue;
            }

            slots[i].slotGroup.alpha = 1;

            CharacterSlot slot = slots[i];
            CharacterBase character = playerParty.characters[i];

            slot.AssignedCharacter = character;
            slot.characterName.text = character.stats.characterName;
            UpdateHealth(slot, snap);
            UpdateMana(slot, snap);
        }

        level.text = playerParty.Level.ToString();
    }

    public void UpdateHealth(CharacterSlot slot, bool snap = false) {
        Image image = slot.healthImage;
        float fill = (float)slot.AssignedCharacter.CurrentHealth / (float)Calculator.GetStat(slot.AssignedCharacter.stats.maximumHealth, playerParty.Level, true);
        slot.healthText.text = slot.AssignedCharacter.CurrentHealth.ToString() + hpText;
        if (snap) {
            image.fillAmount = fill;
        }
        else {
            image.DOFillAmount(fill, 0.75f);
        }
    }

    public void UpdateMana(CharacterSlot slot, bool snap = false) {
        Image image = slot.manaImage;
        float fill = (float)slot.AssignedCharacter.CurrentMP / (float)Calculator.GetStat(slot.AssignedCharacter.stats.maximumMP, playerParty.Level, true);
        slot.manaText.text = slot.AssignedCharacter.CurrentMP.ToString() + mpText;
        if (snap) {
            image.fillAmount = fill;
        }
        else {
            image.DOFillAmount(fill, 0.75f);
        }
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

    public void FillPartyMenu() {
        for (int i = 0; i < partySlots.Count; i++) {
            if (i > playerParty.characters.Count - 1) {
                partySlots[i].text = "-";
                partySlots[i].GetComponent<Button>().interactable = false;
                continue;
            }

            partySlots[i].text = playerParty.characters[i].stats.characterName;
            partySlots[i].GetComponent<Button>().interactable = true;
        }
    }

    public void FillStatMenu(int id) {
        foreach (CharacterBase character in playerParty.characters) {
            character.SelectedEffect.Stop();
        }

        playerParty.characters[id].SelectedEffect.Play();
        statScreen.Fill(playerParty.characters[id], playerParty.Level);
    }

    public void ClearSelected() {
        foreach (CharacterBase characterBase in playerParty.characters) {
            characterBase.SelectedEffect.Stop();
        }
    }

    public IEnumerator RemoveDeadCharacter(CharacterBase character) {
        CharacterSlot slot = slots.Single(x => x.AssignedCharacter == character);

        if (slot != null) {
            Tween slotAlpha = slot.slotGroup.DOFade(0, fadeSpeed);
            int slotIndex = slots.IndexOf(slot);

            List<CharacterSlot> slotsBelow = new List<CharacterSlot>();
            for (int i = slotIndex + 1; i < slots.Count; i++) {
                slotsBelow.Add(slots[i]);
            }

            Tween moveUp = null;
            foreach (CharacterSlot characterSlot in slotsBelow) {
                moveUp = characterSlot.slotGroup.transform.DOLocalMoveY(characterSlot.slotGroup.transform.localPosition.y + moveAmount, moveSpeed).SetEase(moveEase);
            }

            if (moveUp != null) {
                yield return moveUp.WaitForCompletion();
            }
            else {
                yield return slotAlpha.WaitForCompletion();
            }
                
            if (slotsBelow.Count > 1) {
                foreach (CharacterSlot characterSlot in slotsBelow) {
                    RectTransform rt = (RectTransform)characterSlot.slotGroup.transform;
                    rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, rt.anchoredPosition.y - moveAmount);
                }
            }

            if (slotsBelow.Any(x => x.AssignedCharacter != null)) {
                slot.slotGroup.alpha = 1;
            }
            FillWithStats(true);
                                          
        }
    }
}


[System.Serializable]
public class CharacterSlot {
    public TextMeshProUGUI characterName;
    public Image healthImage;
    public TextMeshProUGUI healthText;
    public Image manaImage;
    public TextMeshProUGUI manaText;
    public CanvasGroup slotGroup;
    public CharacterBase AssignedCharacter { get; set; }
}