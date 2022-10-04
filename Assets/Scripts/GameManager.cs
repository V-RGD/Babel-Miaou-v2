using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject player;
    public GameObject[] items;

    public int money;
    public float health;
    public int maxHealth;

    public int currentRoom;

    private void Awake()
    {
        player = GameObject.Find("Player");
    }

    void Start()
    {
        health = maxHealth;
    }
}
