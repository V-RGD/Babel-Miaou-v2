using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class Room : MonoBehaviour
{
    //manages what spawns in the room, and what events happen (doors, events, stairs)

    [Header("Generic Prefabs")] public GameObject chest;
    [HideInInspector] public GameObject doorPrefab;
    [HideInInspector] public List<GameObject> doorsObjects;
    [HideInInspector] public List<Animator> doorsAnimators;

    //components
    UIManager _uiManager;
    DunGen _dunGen;
    GameManager_old _gameManager;
    ObjectsManager_old _objectsManager;
    RoomInfo _roomInfo;
    public Transform roomCenter;
    public Transform safeSpot;

    //room info
    public int roomType;
    public int currentRoom;

    //values
    GameObject _player;
    [SerializeField] GameObject visuals;
    [SerializeField] GameObject empty;
    [HideInInspector] public GameObject enemyGroup;
    GameObject stela;
    [HideInInspector] public bool isStelaActive;
    int _enemiesRemaining;
    bool _canChestSpawn = true;
    bool _canActivateEnemies = true;
    bool _hasPlayerEnteredRoom;
    const float
        RoomDetectZoneSize =
            0.4f; //gave up finding a name --- the percentage of the room which detects the player if it's close from the center

    IEnumerator Start()
    {
        //component assignations
        _player = GameObject.Find("Player");
        LevelManager.instance = LevelManager.instance;
        _dunGen = DunGen.instance;
        _objectsManager = ObjectsManager_old.instance;
        _gameManager = GameManager_old.instance;
        chest = LevelManager.instance.chest;
        doorPrefab = LevelManager.instance.door;
        _roomInfo = GetComponent<RoomInfo>();
        var group = Instantiate(empty);
        group.transform.Rotate(0, 45, 0);
        enemyGroup = group;
        group.name = gameObject.name + "Enemy Group";
        if (safeSpot == null) safeSpot = roomCenter;

        DoorSpawn();
        yield return new WaitUntil(() => _dunGen.finishedGeneration);
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
                //destroys stela
                StartCoroutine(StelaFadeOut());
                UIManager.instance.gameIndications.CrossFade(Animator.StringToHash("Room Cleared"), 0);
                UIManager.instance.indicationTxt.text = "Level Cleared";
                //spawns stairs
            }
            else
            {
                UIManager.instance.gameIndications.CrossFade(Animator.StringToHash("Room Cleared"), 0);
                UIManager.instance.indicationTxt.text = "Room Cleared";
                //randomize chest spawn
                var randChest = Random.Range(0, 100);
                if (randChest < 50) chest = Instantiate(chest, safeSpot.position + Vector3.up, quaternion.identity);
            }
        }

        if (_hasPlayerEnteredRoom && _canActivateEnemies && roomType == 1)
        {
            _canActivateEnemies = false;
            DoorLock();
            StartCoroutine(ActivateAllEnemies());
            if (LittleShit.instance != null) LittleShit.instance.TpToPlayer();
        }

        if (isStelaActive)
        {
            isStelaActive = false;
            DoorLock();
            StartCoroutine(ActivateAllEnemies());
            if (LittleShit.instance != null) LittleShit.instance.TpToPlayer();
        }

        CheckPlayerPresence();
        //ActiveEffects();
    }

    void EnemyGeneration()
    {
        //determine etage
        var stage = _gameManager.currentLevel;
        var stageBonus = 0;
        int difficulty;
        //determine niveau de difficulté
        switch (stage)
        {
            case 0:
                stageBonus = 0;
                break;
            case 1:
                stageBonus = 3;
                break;
            case 2:
                stageBonus = 6;
                break;
        }

        if (currentRoom < 3)
            difficulty = 0;
        else if (currentRoom < 5)
            difficulty = 1;
        else
            difficulty = 2;

        //determine le nombre d'ennemis devant spawn
        var enemyNumber = LevelManager.instance.roomSpawnAmountMatrix[difficulty + stageBonus];
        //pour chaque ennemi
        for (var i = 0; i < enemyNumber; i++)
        {
            //determine les pourcentages d'apparition pour chacun
            var rand = Random.Range(0, 100);
            //determines les plafonds d'apparition pour les ennemis
            var wandererCeil = LevelManager.instance.matrices[0].spawnMatrix[difficulty + stage];
            var bullCeil = LevelManager.instance.matrices[1].spawnMatrix[difficulty + stage];
            var shooterCeil = LevelManager.instance.matrices[2].spawnMatrix[difficulty + stage];
            var tankCeil = LevelManager.instance.matrices[3].spawnMatrix[difficulty + stage];
            var mkCeil = LevelManager.instance.matrices[4].spawnMatrix[difficulty + stage];

            var randomEnemyType = new List<int>();
            //adds every enemy type
            var possibleEnemies = new List<int>() {0, 1, 2, 3, 4};
            //adds every enemy probas
            var enemyProbas = new List<int>();
            enemyProbas.Add(wandererCeil);
            enemyProbas.Add(bullCeil);
            enemyProbas.Add(shooterCeil);
            enemyProbas.Add(tankCeil);
            enemyProbas.Add(mkCeil);

            //pour chaque type d'ennemi diff de 0
            //pour le nombre de probas
            for (var j = 0; j < possibleEnemies.Count; j++)
            for (var k = 0; k < enemyProbas[possibleEnemies[j]]; k++)
                randomEnemyType.Add(possibleEnemies[j]);

            var enemyType = randomEnemyType[rand];
            var enemySpawning = Instantiate(LevelManager.instance.basicEnemies[enemyType], enemyGroup.transform);

            //determines position
            //calculates a random position where the enemy will spawn
            float randPosX = Random.Range(-15, 15);
            float randPosY = Random.Range(-15, 15);
            var spawnPoint = roomCenter.position + new Vector3(randPosX, 1, randPosY);

            //spawns enemy
            enemySpawning.transform.position = spawnPoint;
            enemySpawning.transform.Rotate(0, -45, 0);
            enemySpawning.GetComponent<Enemy_old>().room = gameObject;
            enemySpawning.SetActive(false);

            //sets variables
            enemySpawning.GetComponent<Enemy_old>().health = LevelManager.instance.matrices[enemyType].enemyValues[stage].x;
            enemySpawning.GetComponent<Enemy_old>().damage = LevelManager.instance.matrices[enemyType].enemyValues[stage].y;
            enemySpawning.GetComponent<Enemy_old>().speed = LevelManager.instance.matrices[enemyType].enemyValues[stage].z;
            enemySpawning.GetComponent<Enemy_old>().eyesLooted =
                LevelManager.instance.matrices[enemyType].enemyValues[stage].w;
        }
    }

    IEnumerator StelaFadeOut()
    {
        LevelManager.instance.stela.transform.GetChild(1).GetComponent<Animator>()
            .CrossFade(Animator.StringToHash("Fade"), 0);
        yield return new WaitForSeconds(1);
        stela.SetActive(false);
        //activates colis stratégique des escaliers
        StartCoroutine(StairSpawn());
    }

    void RoomType()
    {
        if (currentRoom == 999) return;
        //roomType = 2;
        if (currentRoom == 0)
            roomType = 0;
        else if (currentRoom == _dunGen.dungeonSize)
            roomType = 4;
        else if (currentRoom == _dunGen.dungeonSize - 1)
            roomType = 3;
        else
            roomType = 1;

        //starting room doesn't have any enemies
        switch (roomType)
        {
            case 0: //start room
                LevelManager.instance.entrance.transform.position = roomCenter.position;
                PlacePlayerAtSpawnPoint();
                break;
            case 1: //normal room
                EnemyGeneration();
                break;
            case 2: //special room
                break;
            case 3: //shop room
                LevelManager.instance.currentShopPosition = transform.position;
                //ShopSpawn();
                break;
            case 4: //mini-boss room
                stela = Instantiate(LevelManager.instance.stela, safeSpot.position, Quaternion.identity);
                stela.GetComponent<ActiveStela>().room = this;
                LevelManager.instance.currentStelaPosition = safeSpot.position;
                //LevelManager.instance.currentStela = stela;
                //GameObject exitPrefab = Instantiate(LevelManager.instance.exit, roomCenter.position, Quaternion.identity);
                //LevelManager.instance.exit = exitPrefab;
                //LevelManager.instance.exit.SetActive(false);
                StelaSpawn();
                break;
            case 5: //final boss room
                break;
        }
    }

    public void StelaSpawn()
    {
        //determine etage
        var stage = _gameManager.currentLevel;
        var stageBonus = 0;
        var enemyNumber = 0;
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
        for (var i = 0; i < enemyNumber; i++)
        {
            //Debug.Log("stela spawned enemy");
            //determine les pourcentages d'apparition pour chacun
            var rand = Random.Range(0, 100);
            //determines les plafonds d'apparition pour les ennemis
            var wandererCeil = LevelManager.instance.stelaMatrices[0].spawnMatrix[stage];
            var bullCeil = LevelManager.instance.stelaMatrices[1].spawnMatrix[stage];
            var shooterCeil = LevelManager.instance.stelaMatrices[2].spawnMatrix[stage];
            var tankCeil = LevelManager.instance.stelaMatrices[3].spawnMatrix[stage];
            var mkCeil = LevelManager.instance.stelaMatrices[4].spawnMatrix[stage];

            var randomEnemyType = new List<int>();
            //adds every enemy type
            var possibleEnemies = new List<int>() {0, 1, 2, 3, 4};
            //adds every enemy probas
            var enemyProbas = new List<int>();
            enemyProbas.Add(wandererCeil);
            enemyProbas.Add(bullCeil);
            enemyProbas.Add(shooterCeil);
            enemyProbas.Add(tankCeil);
            enemyProbas.Add(mkCeil);

            //pour chaque type d'ennemi diff de 0
            //pour le nombre de probas
            for (var j = 0; j < possibleEnemies.Count; j++)
            for (var k = 0; k < enemyProbas[possibleEnemies[j]]; k++)
                randomEnemyType.Add(possibleEnemies[j]);

            var enemyType = randomEnemyType[rand];
            var enemySpawning = Instantiate(LevelManager.instance.basicEnemies[enemyType], enemyGroup.transform);

            //determines position
            //calculates a random position where the enemy will spawn
            float randPosX = Random.Range(-15, 15);
            float randPosY = Random.Range(-15, 15);
            var spawnPoint = roomCenter.position + new Vector3(randPosX, 1, randPosY);

            //spawns enemy
            enemySpawning.transform.position = spawnPoint;
            enemySpawning.transform.parent = enemyGroup.transform;
            enemySpawning.transform.Rotate(0, -45, 0);
            var enemyComponent = enemySpawning.GetComponent<Enemy_old>();
            enemyComponent.room = gameObject;
            enemySpawning.SetActive(false);

            //sets variables
            enemyComponent.health = LevelManager.instance.stelaMatrices[enemyType].enemyValues[stage].x;
            enemyComponent.damage = LevelManager.instance.stelaMatrices[enemyType].enemyValues[stage].y;
            enemyComponent.eyesLooted = LevelManager.instance.stelaMatrices[enemyType].enemyValues[stage].z;
            enemyComponent.speed = LevelManager.instance.matrices[enemyType].enemyValues[stage].z;
            enemyComponent.isFromStela = true;
        }
    }

    void ShopSpawn()
    {
        Instantiate(LevelManager.instance.shop, roomCenter.position + Vector3.up * 7, quaternion.identity);
    }

    void CheckPlayerPresence()
    {
        _gameManager.playerRoom = currentRoom;
        var position = roomCenter.position;
        var roomDetectionXMax = position.x + LevelManager.instance.roomSize * RoomDetectZoneSize;
        var roomDetectionXMin = position.x - LevelManager.instance.roomSize * RoomDetectZoneSize;
        var roomDetectionZMax = position.z + LevelManager.instance.roomSize * RoomDetectZoneSize;
        var roomDetectionZMin = position.z - LevelManager.instance.roomSize * RoomDetectZoneSize;

        if (_player.transform.position.x < roomDetectionXMax && _player.transform.position.x > roomDetectionXMin &&
            _player.transform.position.z > roomDetectionZMin && _player.transform.position.z < roomDetectionZMax)
        {
            _hasPlayerEnteredRoom = true;
            LevelManager.instance.currentRoom = gameObject;
        }
        else
        {
            _hasPlayerEnteredRoom = false;
        }
    }

    IEnumerator ActivateAllEnemies()
    {
        if (_objectsManager.noHit)
        {
            _objectsManager.noHitStreak = true;
            _objectsManager.noHitFx.Play();
        }

        for (var i = 0; i < enemyGroup.transform.childCount; i++)
        {
            enemyGroup.transform.GetChild(i).gameObject.SetActive(true);
            enemyGroup.transform.GetChild(i).gameObject.GetComponent<EnemyDamage>().enabled = true;
            var enemy = enemyGroup.transform.GetChild(i).gameObject.GetComponent<Enemy_old>();
            enemy.enabled = true;
            enemy.StartCoroutine(enemy.EnemyApparition());
            yield return new WaitForSeconds(0.5f);
        }

        for (var i = 0; i < enemyGroup.transform.childCount; i++)
            if (!enemyGroup.transform.GetChild(i).gameObject.activeInHierarchy)
            {
                enemyGroup.transform.GetChild(i).gameObject.SetActive(true);
                enemyGroup.transform.GetChild(i).gameObject.GetComponent<EnemyDamage>().enabled = true;
                var enemy = enemyGroup.transform.GetChild(i).gameObject.GetComponent<Enemy_old>();
                enemy.enabled = true;
                enemy.StartCoroutine(enemy.EnemyApparition());
                yield return new WaitForSeconds(0.5f);
            }
    }

    void DoorSpawn()
    {
        for (var i = 0; i < 4; i++)
            //checks if there is supposed to be doors on each corner
            if (_roomInfo.doors[i] == 1)
            {
                //spawns doors accordingly
                var cornerOffset = -15;
                var upOffset = 0;
                var roomSize = 50 / 2;
                var spawnPoint = Vector3.zero;
                var rotation = Vector3.zero;
                switch (i)
                {
                    case 0: //left
                        spawnPoint = new Vector3(-roomSize + cornerOffset, upOffset, 0);
                        rotation = new Vector3(0, -90, 0);
                        break;
                    case 1: //up
                        spawnPoint = new Vector3(0, upOffset, roomSize - cornerOffset);
                        rotation = new Vector3(0, 0, 0);
                        break;
                    case 2: //right
                        spawnPoint = new Vector3(roomSize - cornerOffset, upOffset, 0);
                        rotation = new Vector3(0, 90, 0);
                        break;
                    case 3: //down
                        spawnPoint = new Vector3(0, upOffset, -roomSize + cornerOffset);
                        rotation = new Vector3(0, 180, 0);
                        break;
                }

                var door = Instantiate(doorPrefab, spawnPoint + roomCenter.position, Quaternion.Euler(rotation));
                door.transform.parent = gameObject.transform;
                door.SetActive(false);
                doorsObjects.Add(door);
                doorsAnimators.Add(door.transform.GetChild(0).transform.GetChild(0).GetComponent<Animator>());
            }
    }

    void DoorUnlock()
    {
        for (var i = 0; i < doorsObjects.Count; i++)
            StartCoroutine(DoorOpenCoroutine(doorsAnimators[i], doorsObjects[i]));
    }

    void DoorLock()
    {
        for (var i = 0; i < doorsObjects.Count; i++)
            StartCoroutine(DoorCloseCoroutine(doorsAnimators[i], doorsObjects[i]));
    }

    IEnumerator DoorOpenCoroutine(Animator doorAnimator, GameObject door)
    {
        doorAnimator.CrossFade(Animator.StringToHash("Open"), 0);
        yield return new WaitForSeconds(1.5f);
        door.GetComponent<BoxCollider>().enabled = false;
        yield return new WaitForSeconds(0.5f);
        door.SetActive(false);
    }

    IEnumerator DoorCloseCoroutine(Animator doorAnimator, GameObject door)
    {
        door.SetActive(true);
        doorAnimator.CrossFade(Animator.StringToHash("Close"), 0);
        yield return new WaitForSeconds(0.1f);
    }

    void PlacePlayerAtSpawnPoint()
    {
        _player.transform.position = safeSpot.transform.position + Vector3.up * 2.1f;
    }

    void ActiveEffects()
    {
        if (_gameManager.playerRoom == currentRoom || _gameManager.playerRoom == currentRoom + 1 ||
            _gameManager.playerRoom == currentRoom - 1)
            visuals.SetActive(true);
        else
            visuals.SetActive(false);
    }

    IEnumerator StairSpawn()
    {
        var stairs = Instantiate(LevelManager.instance.exit, safeSpot.position,
            quaternion.identity);
        stairs.SetActive(true);
        stairs.transform.GetChild(1).transform.GetChild(0).GetComponent<Animator>()
            .CrossFade(Animator.StringToHash("Fall"), 0);
        yield return new WaitForSeconds(0.4f);
        stairs.transform.GetChild(0).GetComponent<ToNextLevel>().isActive = true;
    }
}