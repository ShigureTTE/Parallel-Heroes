using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBase : MonoBehaviour {
    [SerializeField] private Faction faction;
    public Faction Faction { get { return faction; } }
    [SerializeField] private Lane currentLane;
    public Lane Lane { get { return currentLane; } set { currentLane = value; } }
}
