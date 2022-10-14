using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    //manages all prefab instanciations
    //updates infos regarding the level progression
    [Header("Generic Prefabs")]
    public GameObject[] basicEnemies;
    public GameObject[] miniBosses;
    public GameObject finalBoss;
    public GameObject chest; 
    public GameObject shop;
    public GameObject door;

    public int minEnemies = 3;
    public float populationGrowthFactor = 0.5f; //used for enemy population increase over time -- enemyspawnfactor

    public float roomSize;
    public int[] spawnMatrixUnlock = new int[5]; //--- spawnMatrixUnlock -- serves to tweak the number of rooms needed to unlock new enemies
    public List<GameObject> roomList;
    public GameObject entrance;
    public GameObject exit;
    public int currentLevel;

    private DunGen _dunGen;

    private void Awake()
    {
        _dunGen = GetComponent<DunGen>();
    }

    public void LoadNextLevel()
    {
        //keeps track of passed rooms : pass to next level
        currentLevel++;
        
        //disables all current rooms
        for (int i = 0; i < roomList.Count; i++)
        {
            roomList[i].SetActive(false);
            Destroy(roomList[i]);
        }
        
        //builds new level
        _dunGen.StartCoroutine(_dunGen.GenPro());
        
        //organises room order, so the list isn't made by spawn order, but by roomIndex
        roomList = new List<GameObject>(_dunGen.dungeonSize);

        //places player in first room
        GameObject.Find("Player").transform.position = entrance.transform.position;
    }
}
