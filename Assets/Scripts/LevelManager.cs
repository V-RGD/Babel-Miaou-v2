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

    public int minEnemies = 3;
    public float populationGrowthFactor = 0.5f; //used for enemy population increase over time -- enemyspawnfactor

    public float roomSize;
    public int[] spawnMatrixUnlock = new int[5]; //--- spawnMatrixUnlock -- serves to tweak the number of rooms needed to unlock new enemies
    public int roomPlaying;
}
