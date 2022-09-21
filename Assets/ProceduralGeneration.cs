using Unity.AI.Navigation;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class ProceduralGeneration : MonoBehaviour
{
    private int size;
    public int maxSize;
    public int mixSize;

    public GameObject[] onExitRoom;
    public GameObject[] fourExits;
    public GameObject[] threeExits;
    public GameObject[] twoExits;
    public GameObject[] rooms;
    public GameObject[] itemLoots;
    public GameManager gameManager;
    public GameObject[] enemies;

    private GameObject[] roomsToSpawn;
    
    public Vector3 startPos;
    public Vector3 generatorPos;

    public float fourRoomRate;
    public float twoRoomRate;
    public float threeRoomRate;
    public float offset;

    public bool canGenerate;
    
    public int nextObject;
    private int generatingRoomNumber;



    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        //generate level's lenght
        size = Random.Range(mixSize, maxSize + 1);
        generatorPos = startPos;
        //StartGeneration();
        generatingRoomNumber = 1;
    }

    private void Update()
    {
        if (canGenerate)
        {
            RoomGeneration();
        }
    }

    void StartGeneration()
    {
        GameObject room = Instantiate(onExitRoom[Random.Range(0, onExitRoom.Length)], transform.position, quaternion.identity);
        //make it an entrance
    }

    void RoomGeneration()
    {
        canGenerate = false;
        //to update info
        gameManager.currentRoom = generatingRoomNumber;
        Debug.Log("generated room");

        if (generatingRoomNumber < size - 1)
        {
            //roulette qui décide du type de salle a générer
            int randRoom = Random.Range(0, 10);
            if (randRoom <= twoRoomRate)
            {
                roomsToSpawn = twoExits;
            }
            if (randRoom > twoRoomRate && randRoom <= threeRoomRate)
            {
                roomsToSpawn = threeExits;
            }
            if (randRoom > threeRoomRate && randRoom <= fourRoomRate)
            {
                roomsToSpawn = fourExits;
            }
            GameObject room = Instantiate(roomsToSpawn[Random.Range(0, rooms.Length)], generatorPos, Quaternion.identity);
            //rotate room in the right direction
            room.transform.LookAt(new Vector3(room.transform.GetChild(0).transform.position.x, generatorPos.y, room.transform.GetChild(0).transform.position.z));
            //keep track of the reward desired
        }

        if (generatingRoomNumber == size - 1)
        {
            //spawns shop
            GameObject room = Instantiate(roomsToSpawn[Random.Range(0, rooms.Length)], generatorPos, Quaternion.identity);
        }
        
        if (generatingRoomNumber == size)
        {
            //spawns exit
            GameObject room = Instantiate(onExitRoom[Random.Range(0, rooms.Length)], generatorPos, Quaternion.identity);
        }
        //keep track of room generated
        generatingRoomNumber++;
    }
}
