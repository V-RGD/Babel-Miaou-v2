using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class SaveProgression : MonoBehaviour
{
    public static SaveProgression instance;
    
    public int health;
    public int maxHealth;
    public int initialMaxHealth;
    public int money;
    public int currentLevel;

    public List<int> itemObjectsInventory;

    public float score;

    private void Awake()
    {
        if (instance != null)
        {
            return;
        }
        else
        {
            instance = this;
        }
    }

    public void SetGameValues()
    {
        health = GameManager_old.instance.health;
        maxHealth = GameManager_old.instance.maxHealth;
        initialMaxHealth = GameManager_old.instance.initialMaxHealth;

        money = GameManager_old.instance.money;
        score = GameScore_old.instance.tempScore;

        for (int i = 0; i < ObjectsManager_old.instance.itemObjectsInventory.Count; i++)
        {
            itemObjectsInventory[i] = ObjectsManager_old.instance.itemObjectsInventory[i];
        }
    }
}
