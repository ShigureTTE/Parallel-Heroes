using UnityEngine;

public class DontDestroySingleton<T> : MonoBehaviour
    where T : Component {

    private static T instance;
    private static bool willQuit;

    public static T Instance {
        get {
            if (instance != null) return instance;
            if (willQuit) return null;

            var objects = FindObjectsOfType(typeof(T)) as T[];
            if (objects.Length > 0)
                instance = objects[0];
            if (objects.Length > 1)
                Debug.LogError("[MonoBehaviourSingleton] The is more than one " + typeof(T).Name + " in the scene.");

            if (instance != null) return instance;

            Debug.Log("[MonoBehaviourSingleton] Creating new instance of " + typeof(T).Name);
            var obj = new GameObject(typeof(T).Name);
            instance = obj.AddComponent<T>();
            return instance;
        }
    }

    private void Awake() {
        if (instance == null) {
            instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }
    }

    private void OnApplicationQuit() {
        willQuit = true;
    }
}
