using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SingletonMonobehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    static T Instance;

    public static T instance
    {
        get
        {
            if (null == Instance)
            {
                Instance = FindObjectOfType(typeof(T)) as T;
                if (null == Instance)
                {
                    return null;
                }
            }
            return Instance;
        }
    }
}

public abstract class Singleton<T> where T : class, new()
{
    static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new T();
            }

            return instance;
        }
    }
}
