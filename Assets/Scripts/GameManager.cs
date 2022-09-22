using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject player;
    public int money;
    public float health;
    public int maxHealth;

    public int currentRoom;

    public LevelGeneration levelGeneration;
    public bool hasReachedTheEnd;

    public GameObject[] items;

    private void Awake()
    {
        player = GameObject.Find("Player");
        levelGeneration = GameObject.Find("LevelManager").GetComponent<LevelGeneration>();
    }

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
    }
}
