using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private GameObject player;
    public GameObject[] items;

    public int money;
    public int maxHealth = 3;
    [HideInInspector]public int health;

    //player base stats
    [HideInInspector]public float baseHealth;
    [HideInInspector]public float baseAttack;
    [HideInInspector]public float baseDexterity;
    
    public int currentRoom;

    public bool isDead;

    private void Awake()
    {
        player = GameObject.Find("Player");
    }

    void Start()
    {
        health = maxHealth;
    }

    private void Update()
    {
        //caps health to the max amount
        if (maxHealth < health)
        {
            health = maxHealth;
        }

        if (health <= 0)
        {
            isDead = true;
        }
    }
}
