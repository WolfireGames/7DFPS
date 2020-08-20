using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : UnityEngine.Component {
    private static T _instance = default(T);
    public static T instance {
        get {
            if(!_instance) {
                _instance = UnityEngine.Object.FindObjectOfType<T>();

                if(!_instance)
                    _instance = (T) new GameObject(typeof(T).ToString()).AddComponent(typeof(T));
            }

            return _instance;
        }
    }

    public void Awake() {
        if(_instance && _instance != this) {
            DestroyImmediate(this.gameObject);
        } else {
            _instance = instance;
            DontDestroyOnLoad(this.gameObject);
            Init();
        }
    }

    public abstract void Init();
}
