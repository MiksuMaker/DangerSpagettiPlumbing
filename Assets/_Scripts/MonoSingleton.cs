using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    public static T Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            if (Instance != this)
            {
                Destroy(this);
            }
        }
        else
        {
            Instance = this as T;
        }
    }
}