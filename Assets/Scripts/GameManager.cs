using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject player;
    public int money;
    public float health;
    public int maxHealth;

    public int currentRoom;

    public bool canSpawnEnemies;

    public GameObject[] items;

    private void Awake()
    {
        player = GameObject.Find("Player");
    }

    void Start()
    {
        health = maxHealth;
    }
}
