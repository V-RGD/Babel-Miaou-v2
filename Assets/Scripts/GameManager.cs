using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject player;
    public Vector2 startPos;
    public float height;
    public int currentLevel = 0;
    public LevelGeneration levelGeneration;
    public bool hasReachedTheEnd;

    private void Awake()
    {
        player = GameObject.Find("Player");
        levelGeneration = GameObject.Find("LevelManager").GetComponent<LevelGeneration>();
    }

    // Start is called before the first frame update
    void Start()
    {
        startPos = player.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateHeight();
        CurrentLevelUpdate();
    }

    void UpdateHeight()
    {
        height = player.transform.position.y - startPos.y;
    }

    void CurrentLevelUpdate()
    {
        if (height < levelGeneration.levelLimits[1])
        {
            currentLevel = 1;
        }
        if (height >= levelGeneration.levelLimits[1] && height < levelGeneration.levelLimits[2])
        {
            currentLevel = 2;
        }
        if (height >= levelGeneration.levelLimits[2] && height < levelGeneration.levelLimits[3])
        {
            currentLevel = 3;
        }
        if (height >= levelGeneration.levelLimits[3] && height < levelGeneration.levelLimits[4])
        {
            currentLevel = 4;
        }
        if (height >= levelGeneration.levelLimits[4] && height < levelGeneration.levelLimits[5])
        {
            currentLevel = 5;
        }
        if (height >= levelGeneration.levelLimits[5] && height < levelGeneration.levelLimits[6])
        {
            currentLevel = 6;
        }
        if (height >= levelGeneration.levelLimits[6] && height < levelGeneration.levelLimits[7])
        {
            currentLevel = 7;
        }
        if (height >= levelGeneration.levelLimits[7] && height < levelGeneration.levelLimits[8])
        {
            currentLevel = 8;
        }
        if (height >= levelGeneration.levelLimits[8] && height < levelGeneration.levelLimits[9])
        {
            currentLevel = 9;
        }
        /*if (height >= levelGeneration.levelLimits[9])
        {
            hasReachedTheEnd = true;
        }*/
    }
}
