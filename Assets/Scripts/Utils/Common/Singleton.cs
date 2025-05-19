using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonBase {
    protected static SingletonBase instance;
    public static SingletonBase Instance {
        get {
            if (instance == null) {
                instance = new SingletonBase();
            }
            return instance;
        }
    }

    protected SingletonBase() { }
}


public abstract class Singleton : MonoBehaviour
{
    protected static bool m_isQuitting = false;
    public static bool IsQuitting { get; protected set; }
    
    
    [SerializeField, SetProperty("Persistant")]
    protected bool m_isPersistant = false;
    public bool Persistant { 
        get => m_isPersistant; 
        protected set => m_isPersistant = value; 
    }
 
    protected void Awake() {
        if (Persistant) {
            DontDestroyOnLoad(gameObject);
        }
        OnAwake();
    }

    protected virtual void OnAwake() { }

    protected void OnDestroy()
    {
        IsQuitting = true;
    }

    protected static Singleton instance;
    public static Singleton Instance {
        get {
            if (IsQuitting) {
                // Debug.LogWarning("[Singleton] Instance '" + typeof(Singleton) +
                //                 "' already destroyed on application quit." +
                //                 " Won't create again - returning null.");
                return null;
            }
            if (instance == null) {
                instance = FindObjectOfType<Singleton>();
                if (instance == null) {
                    GameObject obj = new GameObject
                    {
                        name = typeof(Singleton).Name
                    };
                    instance = obj.AddComponent<Singleton>();
                }
            }
            return instance;
        }
    }

    protected Singleton() { }
}

public abstract class Singleton<T> : Singleton where T : MonoBehaviour
{
    protected new void Awake() {
        if (Persistant) {
            DontDestroyOnLoad(gameObject);
        }
        OnAwake();
    }

    protected new void OnDestroy()
    {
        IsQuitting = true;
    }

    private static new T instance;
    public static new T Instance {
        get {
            if (instance == null) {
                instance = FindObjectOfType<T>();
                if (instance == null) {
                    GameObject obj = new GameObject
                    {
                        name = typeof(T).Name
                    };
                    instance = obj.AddComponent<T>();
                }
            }
            return instance;
        }
    }
}
