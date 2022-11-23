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
    private DunGen _dunGen;
    private ObjectsManager _objectsManager;
    private RoomInfo _roomInfo;
    [SerializeField]private Transform roomCenter;

    //room info
    public int roomType;
    public int currentRoom;

    //values
    private GameObject _player;
    [SerializeField]private GameObject empty;
    [HideInInspector]public GameObject enemyGroup;
    private int _enemiesRemaining;
    private bool _canChestSpawn = true;
    private bool _canSpawnStela;
    private bool _canActivateEnemies = true;
    private bool _hasPlayerEnteredRoom;
    private const float RoomDetectZoneSize = 0.4f; //gave up finding a name --- the percentage of the room which detects the player if it's close from the center
    void Awake()
    {
        // component assignations
        _player = GameObject.Find("Player");
        _lm = GameObject.Find("LevelManager").GetComponent<LevelManager>();
        _dunGen = GameObject.Find("LevelManager").GetComponent<DunGen>();
        _objectsManager = GameObject.Find("GameManager").GetComponent<ObjectsManager>();
        enemyGroup = transform.GetChild(0).gameObject;
        chest = _lm.chest;
        doorPrefab = _lm.door;
        _roomInfo = GetComponent<RoomInfo>();
        GameObject group = Instantiate(empty, transform);
        enemyGroup = group;
    }

    private void Start()
    {
        RoomType();
        //PropsSpawn();
        DoorSpawn();
    }

    void Update()
    {
        _enemiesRemaining = enemyGroup.transform.childCount; //check how many enemies are still in the room
        if (_enemiesRemaining == 0 && _canChestSpawn && roomType != 0 && roomType != 3 || Input.GetKeyDown(KeyCode.O))
        {
            _canChestSpawn = false;
            chest = Instantiate(chest, roomCenter.position + Vector3.up, quaternion.identity);
            DoorUnlock();
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
    }
    void EnemyGeneration()
    {
        //decides the number of enemies to spawn : base population + difficulty increase + 20% uncertainty
        int enemyPopulation = Mathf.RoundToInt((_lm.minEnemies + currentRoom * _lm.populationGrowthFactor) * Random.Range(1f, 1.2f));

        _enemiesRemaining = enemyPopulation;
        //for each enemy, randomizes which to spawn
        for (var i = 0; i < enemyPopulation; i++)
        {
            //calculates a random position where the enemy will spawn
            float randPosX = Random.Range(-15, 15);
            float randPosY = Random.Range(-15, 15);
            var spawnPoint = roomCenter.position + new Vector3(randPosX, 3, randPosY);

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
            //instantiates it as a child to track down how many are left
            GameObject enemySpawning = Instantiate(_lm.basicEnemies[enemyToSpawn], enemyGroup.transform);
            enemySpawning.transform.position = spawnPoint;
            enemySpawning.GetComponent<Enemy>().room = gameObject;
            enemySpawning.SetActive(false);
            Debug.Log("enemy spawned");
        }
    }
    
    void RoomType()
    {
        if (currentRoom == 999)
        {
            return;
            //roomType = 2;
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
        //starting room doesn't have any enemies
        switch (roomType)
        {
            case 0 : //start room
                _lm.entrance.transform.position = roomCenter.position;
                StartCoroutine(PlacePlayerAtSpawnPoint());
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
            case 4 : //mini-boss room
                MiniBossSpawn();
                _lm.exit.transform.position = roomCenter.position;
                _lm.exit.SetActive(false);
                break;
            case 5 : //final boss room
                break;
        }
    }

    void MiniBossSpawn()
    {
        GameObject bossSpawning = Instantiate(_lm.miniBosses[Random.Range(0, _lm.miniBosses.Length)], enemyGroup.transform);
        bossSpawning.transform.position = roomCenter.position;
    }

    void ShopSpawn()
    {
        Instantiate(_lm.shop, roomCenter.position + Vector3.up * 7, quaternion.identity);
    }

    void CheckPlayerPresence()
    {
        var position = roomCenter.position;
        var roomDetectionXMax = position.x + _lm.roomSize * RoomDetectZoneSize;
        var roomDetectionXMin = position.x - _lm.roomSize * RoomDetectZoneSize;
        var roomDetectionZMax = position.z + _lm.roomSize * RoomDetectZoneSize;
        var roomDetectionZMin = position.z - _lm.roomSize * RoomDetectZoneSize;
        
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
        _objectsManager.noHitStreak = true;
        for (int i = 0; i < enemyGroup.transform.childCount; i++)
        {
            enemyGroup.transform.GetChild(i).gameObject.SetActive(true);
            enemyGroup.transform.GetChild(i).gameObject.GetComponent<Enemy>().enabled = true;
            enemyGroup.transform.GetChild(i).gameObject.GetComponent<EnemyDamage>().enabled = true;
            enemyGroup.transform.GetChild(i).gameObject.GetComponent<Enemy>().startSpawning = true;
            yield return new WaitForSeconds(0.5f);
        }
    }

    void DoorSpawn()
    {
        
        for (int i = 0; i < 4; i++)
        {
            //checks if there is supposed to be doors on each corner
            if (_roomInfo.doors[i] == 1)
            {
                //spawns doors accordingly
                var cornerOffset = -17;
                var upOffset = 8;
                var roomSize = 50 /2;
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
                GameObject door = Instantiate(doorPrefab, spawnPoint + roomCenter.position, Quaternion.Euler(rotation));
                door.transform.parent = gameObject.transform;
                door.SetActive(false);
                doorsObjects.Add(door);
            }
        }
    }

    void DoorUnlock()
    {
        foreach (var door in doorsObjects)
        {
            door.SetActive(false);
        }
    }

    IEnumerator ForeignFriend()
    {
        yield return new WaitForSeconds(2);
        int rand = Random.Range(0, enemyGroup.transform.childCount);
        Destroy(enemyGroup.transform.GetChild(rand));
    }

    IEnumerator PlacePlayerAtSpawnPoint()
    {
        yield return new WaitForSeconds(0.5f);
        _player.transform.position = roomCenter.transform.position + Vector3.up * 1;
    }
}
