using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviourSingleton<ObjectPooler> {

    public Dictionary<string, Queue<GameObject>> poolDictionary;
    public List<ObjectPool> pools;

    void Awake() {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (ObjectPool pool in pools) {
            Queue <GameObject> objects = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++) {
                GameObject obj = Instantiate(pool.prefab, transform);
                obj.SetActive(false);
                objects.Enqueue(obj);
            }

            poolDictionary.Add(pool.tag, objects);
        }
    }

    public GameObject SpawnFromPool(string tag, Transform parent, Vector3 position, Quaternion rotation) {
        try {
            GameObject obj = poolDictionary[tag].Dequeue();

            obj.transform.SetParent(parent);
            obj.SetActive(true);
            obj.transform.position = position;
            obj.transform.rotation = rotation;

            poolDictionary[tag].Enqueue(obj);

            return obj;
        }
        catch(NullReferenceException e) {
            return null;
        }     
    }

    public void Despawn(GameObject obj) {
        if (!ObjectPooler.Instance) return;

        StartCoroutine(WaitFrameBeforeHierachyMove(obj));
    }

    private IEnumerator WaitFrameBeforeHierachyMove(GameObject obj) {
        yield return null;
        obj.transform.SetParent(this.transform);
        obj.SetActive(false);
    }

}
