using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;
    
    //manages all prefab instanciations
    //updates infos regarding the level progression
    [Header("Generic Prefabs")]
    public GameObject[] basicEnemies;
    //public GameObject[] miniBosses;
    public GameObject chest; 
    public GameObject shop;
    public GameObject door;
    public GameObject stela;
    
    public float roomSize;
    public List<GameObject> roomList;
    public GameObject entrance;
    public GameObject exit;
    public int currentLevel;
    public PropsGenProfile propsProfile;
    
    //how much enemy will spawn in the room
    public List<int> roomSpawnAmountMatrix = new List<int>(9);
    [Serializable]public class EnemyMatrix
    {
        //what probability does this enemy have to spawn
        public List<int> spawnMatrix = new List<int>(9);
        //stats depending on the stage, and difficulty
        public List<Vector4> enemyValues = new List<Vector4>(4);
    }
    
    [Serializable]public class EnemyStelaMatrix
    {
        //what probability does this enemy have to spawn
        public List<int> spawnMatrix = new List<int>(2);
        //stats depending on the stage, and difficulty
        public List<Vector3> enemyValues = new List<Vector3>(4);
    }

    public List<EnemyMatrix> matrices;
    public List<EnemyStelaMatrix> stelaMatrices;

    private DunGen _dunGen;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        instance = this;
        
        _dunGen = DunGen.instance;
    }

    public void LoadNextLevel()
    {
        //keeps track of passed rooms : pass to next level
        currentLevel++;
        //disables all current rooms
        for (int i = 0; i < roomList.Count; i++)
        {
            GameObject objectToRemove = roomList[i];
            roomList.Remove(roomList[i]);
            Destroy(objectToRemove);
        }
        //builds new level
        _dunGen.StartCoroutine(_dunGen.GenPro());
        //places player in first room
        GameObject.Find("Player").transform.position = new Vector3(entrance.transform.position.x, 1, entrance.transform.position.z);
    }
}
