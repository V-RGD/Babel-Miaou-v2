using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class Room : MonoBehaviour
{
    //manages what spawns in the room, and what events happen (doors, events, stairs)

    [Header("Generic Prefabs")]
    public GameObject chest;
    [HideInInspector] public GameObject doorPrefab;
    [HideInInspector] public List<GameObject> doorsObjects;

    //components
    private LevelManager _lm;
    private UIManager _uiManager;
    private GameManager _gameManager;
    private DunGen _dunGen;
    
    //room info
    public int roomType;
    public int currentRoom;

    //values
    private GameObject _player;
    private GameObject _enemyGroup;
    private int _enemiesRemaining;
    private bool _canChestSpawn = true;
    private bool _canOpenDoors = true;
    private bool _canSpawnStela;
    private bool _canActivateEnemies = true;
    private bool _hasPlayerEnteredRoom;
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
        doorPrefab = _lm.door;
        _canOpenDoors = true;

        RoomType();
        //PropsSpawn();
        DoorSpawn();
    }
    
    void Update()
    {
        _enemiesRemaining = _enemyGroup.transform.childCount; //check how many enemies are still in the room
        
        if (_enemiesRemaining == 0 && _canChestSpawn && roomType != 0 && roomType != 3)
        {
            _canOpenDoors = true;
            _canChestSpawn = false;
            chest = Instantiate(chest, new Vector3(transform.position.x, 0, transform.position.z), quaternion.identity);
            if (roomType == 4)
            {
                _lm.exit.SetActive(true);
            }
        }

        if (_hasPlayerEnteredRoom && _canActivateEnemies && roomType == 1)
        {
            _canActivateEnemies = false;
            StartCoroutine(ActivateAllEnemies());
        }
        
        CheckPlayerPresence();
        DoorUnlock();
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
            float randPosX = Random.Range(-30, 30);
            float randPosY = Random.Range(-30, 30);
            Vector3 spawnPoint = transform.position + new Vector3(randPosX, 3, randPosY);

            //check which enemy is available
            List<int> enemiesAvailable = new List<int>(5);

            if (currentRoom >= _lm.spawnMatrixUnlock[0])
            {
                enemiesAvailable.Add(0);
            }
            if (currentRoom >= _lm.spawnMatrixUnlock[1])
            {
                enemiesAvailable.Add(1);
            }
            if (currentRoom >= _lm.spawnMatrixUnlock[2])
            {
                enemiesAvailable.Add(2);
            }
            if (currentRoom >= _lm.spawnMatrixUnlock[3])
            {
                enemiesAvailable.Add(3);
            }
            if (currentRoom >= _lm.spawnMatrixUnlock[4])
            {
                enemiesAvailable.Add(4);
            }
            //take a random one for a list of available enemies
            int enemyToSpawn = enemiesAvailable[Random.Range(0, enemiesAvailable.Count)];
            //instanciates it as a child to track down how many are left
            GameObject enemySpawning = Instantiate(_lm.basicEnemies[enemyToSpawn], _enemyGroup.transform);
            enemySpawning.transform.position = spawnPoint;
            enemySpawning.SetActive(false);
        }
    }
    
    void RoomType()
    {
        //Debug.Log(currentRoom);
        if (currentRoom == 999)
        {
            roomType = 2;
        }

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
                _lm.entrance.transform.position = transform.position;
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
                break;
            case 4 : //miniboss room
                MiniBossSpawn();
                _lm.exit.transform.position = transform.position;
                _lm.exit.SetActive(false);
                break;
            case 5 : //final boss room
                break;
        }
    }

    void MiniBossSpawn()
    {
        Debug.Log("spawned miniboss");
        GameObject bossSpawning = Instantiate(_lm.miniBosses[Random.Range(0, _lm.miniBosses.Length)], _enemyGroup.transform);
        bossSpawning.transform.position = transform.position;
    }

    void ShopSpawn()
    {
        Debug.Log("spawned shop");
        Instantiate(_lm.shop, transform.position + Vector3.up * 7, quaternion.identity);
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

    IEnumerator ActivateAllEnemies()
    {
        _canOpenDoors = false;
        for (int i = 0; i < _enemyGroup.transform.childCount; i++)
        {
            _enemyGroup.transform.GetChild(i).gameObject.SetActive(true);
            _enemyGroup.transform.GetChild(i).gameObject.GetComponent<Enemy>().enabled = true;
            _enemyGroup.transform.GetChild(i).gameObject.GetComponent<EnemyDamage>().enabled = true;
            _enemyGroup.transform.GetChild(i).gameObject.GetComponent<Enemy>().startSpawning = true;
            yield return new WaitForSeconds(0.5f);
        }
    }

    void DoorSpawn()
    {
        RoomInfo info = GetComponent<RoomInfo>();
        for (int i = 0; i < 4; i++)
        {
            //checks if there is supposed to be doors on each corner
            if (info.doors[i] == 1)
            {
                //spawns doors accordingly
                var cornerOffset = 5;
                var upOffset = 8;
                var roomSize = 50;
                var spawnPoint = Vector3.zero;
                var rotation = Vector3.zero;
                switch (i)
                {
                    case 0 : //left
                        spawnPoint = new Vector3(-roomSize + cornerOffset, upOffset, 0);
                        rotation = new Vector3(90, -90, 0); break;
                    case 1 :  //up
                        spawnPoint = new Vector3(0, upOffset, roomSize - cornerOffset);
                        rotation = new Vector3(90, 0, 0); break;
                    case 2 :  //right
                        spawnPoint = new Vector3(roomSize - cornerOffset, upOffset, 0);
                        rotation = new Vector3(90, 90, 0); break;
                    case 3 :  //down
                        spawnPoint = new Vector3(0, upOffset, -roomSize + cornerOffset);
                        rotation = new Vector3(90, 180, 0); break;
                }
                GameObject door = Instantiate(doorPrefab, spawnPoint + transform.position, Quaternion.Euler(rotation));
                door.transform.parent = gameObject.transform;
                doorsObjects.Add(door);
            }
        }
    }

    void DoorUnlock()
    {
        foreach (var door in doorsObjects)
        {
            switch (_canOpenDoors)
            {
                case true : door.SetActive(false); break;
                case false : door.SetActive(true); break;
            }
        }
    }
}
