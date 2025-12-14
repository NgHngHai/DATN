using UnityEngine;

public abstract class GenericSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    private static bool isQuitting;

    public static T Instance
    {
        get
        {
            if (isQuitting)
                return null;

            if (_instance == null)
            {
                _instance = FindFirstObjectByType<T>();

                if (_instance == null)
                {
                    GameObject go = new GameObject(typeof(T).Name);
                    _instance = go.AddComponent<T>();
                    DontDestroyOnLoad(go);
                }
            }
            return _instance;
        }
    }

    protected virtual void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this as T;
        DontDestroyOnLoad(gameObject);
    }

    protected virtual void OnApplicationQuit()
    {
        isQuitting = true;
    }

    protected virtual void OnDestroy()
    {
        if (_instance == this)
            isQuitting = true;
    }
}
