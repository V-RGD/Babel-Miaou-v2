using System;
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
    private GameManager _gameManager;
    private ObjectsManager _objectsManager;
    private RoomInfo _roomInfo;
    public Transform roomCenter;

    //room info
    public int roomType;
    public int currentRoom;

    //values
    private GameObject _player;
    [SerializeField]private GameObject visuals;
    [SerializeField]private GameObject empty;
    [HideInInspector]public GameObject enemyGroup;
    [HideInInspector]public bool isStelaActive;
    private int _enemiesRemaining;
    private bool _canChestSpawn = true;
    private bool _canActivateEnemies = true;
    private bool _hasPlayerEnteredRoom;
    private const float RoomDetectZoneSize = 0.4f; //gave up finding a name --- the percentage of the room which detects the player if it's close from the center

    private IEnumerator Start()
    {
        //component assignations
        _player = GameObject.Find("Player");
        _lm = LevelManager.instance;
        _dunGen = DunGen.instance;
        _objectsManager = ObjectsManager.instance;
        _gameManager = GameManager.instance;
        enemyGroup = transform.GetChild(0).gameObject;
        chest = _lm.chest;
        doorPrefab = _lm.door;
        _roomInfo = GetComponent<RoomInfo>();
        GameObject group = Instantiate(empty, transform);
        enemyGroup = group;

        DoorSpawn();
        yield return new WaitUntil(()=> _dunGen.finishedGeneration);
        RoomType();
        //PropsSpawn();
    }

    void Update()
    {
        _enemiesRemaining = enemyGroup.transform.childCount; //check how many enemies are still in the room
        if (_enemiesRemaining == 0 && _canChestSpawn && roomType != 0 && roomType != 3)
        {
            _canChestSpawn = false;
            DoorUnlock();
            if (roomType == 4)
            {
                _lm.exit.SetActive(true);
            }
            //randomize chest spawn
            int randChest = Random.Range(0, 100);
            if (randChest < 15)
            {
                chest = Instantiate(chest, roomCenter.position + Vector3.up, quaternion.identity);
            }
        }

        if (_hasPlayerEnteredRoom && _canActivateEnemies && roomType == 1)
        {
            _canActivateEnemies = false;
            DoorLock();
            StartCoroutine(ActivateAllEnemies());
        }

        if (isStelaActive)
        {
            isStelaActive = false;
            DoorLock();
            StartCoroutine(ActivateAllEnemies());
        }
        
        CheckPlayerPresence();
        //ActiveEffects();
    }

    void EnemyGeneration()
    {
        //determine etage
        int stage = _gameManager.currentLevel;
        int stageBonus = 0;
        int difficulty;
        //determine niveau de difficult??
        switch (stage)
        {
            case 0 :
                stageBonus = 0;
                break;
            case  1 : 
                stageBonus = 3;
                break;
            case  2 : 
                stageBonus = 6;
                break;
        }
        if (currentRoom < 3)
        {
            difficulty = 0;
        }
        else if (currentRoom < 5)
        {
            difficulty = 1;
        }
        else
        {
            difficulty = 2;
        }
        //determine le nombre d'ennemis devant spawn
        int enemyNumber = _lm.roomSpawnAmountMatrix[difficulty + stageBonus];
        //pour chaque ennemi
        for (int i = 0; i < enemyNumber; i++)
        {
            //determine les pourcentages d'apparition pour chacun
            int rand = Random.Range(0, 100);
            //determines les plafonds d'apparition pour les ennemis
            int wandererCeil = _lm.matrices[0].spawnMatrix[difficulty + stage];
            int bullCeil = _lm.matrices[1].spawnMatrix[difficulty + stage];
            int shooterCeil = _lm.matrices[2].spawnMatrix[difficulty + stage];
            int tankCeil = _lm.matrices[3].spawnMatrix[difficulty + stage];
            int mkCeil = _lm.matrices[4].spawnMatrix[difficulty + stage];
            
            List<int> randomEnemyType = new List<int>();
            //adds every enemy type
            List<int> possibleEnemies = new List<int>() {0, 1, 2, 3, 4};
            //adds every enemy probas
            List<int> enemyProbas = new List<int>();
            enemyProbas.Add(wandererCeil);
            enemyProbas.Add(bullCeil);
            enemyProbas.Add(shooterCeil);
            enemyProbas.Add(tankCeil);
            enemyProbas.Add(mkCeil);
            
            //pour chaque type d'ennemi diff de 0
            //pour le nombre de probas
            for (int j = 0; j < possibleEnemies.Count; j++)
            {
                for (int k = 0; k < enemyProbas[possibleEnemies[j]]; k++)
                {
                    randomEnemyType.Add(possibleEnemies[j]);
                }
            }

            int enemyType = randomEnemyType[rand];
            GameObject enemySpawning = Instantiate(_lm.basicEnemies[enemyType], enemyGroup.transform);
            
            //determines position
            //calculates a random position where the enemy will spawn
            float randPosX = Random.Range(-15, 15);
            float randPosY = Random.Range(-15, 15);
            var spawnPoint = roomCenter.position + new Vector3(randPosX, 1, randPosY);
            
            //spawns enemy
            enemySpawning.transform.position = spawnPoint;
            enemySpawning.transform.Rotate(0, -45, 0);
            enemySpawning.GetComponent<Enemy>().room = gameObject;
            enemySpawning.SetActive(false);
            
            //sets variables
            enemySpawning.GetComponent<Enemy>().health = _lm.matrices[enemyType].enemyValues[stage].x;
            enemySpawning.GetComponent<Enemy>().damage = _lm.matrices[enemyType].enemyValues[stage].y;
            enemySpawning.GetComponent<Enemy>().speed = _lm.matrices[enemyType].enemyValues[stage].z;
            enemySpawning.GetComponent<Enemy>().eyesLooted = _lm.matrices[enemyType].enemyValues[stage].w;
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
                PlacePlayerAtSpawnPoint();
                break;
            case 1 : //normal room
                EnemyGeneration();
                break;
            case 2 : //special room
                break;
            case 3 : //shop room
                ShopSpawn();
                break;
            case 4 : //mini-boss room
                GameObject stela = Instantiate(_lm.stela, roomCenter.position, Quaternion.identity);
                stela.GetComponent<ActiveStela>().room = this;
                GameObject exitPrefab = Instantiate(_lm.exit, roomCenter.position, Quaternion.identity);
                _lm.exit = exitPrefab;
                _lm.exit.SetActive(false);
                StelaSpawn();
                break;
            case 5 : //final boss room
                break;
        }
    }

    public void StelaSpawn()
    {
        //determine etage
        int stage = _gameManager.currentLevel;
        int stageBonus = 0;
        int enemyNumber = 0;
        //determine le nombre d'ennemis devant spawn
        switch (stage)
        {
            case 0:
                enemyNumber = 5;
                stageBonus = 0;
                break;
            case 1:
                enemyNumber = 6;
                stageBonus = 5;
                break;
        }

        //pour chaque ennemi
        for (int i = 0; i < enemyNumber; i++)
        {
            //Debug.Log("stela spawned enemy");
            //determine les pourcentages d'apparition pour chacun
            int rand = Random.Range(0, 100);
            //determines les plafonds d'apparition pour les ennemis
            int wandererCeil = _lm.stelaMatrices[0].spawnMatrix[stage];
            int bullCeil = _lm.stelaMatrices[1].spawnMatrix[stage];
            int shooterCeil = _lm.stelaMatrices[2].spawnMatrix[stage];
            int tankCeil = _lm.stelaMatrices[3].spawnMatrix[stage];
            int mkCeil = _lm.stelaMatrices[4].spawnMatrix[stage];

            List<int> randomEnemyType = new List<int>();
            //adds every enemy type
            List<int> possibleEnemies = new List<int>() { 0, 1, 2, 3, 4 };
            //adds every enemy probas
            List<int> enemyProbas = new List<int>();
            enemyProbas.Add(wandererCeil);
            enemyProbas.Add(bullCeil);
            enemyProbas.Add(shooterCeil);
            enemyProbas.Add(tankCeil);
            enemyProbas.Add(mkCeil);

            //pour chaque type d'ennemi diff de 0
            //pour le nombre de probas
            for (int j = 0; j < possibleEnemies.Count; j++)
            {
                for (int k = 0; k < enemyProbas[possibleEnemies[j]]; k++)
                {
                    randomEnemyType.Add(possibleEnemies[j]);
                }
            }

            int enemyType = randomEnemyType[rand];
            GameObject enemySpawning = Instantiate(_lm.basicEnemies[enemyType], enemyGroup.transform);

            //determines position
            //calculates a random position where the enemy will spawn
            float randPosX = Random.Range(-15, 15);
            float randPosY = Random.Range(-15, 15);
            var spawnPoint = roomCenter.position + new Vector3(randPosX, 1, randPosY);

            //spawns enemy
            enemySpawning.transform.position = spawnPoint;
            enemySpawning.transform.parent = enemyGroup.transform;
            enemySpawning.transform.Rotate(0, -45, 0);
            enemySpawning.GetComponent<Enemy>().room = gameObject;
            enemySpawning.SetActive(false);

            //sets variables
            enemySpawning.GetComponent<Enemy>().health = _lm.stelaMatrices[enemyType].enemyValues[stage].x;
            enemySpawning.GetComponent<Enemy>().damage = _lm.stelaMatrices[enemyType].enemyValues[stage].y;
            enemySpawning.GetComponent<Enemy>().eyesLooted = _lm.stelaMatrices[enemyType].enemyValues[stage].z;
            enemySpawning.GetComponent<Enemy>().speed = _lm.matrices[enemyType].enemyValues[stage].z;
        }
    }

    void ShopSpawn()
    {
        Instantiate(_lm.shop, roomCenter.position + Vector3.up * 7, quaternion.identity);
    }

    void CheckPlayerPresence()
    {
        _gameManager.playerRoom = currentRoom;
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

    public IEnumerator ActivateAllEnemies()
    {
        if (_objectsManager.noHit)
        {
            _objectsManager.noHitStreak = true;
        }
        for (int i = 0; i < enemyGroup.transform.childCount; i++)
        {
            enemyGroup.transform.GetChild(i).gameObject.SetActive(true);
            enemyGroup.transform.GetChild(i).gameObject.GetComponent<EnemyDamage>().enabled = true;
            Enemy enemy = enemyGroup.transform.GetChild(i).gameObject.GetComponent<Enemy>();
            enemy.enabled = true;
            enemy.StartCoroutine(enemy.EnemyApparition());
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
    void DoorLock()
    {
        foreach (var door in doorsObjects)
        {
            door.SetActive(true);
        }
    }

    void PlacePlayerAtSpawnPoint()
    {
        _player.transform.position = roomCenter.transform.position + Vector3.up * 1.65f;
    }

    void ActiveEffects()
    {
        if (_gameManager.playerRoom == currentRoom || _gameManager.playerRoom == currentRoom + 1 || _gameManager.playerRoom == currentRoom - 1)
        {
            visuals.SetActive(true);
        }
        else
        {
            visuals.SetActive(false);
        }
    }
}
