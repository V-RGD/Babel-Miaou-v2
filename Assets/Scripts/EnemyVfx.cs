using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVfx : MonoBehaviour
{
    public static EnemyVfx instance;
    public VfxPulling hitFx;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }
}
