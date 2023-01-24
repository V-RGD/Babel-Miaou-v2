using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyScene : MonoBehaviour
{
    public static DestroyScene instance;
    
    public List<GameObject> components;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    public void DestroyEverything()
    {
        foreach (var VARIABLE in components)
        {
            Destroy(VARIABLE);
        }
    }
}
