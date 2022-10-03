using System.Collections;
using Unity.AI.Navigation;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class Room : MonoBehaviour
{
    //manages enemy spawns, chest loot, doors

    /*
    public int maxEnemies = 4;
    public int minEnemies = 2;
    private int enemyNumber;
    public int enemiesRemaining;
    public int haunterRate;
    public int mageRate;
    public int bullRate;
    private int currentRoom;
    
    public bool canChestSpawn = true;
    public bool doorTaken;

    public GameObject chestPrefab;
    public GameObject spawnWarning;
    public GameObject player;
    private GameObject[] enemies;
    private GameObject[] bosses;
    public GameManager gameManager;
    //private ProceduralGeneration proGen;
    public GameObject enemyGroup;
    public NavMeshSurface navMeshSurface;
    public UIManager uiManager;
    public GameObject chest;

    public Vector3 playerSpawnPoint;
    public Vector3 nextPlayerSpawn;
    private Vector3 enterPoint;
    private Vector3 exitPoint;

    public int lastDoorPos;
    public bool finishedEnemySpawn;
    public bool canEnterDoor;
    public bool autoWalk;
    public bool changeRoom;

    public float autoWalkMagnitude = 30;
    
    public bool canSpawnEnemies;
    public bool isBossRoom;
    public bool canShopSpawn;
    
    private void Awake()
    {
        navMeshSurface = GameObject.Find("NavMeshSurface").GetComponent<NavMeshSurface>();
        navMeshSurface.transform.position = transform.position;
        navMeshSurface.BuildNavMesh();
    }

    IEnumerator Start()
    {
        //assignations
        player = GameObject.Find("Player");
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        //proGen = GameObject.Find("LevelManager").GetComponent<ProceduralGeneration>();
        uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();
        currentRoom = gameManager.currentRoom;
        //enemies = proGen.enemies;
        //bosses = proGen.bosses;
        canChestSpawn = true;
        
        //black fade out en marchant vers la salle puis enemis spawnent
        uiManager.StartCoroutine("Whiteout");
        
        EntryWalk();
        yield return new WaitForSeconds(2f);
        RoomType();
    }
    
    void Update()
    {
        //when all enemies defeated
        enemiesRemaining = enemyGroup.transform.childCount;
        //lastDoorPos = proGen.lastDoorPos;
        if (finishedEnemySpawn && enemiesRemaining == 0 && canChestSpawn)
        {
            canChestSpawn = false;
            chest = Instantiate(chestPrefab, new Vector3(transform.position.x, player.transform.position.y, transform.position.z), quaternion.identity);
        }

        if (chest != null)
        {
            if (chest.GetComponent<Chest>().isOpen) 
            {
                //opens door
                canEnterDoor = true;
            }
        }
        
        //quand prend une porte, les autres se ferment
        if (doorTaken && canEnterDoor)
        {
            canEnterDoor = false;
            doorTaken = false;
            //blackout
            uiManager.StartCoroutine("Blackout");
            //marche vers le couloir
            ExitWalk();
        }
        
        if (autoWalk)
        {
            //player.GetComponent<Rigidbody>().AddForce(exitPoint - transform.position * autoWalkMagnitude);
            player.GetComponent<PlayerController>().movementDir = (exitPoint - transform.position).normalized;
        }

        if (changeRoom)
        {
            //genere salle suivante
            //proGen.canGenerate = true;
            //tp dans salle suivante
            player.transform.position = enterPoint;
            //destroy itself
            Destroy(gameObject);
        }
    }

    void EntryWalk()
    {
        //place player accordingly to last door taken
        if (currentRoom == 0)
        {
            playerSpawnPoint = transform.position;
            Debug.Log("this is the first room");
        }
        
        else
        {
            Debug.Log("this is the" + currentRoom + "room");
            //player.transform.position = proGen.enterPos;
        }
    }
    
    void ExitWalk()
    {
        if (lastDoorPos == 1)
        {
            exitPoint = transform.position + Vector3.left * 30;
            enterPoint = transform.position + Vector3.right * 30;
        }
        if (lastDoorPos == 2)
        {
            exitPoint = transform.position + Vector3.forward * 30;
            enterPoint = transform.position + Vector3.back * 30;
        }
        if (lastDoorPos == 3)
        {
            exitPoint = transform.position + Vector3.right * 30;
            enterPoint = transform.position + Vector3.left * 30;
        }

        //proGen.enterPos = enterPoint;
        StartCoroutine(WalkToPoint());
    }

    IEnumerator EnemyGeneration()
    {
        
        if (!isBossRoom)//spawns enemies as normal
        {
            
            //decides the number of enemies to spawn
            enemyNumber = Random.Range(minEnemies + currentRoom/2, maxEnemies + currentRoom/2);
            enemiesRemaining = enemyNumber;
        
            //for each enemy, randomizes which to spawn
            for (int i = 0; i < enemyNumber; i++)
            {
                //calculates a random position where the enemy will spawn
                int randPosX = Random.Range(-20, 21);
                int randPosY = Random.Range(-20, 21);

                Vector3 spawnPoint = transform.position + new Vector3(randPosX, player.transform.position.y + 5, randPosY);

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
                //enables a gameobject there to show spawn point
                //GameObject debugSpawnPoint = Instantiate(spawnWarning);
                //debugSpawnPoint.transform.position = spawnPoint;
                //Destroy(debugSpawnPoint);
                GameObject enemySpawning = Instantiate(enemies[enemyToSpawn], enemyGroup.transform);
                enemySpawning.transform.position = spawnPoint;
                
                yield return new WaitForSeconds(0.4f);
            }

            finishedEnemySpawn = true;
        }
        
        else //spawns boss instead
        {
            //enables a gameobject there to show spawn point
            //GameObject debugBossSpawn = Instantiate(bossSpawnWarning);
            //debugSpawnPoint.transform.position = spawnPoint;
            //Destroy(debugSpawnPoint);
            GameObject bossSpawning = Instantiate(bosses[Random.Range(0, bosses.Length)], enemyGroup.transform);
            bossSpawning.transform.position = transform.position;
            yield return new WaitForSeconds(0.4f);
        }
        
    }

    IEnumerator WalkToPoint()
    {
        player.GetComponent<PlayerController>().canMove = false;
        player.GetComponent<Rigidbody>().velocity = Vector3.zero;
        autoWalk = true;
        yield return new WaitForSeconds(2);
        autoWalk = false;
        player.GetComponent<PlayerController>().canMove = true;
        player.GetComponent<Rigidbody>().velocity = Vector3.zero;
        changeRoom = true;
    }
    
    void RoomType()
    {
        //starting room doesn't have any ennemies
        if (currentRoom == 0)
        {
            //do nothing
            canEnterDoor = true;
        }
        
        //if room = shop, opens by default
        else if (currentRoom == proGen.size - 1)
        {
            canShopSpawn = true;
            GameObject mecLouche = Instantiate(proGen.shop, transform);
            mecLouche.transform.position = new Vector3(0, player.transform.position.y , 0) + Vector3.up * 5;
            canEnterDoor = true;
        }
        //if room = boss spawns boss instead
        else if (currentRoom == proGen.size)
        {
            isBossRoom = true;
            StartCoroutine(EnemyGeneration());
        }

        //if room normal, act as usual
        else
        {
            StartCoroutine(EnemyGeneration());
        }
    }
    
    */
}
