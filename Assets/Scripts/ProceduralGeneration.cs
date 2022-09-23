using Unity.AI.Navigation;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class ProceduralGeneration : MonoBehaviour
{
    public int size;
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
    public GameObject[] bosses;
    public GameObject shop;

    private GameObject[] roomsToSpawn;
    
    public Vector3 startPos;

    public float fourRoomRate;
    public float twoRoomRate;
    public float threeRoomRate;
    public float offset;
    public int roomRotation;
    public int lastRoomRotation;

    public bool canGenerate;
    
    public int nextObject;
    public int generatingRoomNumber;

    public int lastDoorPos;

    public Vector3 enterPos;


    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        //generate level's lenght
        size = Random.Range(mixSize, maxSize + 1);
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

    void RotateRoom()
    {
        lastRoomRotation = roomRotation;
        if (lastDoorPos == 1)
        {
            roomRotation += -90;
        }
        if (lastDoorPos == 2)
        {
            roomRotation += 180;
        }
        if (lastDoorPos == 3)
        {
            roomRotation += 90;
        }
    }

    void RoomGeneration()
    {
        canGenerate = false;
        //to update info
        gameManager.currentRoom = generatingRoomNumber;

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
                roomsToSpawn = threeExits;
            }
            RotateRoom();
            //rotate room in the right direction
            GameObject room = Instantiate(roomsToSpawn[Random.Range(0, rooms.Length)], startPos, Quaternion.Euler(0, roomRotation, 0));
            //keep track of the reward desired
        }

        if (generatingRoomNumber == size - 1)
        {
            //spawns shop
            GameObject room = Instantiate(roomsToSpawn[Random.Range(0, rooms.Length)], startPos, Quaternion.identity);
        }
        
        if (generatingRoomNumber == size)
        {
            //spawns exit
            GameObject room = Instantiate(onExitRoom[Random.Range(0, rooms.Length)], startPos, Quaternion.identity);
        }
        //keep track of room generated
        generatingRoomNumber++;
    }
}
