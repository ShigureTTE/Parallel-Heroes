using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBase : MonoBehaviour {
    [SerializeField] private Faction faction;
    public Faction Faction { get { return faction; } }
    [SerializeField] private Lane currentLane;
    public Lane Lane { get { return currentLane; } set { currentLane = value; } }
    public CharacterStats stats;

    public Party Party { get; set; }
    public bool IsBlocking { get; set; }

    [SerializeField] private SpriteRenderer rangeSprite;
    public SpriteRenderer RangeSprite { get { return rangeSprite; } }
    [SerializeField] private ParticleSystem selectedEffect;
    public ParticleSystem SelectedEffect { get { return selectedEffect; } }

    /// <summary>
    /// Already gone trough Calculator
    /// </summary>
    public int CurrentHealth { get; private set; }
    /// <summary>
    /// Already gone trough Calculator
    /// </summary>
    public int CurrentMP { get; private set; }

    public void SetHealth(int health) {
        CurrentHealth = health;
    }

    public void SetMP(int mana) {
        CurrentMP = mana;
    }

    public void SubstractHealth(int damage) {
        CurrentHealth -= damage;
    }

    public void SubstractMP(int manaCost) {
        CurrentMP -= manaCost;
    }
}
