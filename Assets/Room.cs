using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class Room : MonoBehaviour
{
    //manages enemy spawns, chest loot, doors

    public int maxEnemies = 4;
    public int minEnemies = 2;
    private int enemyNumber;

    public int enemiesRemaining;

    public int haunterRate;
    public int mageRate;
    public int bullRate;

    public GameObject[] enemies;
    
    private int currentRoom;

    public GameManager gameManager;
    private ProceduralGeneration proGen;

    public GameObject enemyGroup;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        proGen = GameObject.Find("LevelManager").GetComponent<ProceduralGeneration>();
        currentRoom = gameManager.currentRoom;
        enemies = proGen.enemies;
            EnemyGeneration();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void EnemyGeneration()
    {
        //decides the number of enemies to spawn
        enemyNumber = Random.Range(minEnemies + currentRoom/2, maxEnemies + currentRoom/2);
        enemiesRemaining = enemyNumber;
        for (int i = 0; i < enemyNumber; i++)
        {
            Vector3 spawnPoint = transform.position + new Vector3(0, -5, 0);
            
            int randomizeEnemy = Random.Range(0, 10);
            int enemyToSpawn = 0;
            if (randomizeEnemy <= haunterRate)
            {
                enemyToSpawn = 0;
            }
            if (randomizeEnemy > haunterRate && randomizeEnemy <= mageRate)
            {
                enemyToSpawn = 1;

            }
            if (randomizeEnemy > mageRate && randomizeEnemy <= bullRate)
            {
                enemyToSpawn = 2;
            }

            GameObject enemySpawning = Instantiate(enemies[enemyToSpawn], enemyGroup.transform);
            enemySpawning.transform.position = spawnPoint;
        }
    }
}
