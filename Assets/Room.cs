using System;
using System.Collections;
using Unity.AI.Navigation;
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
    private int currentRoom;

    public float enemiesSpawnDelay;

    public bool canEnemiesSpawn;
    public bool canChestSpawn;
    public bool chestTaken;
    public bool doorTaken;

    public GameObject door1;
    public GameObject door2;
    public GameObject door3;

    public GameObject chest;
    public GameObject player;

    public GameObject[] enemies;
    public GameManager gameManager;
    private ProceduralGeneration proGen;
    public GameObject enemyGroup;
    public NavMeshSurface navMeshSurface;

    public UIManager uiManager;

    public Transform spawnPoint;
    public float enterWalkPointOffset;
    public float exitWalkPointOffset;
    private Vector3 walkToPointWhenEnter;
    private Vector3 walkToPointWhenExit;

    public int lastDoorPos;

    private void Awake()
    {
        /*navMeshSurface = GameObject.Find("NavMeshSurface").GetComponent<NavMeshSurface>();
        navMeshSurface.transform.position = transform.position;
        navMeshSurface.BuildNavMesh();*/
    }

    IEnumerator Start()
    {
        //assignations
        player = GameObject.Find("Player");
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        proGen = GameObject.Find("LevelManager").GetComponent<ProceduralGeneration>();
        uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();
        currentRoom = gameManager.currentRoom;
        enemies = proGen.enemies;
        
        //black fade out
        uiManager.StartCoroutine("Whiteout");
        
        //marche jusqu'a la porte d'arrivée
        EntryWalkProcedure();
        
        //quand c'est fini

        yield return new WaitForSeconds(0.1f);
        //enemis apparaissent après un certain temps
        StartCoroutine(EnemySpawnDelay());
    }
    
    void Update()
    {
        if (canEnemiesSpawn)
        {
            canEnemiesSpawn = false;
            //EnemyGeneration();
        }
        //quand ennemis tous morts, coffre spawn
    if (enemiesRemaining == 0 && canChestSpawn)
    {
        canChestSpawn = false;
        Instantiate(chest, transform.position, quaternion.identity);
    }

    //quand coffre récupéré, portes s'ouvrent
    if (chestTaken)
    {
        //door.SetActive(true);
    }
    //quand prend un porte, les autres se ferment
    if (doorTaken)
    {
        //goes throught desired door
        //shuts all three doors
        //other doors shut
        //blackout
        uiManager.StartCoroutine("Blackout");
        //genere salle suivante
        //tp dans salle suivante
    }
    
    }

    IEnumerator EnemySpawnDelay()
    {
        yield return new WaitForSeconds(enemiesSpawnDelay);
        canEnemiesSpawn = true;
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

    void EntryWalkProcedure()
    {
        player.transform.position = spawnPoint.position;
        if (lastDoorPos == 1)
        {
            walkToPointWhenEnter = new Vector3(-enterWalkPointOffset, 0, 0);
        }
        if (lastDoorPos == 2)
        {
            walkToPointWhenEnter = new Vector3(0, 0, enterWalkPointOffset);
        }
        if (lastDoorPos == 3)
        {
            walkToPointWhenEnter = new Vector3(enterWalkPointOffset, 0, 0);
        }
        //player.GetComponent<PlayerController>().StartCoroutine(MoveTowardsPoint((spawnPoint.position + walkToPointWhenEnter) - player.transform.position));
    }

    IEnumerator WalkToPoint()
    {
        //yield return WaitUntil(() => player.transform.position == spawnPoint.position + walkToPointWhenEnter);
        yield return new WaitForSeconds(1);
    }
}
