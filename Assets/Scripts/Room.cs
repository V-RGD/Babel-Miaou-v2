using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class Room : MonoBehaviour
{
    //manages what spawns in the room, and what events happen (doors, events, stairs)

    [Header("Generic Prefabs")]
    [HideInInspector] public GameObject chest;
    [HideInInspector] public GameObject doorL;
    [HideInInspector] public GameObject doorR;
    [HideInInspector] public GameObject doorU;
    [HideInInspector] public GameObject doorD;

    //components
    private LevelManager _lm;
    private UIManager _uiManager;
    private GameManager _gameManager;
    private DunGen _dunGen;

    //values
    private GameObject _player;
    private GameObject _enemyGroup;
    private int _enemiesRemaining;
    private int _lastDoorPos;
    private bool _isDoorUnlocked;
    private bool _canChestSpawn = true;
    private bool _doorTaken;
    private bool _canSpawnStela;
    private bool _canActivateEnemies = true;
    private bool _hasPlayerEnteredRoom;
    public int roomType;
    public int currentRoom;
    private const float RoomDetectZoneSize = 0.4f; //gave up finding a name --- the percentage of the room which'll detect the player if it's close from the center
    void Awake()
    {
        // component assignations
        _player = GameObject.Find("Player");
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _lm = GameObject.Find("LevelManager").GetComponent<LevelManager>();
        _dunGen = GameObject.Find("LevelManager").GetComponent<DunGen>();
        _enemyGroup = transform.GetChild(0).gameObject;
        chest = _lm.chest;

        RoomType();
        //PropsSpawn();
        //DoorLock();
    }
    
    void Update()
    {
        _enemiesRemaining = _enemyGroup.transform.childCount; //check how many enemies are still in the room
        
        if (_enemiesRemaining == 0 && _canChestSpawn && roomType != 0 && roomType != 3)
        {
            _canChestSpawn = false;
            chest = Instantiate(chest, new Vector3(transform.position.x, 0, transform.position.z), quaternion.identity);
        }

        if (chest != null) //opens door

        {
            if (chest.GetComponent<Chest>().isOpen) 
            {
                _isDoorUnlocked = true;
            }
        }

        if (_hasPlayerEnteredRoom == _canActivateEnemies)
        {
            _canActivateEnemies = false;
            ActivateAllEnemies();
        }
        /*
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
            //_dunGen.canGenerate = true;
            //tp dans salle suivante
            player.transform.position = enterPoint;
            //destroy itself
            Destroy(gameObject);
        }
        */
        CheckPlayerPresence();
    }

    #region AutoWalk
    /*
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
            //player.transform.position = _dunGen.enterPos;
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

        //_dunGen.enterPos = enterPoint;
        StartCoroutine(WalkToPoint());
    }
    */
    #endregion
    
    void EnemyGeneration()
    {
        //decides the number of enemies to spawn : base population + difficulty increase + 20% incertainty
        int enemyPopulation = Mathf.RoundToInt((_lm.minEnemies + currentRoom * _lm.populationGrowthFactor) * Random.Range(1f, 1.2f));

        _enemiesRemaining = enemyPopulation;
        //for each enemy, randomizes which to spawn
        for (int i = 0; i < enemyPopulation; i++)
        {
            //calculates a random position where the enemy will spawn
            float randPosX = Random.Range(-_lm.roomSize, -_lm.roomSize);
            float randPosY = Random.Range(-_lm.roomSize, -_lm.roomSize);
            Vector3 spawnPoint = transform.position + new Vector3(randPosX, 0, randPosY);

            //check which enemy is available
            List<int> enemiesAvailable = new List<int>();
            for (int j = 0; j < _lm.enemySpawnMatrix.Length; j++)
            {
                if (_lm.enemySpawnMatrix[j] == 1)
                {
                    enemiesAvailable.Add(j);
                }
            }
            Debug.Log(enemiesAvailable.Count);

            //take a random one for a list of available enemies
            int enemyToSpawn = enemiesAvailable[Random.Range(0, enemiesAvailable.Count)];
            //instanciates it as a child to track down how many are left
            GameObject enemySpawning = Instantiate(_lm.basicEnemies[enemyToSpawn], _enemyGroup.transform);
            enemySpawning.transform.position = spawnPoint;
            enemySpawning.SetActive(false);
        }
    }

    /*
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
    */
    
    void RoomType()
    {
        if (currentRoom == 0)
        {
            roomType = 0;
        }

        else if (currentRoom == _dunGen.dungeonSize)
        {
            roomType = 4;
        }

        else if (currentRoom == _dunGen.dungeonSize - 1)
        {
            roomType = 3;
        }
        else
        {
            roomType = 1;
        }
        //starting room doesn't have any ennemies
        switch (roomType)
        {
            case 0 : //start room
                _isDoorUnlocked = true; 
                break;
            case 1 : //normal room
                EnemyGeneration();
                break;
            case 2 : //special room
                EnemyGeneration();
                _canSpawnStela = true;
                break;
            case 3 : //shop room
                ShopSpawn();
                _isDoorUnlocked = true;
                break;
            case 4 : //miniboss room
                MiniBossSpawn();
                break;
            case 5 : //final boss room
                MiniBossSpawn();
                break;
        }
    }

    void MiniBossSpawn()
    {
        GameObject bossSpawning = Instantiate(_lm.miniBosses[Random.Range(0, _lm.miniBosses.Length)], _enemyGroup.transform);
        bossSpawning.transform.position = transform.position;
    }

    void FinalBossSpawn()
    {
        GameObject bossSpawning = Instantiate(_lm.miniBosses[Random.Range(0, _lm.miniBosses.Length)], _enemyGroup.transform);
        bossSpawning.transform.position = transform.position;
    }

    void ShopSpawn()
    {
        Instantiate(_lm.shop, transform.position, quaternion.identity);
    }

    void CheckPlayerPresence()
    {
        float roomDetectionXMax = transform.position.x + _lm.roomSize * RoomDetectZoneSize;
        float roomDetectionXMin = transform.position.x - _lm.roomSize * RoomDetectZoneSize;
        float roomDetectionZMax = transform.position.z + _lm.roomSize * RoomDetectZoneSize;
        float roomDetectionZMin = transform.position.z - _lm.roomSize * RoomDetectZoneSize;
        
        if (_player.transform.position.x < roomDetectionXMax && _player.transform.position.x > roomDetectionXMin && 
            _player.transform.position.z > roomDetectionZMin && _player.transform.position.z < roomDetectionZMax )
        {
            _hasPlayerEnteredRoom = true;
        }
        else
        {
            _hasPlayerEnteredRoom = false;
        }
    }

    void ActivateAllEnemies()
    {
        for (int i = 0; i < _enemyGroup.transform.childCount; i++)
        {
            _enemyGroup.transform.GetChild(i).gameObject.SetActive(true);
            _enemyGroup.transform.GetChild(i).gameObject.GetComponent<Enemy>().enabled = true;
            _enemyGroup.transform.GetChild(i).gameObject.GetComponent<EnemyBehaviour>().enabled = true;
            _enemyGroup.transform.GetChild(i).gameObject.GetComponent<EnemyDamage>().enabled = true;

            _enemyGroup.transform.GetChild(i).gameObject.GetComponent<Enemy>().startSpawning = true;
        }
    }
}
