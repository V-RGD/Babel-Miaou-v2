using System;
using System.Collections.Generic;
using UnityEngine;

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
        health = GameManager.instance.health;
        maxHealth = GameManager.instance.maxHealth;
        initialMaxHealth = GameManager.instance.initialMaxHealth;

        money = GameManager.instance.money;
        score = GameScore.instance.tempScore;

        for (int i = 0; i < ObjectsManager.instance.itemObjectsInventory.Count; i++)
        {
            itemObjectsInventory[i] = ObjectsManager.instance.itemObjectsInventory[i];
        }
    }
}
