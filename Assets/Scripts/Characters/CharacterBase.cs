﻿using System.Collections;
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
    public Tweener RangeTypeTweener { get; private set; }
    public SpriteRenderer RangeSprite { get { return RangeTypeTweener.GetComponent<SpriteRenderer>(); } }

    /// <summary>
    /// Already gone trough Calculator
    /// </summary>
    public int CurrentHealth { get; private set; }
    /// <summary>
    /// Already gone trough Calculator
    /// </summary>
    public int CurrentMP { get; private set; }

    private void Awake() {
        RangeTypeTweener = GetComponentInChildren<Tweener>();
    }

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
