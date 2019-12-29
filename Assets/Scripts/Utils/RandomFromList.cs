using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomFromList {
   
    public static T Get<T>(List<T> list) {
        int index = Random.Range(0, list.Count - 1);
        return list[index];
    }

}
