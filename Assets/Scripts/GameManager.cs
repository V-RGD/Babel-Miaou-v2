using UnityEngine;

public class GameManager : MonoBehaviour
{
    private GameObject player;
    public GameObject[] items;

    public int money;
    public int maxHealth = 3;
    [HideInInspector]public float health;

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
