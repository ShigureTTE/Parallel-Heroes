using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewObjectPool", menuName = "Objects/Pool")]
public class ObjectPool : ScriptableObject {
    public string tag;
    public GameObject prefab;
    public int size;
}
