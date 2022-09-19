using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelGeneration : MonoBehaviour
{
    /*//decides how the level is designed

    public LevelManager levelManager;
    public Vector3 startPos;
    public Vector3 generatingPos;
    public Vector3 generationOffset;

    public int maxSize;
    public int minSize;

    public bool isGenerationFinished;
    public int[] levelLenghts;

    //grid components used to vary level generation
    public GameObject[] level1Parts;
    public GameObject[] level2Parts;
    public GameObject[] level3Parts;
    public GameObject[] level4Parts;
    public GameObject[] level5Parts;
    public GameObject[] level6Parts;
    public GameObject[] level7Parts;
    public GameObject[] level8Parts;
    public GameObject[] level9Parts;

    void Start()
    {
        //building procedure
        LevelGenerationPlanning();
    }

    void LevelGenerator(GameObject[] levelStems, int levelLenght)
    {
        //creates the level
        for (int i = 0; i < levelLenght; i++)
        {
            GameObject parcel = Instantiate(levelStems[Random.Range(0, levelStems.Length)], generatingPos, Quaternion.identity);
            parcel.transform.parent = GameObject.Find("Level1").transform;
            generatingPos += generationOffset;
        }
    }

    void LevelGenerationPlanning()
    {
        //creates the entire game based on the parts added to the inspector
        generatingPos = startPos;
        LevelGenerator(level1Parts, levelLenghts[0]);
        LevelGenerator(level2Parts, levelLenghts[1]);
        LevelGenerator(level3Parts, levelLenghts[2]);
        LevelGenerator(level4Parts, levelLenghts[3]);
        LevelGenerator(level5Parts, levelLenghts[4]);
        LevelGenerator(level6Parts, levelLenghts[5]);
        LevelGenerator(level7Parts, levelLenghts[6]);
        LevelGenerator(level8Parts, levelLenghts[7]);
        LevelGenerator(level9Parts, levelLenghts[8]);
        isGenerationFinished = true;

        levelLimits.Add(0);
        
        for (int i = 1; i < 9; i++)
        {
            levelLimits.Add(levelLenghts[i] * generationOffset.y + levelLimits[i - 1]);
        }
    }*/
}
