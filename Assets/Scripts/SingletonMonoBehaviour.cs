using System;
using UnityEngine;

public class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                Type t = typeof(T);
                _instance = (T)FindObjectOfType(t);
            }

            if (_instance == null)
            {
                Debug.LogError("Singleton MonoBehaviour Error");
            }

            return _instance;
        }
    }

    virtual protected void Awake()
    {
        CheckInstance();
    }

    protected void CheckInstance()
    {
        if (_instance == null)
        {
            _instance = this as T;
            return;
        }
        else if (Instance == this)
        {
            return;
        }

        Destroy(this);
    }
}