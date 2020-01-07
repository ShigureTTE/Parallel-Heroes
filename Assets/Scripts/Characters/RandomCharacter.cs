using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomCharacter : MonoBehaviour {

    public GameObject yes;

    void Awake() {
        Instantiate(yes, transform);
        
    }


}
