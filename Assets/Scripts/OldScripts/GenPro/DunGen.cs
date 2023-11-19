using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using Random = UnityEngine.Random;

public class DunGen : MonoBehaviour
{
    public static DunGen instance;
    public bool stopGen;
    
    public int goldenPathLength = 8;

    public List<GameObject> roomsLevel1;
    public List<GameObject> roomsLevel2;
    public List<GameObject> roomsLevel3;
    public List<GameObject> fullRooms;
    public List<GameObject> marchandRooms;
    public GameObject roomList;

    private int[,] _map = new int[100, 100];
    private int[,] _roomNumberMap = new int[100, 100];
    private int _roomToSpawnNumber = 1;
    public int dungeonSize;
    [HideInInspector]public bool finishedGeneration;

    //components
    private NavMeshSurface _navMeshSurface;
    private LevelManager _lm;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    private void Start()
    {
        _navMeshSurface = GameObject.Find("NavMeshSurface").GetComponent<NavMeshSurface>();
        _lm = GetComponent<LevelManager>();
        StartCoroutine(GenPro());
    }

    #region Genpro
    public IEnumerator GenPro()
    {
        yield return null;
        if (stopGen)
        {
            yield break;
        }
        
        roomList.transform.localEulerAngles = new Vector3(0, 0, 0);
        
        _map = new int[100, 100];
       _roomNumberMap = new int[100, 100];
        
        //set un offset
        float offset = 96f;
        //choisit une diagonale
        int randDiagonal = Random.Range(0, 4);
        int dirX = 0;
        int dirY = 0;
        switch (randDiagonal)
        {
            //upleft
            case 0 :
                dirX = -1; 
                dirY = 1;
                break;
            //upright
            case 1 :
                dirX = 1; 
                dirY = 1;
                break;
            //downright
            case 2 :
                dirX = 1; 
                dirY = -1;
                break;
            //downleft
            case 3 :
                dirX = -1; 
                dirY = -1;
                break;
        }
        
        //starting with every room being false
        //starting in the middle of the table
        int tblX = 50;
        int tblY = 50;
        //add start (middle) room to the table
        _map[50, 50] = 1;
        
        //prepares rooms to be instanciated
        for (int i = 1; i < goldenPathLength; i++)
        {
            //either go one way or the other, in the direction of the diagonal;
            int randDir = Random.Range(0, 2);
            switch (randDir)
            {
                case 0 :
                    tblX += dirX;
                    break;
                case 1 :
                    tblY += dirY;
                    break;
            }
            //add room
            _map[tblX, tblY] = 1;
            _roomNumberMap[tblX, tblY] = _roomToSpawnNumber; //used to track room ID for the golden path
            _roomToSpawnNumber++;
            dungeonSize = _roomToSpawnNumber;
            //yield return new WaitForSeconds(0.01f);
        }
        dungeonSize -= 1;

        #region instanciation
        //pour chaque salle définie, checker quelles sont les salles ajdacentes avec le systeme de tableau
        //for each location possible
        for (int i = 0; i < 100; i++)
        {
            for (int j = 0; j < 100; j++)
            {
                //checks if the location has a room prepared
                if (_map[i, j] == 1)
                {
                    //for each exit, check if a room is prepared
                    bool isThereARoomRight = false;
                    bool isThereARoomUp = false;
                    bool isThereARoomDown = false;
                    bool isThereARoomLeft = false;
                    List<GameObject> potentialRooms = new List<GameObject>();
                    if (_map[i + 1, j] == 1)
                    {
                        isThereARoomRight = true;
                    }

                    if (_map[i, j + 1] == 1)
                    {
                        isThereARoomUp = true;
                    }

                    if (_map[i, j - 1] == 1)
                    {
                        isThereARoomDown = true;
                    }

                    if (_map[i - 1, j] == 1)
                    {
                        isThereARoomLeft = true;
                    }

                    int currentRoom = _roomNumberMap[i, j];
                    
                    //checks level, and then type of the room
                    if (currentRoom == 0 || currentRoom == dungeonSize)
                    {
                        foreach (GameObject room in fullRooms)
                        {
                            //check if the room has same parameters
                            if (room.GetComponent<RoomInfo>().isThereARoomRight == isThereARoomRight &&
                                room.GetComponent<RoomInfo>().isThereARoomLeft == isThereARoomLeft
                                && room.GetComponent<RoomInfo>().isThereARoomUp == isThereARoomUp &&
                                room.GetComponent<RoomInfo>().isThereARoomDown == isThereARoomDown)
                            {
                                potentialRooms.Add(room);
                            }
                        }
                    }

                    if (currentRoom == dungeonSize - 1)
                    {
                        foreach (GameObject room in marchandRooms)
                        {
                            //check if the room has same parameters
                            if (room.GetComponent<RoomInfo>().isThereARoomRight == isThereARoomRight &&
                                room.GetComponent<RoomInfo>().isThereARoomLeft == isThereARoomLeft
                                && room.GetComponent<RoomInfo>().isThereARoomUp == isThereARoomUp &&
                                room.GetComponent<RoomInfo>().isThereARoomDown == isThereARoomDown)
                            {
                                potentialRooms.Add(room);
                            }
                        }
                    }
                    
                    else
                    {
                        if (LevelManager.instance.currentLevel == 0)
                        {
                            foreach (GameObject room in roomsLevel1)
                            {
                                //check if the room has same parameters
                                if (room.GetComponent<RoomInfo>().isThereARoomRight == isThereARoomRight &&
                                    room.GetComponent<RoomInfo>().isThereARoomLeft == isThereARoomLeft
                                    && room.GetComponent<RoomInfo>().isThereARoomUp == isThereARoomUp &&
                                    room.GetComponent<RoomInfo>().isThereARoomDown == isThereARoomDown)
                                {
                                    potentialRooms.Add(room);
                                }
                            }
                        }
                        if (LevelManager.instance.currentLevel == 1)
                        {
                            foreach (GameObject room in roomsLevel2)
                            {
                                //check if the room has same parameters
                                if (room.GetComponent<RoomInfo>().isThereARoomRight == isThereARoomRight &&
                                    room.GetComponent<RoomInfo>().isThereARoomLeft == isThereARoomLeft
                                    && room.GetComponent<RoomInfo>().isThereARoomUp == isThereARoomUp &&
                                    room.GetComponent<RoomInfo>().isThereARoomDown == isThereARoomDown)
                                {
                                    potentialRooms.Add(room);
                                }
                            }
                        }
                        if (LevelManager.instance.currentLevel == 2)
                        {
                            foreach (GameObject room in roomsLevel3)
                            {
                                //check if the room has same parameters
                                if (room.GetComponent<RoomInfo>().isThereARoomRight == isThereARoomRight &&
                                    room.GetComponent<RoomInfo>().isThereARoomLeft == isThereARoomLeft
                                    && room.GetComponent<RoomInfo>().isThereARoomUp == isThereARoomUp &&
                                    room.GetComponent<RoomInfo>().isThereARoomDown == isThereARoomDown)
                                {
                                    potentialRooms.Add(room);
                                }
                            }
                        }
                    }

                    //just checking if the room isn't completely surrounded by others
                    if (potentialRooms.Count != 0)
                    {
                        GameObject roomPrepared = potentialRooms[Random.Range(0, potentialRooms.Count)];
                        GameObject roomSpawning = Instantiate(roomPrepared, new Vector3(i * offset, 0, j * offset) - new Vector3(5000, 0, 5000), Quaternion.Euler(0, 0, 0));
                        //giving it a proper name
                        roomSpawning.name = "Room " + _roomNumberMap[i, j];
                        roomSpawning.GetComponent<Room>().currentRoom = _roomNumberMap[i, j];
                        //adding room to the list
                        _lm.roomList.Add(roomSpawning);
                        roomSpawning.transform.parent = roomList.transform;
                        roomSpawning.SetActive(true);
                    }
                    //yield return new WaitForSeconds(0.01f);
                }
            }
        }
        #endregion
        finishedGeneration = true;
        // yield return new WaitUntil(() => finishedGeneration);
        //roomList.transform.localEulerAngles = new Vector3(0, 90, 0);
        yield return null;
        if (roomList.transform.localEulerAngles.y < 45)
        {
            roomList.transform.Rotate(0, 45, 0);
        }
        
        _navMeshSurface.BuildNavMesh();
        GameMusic.instance.ChooseMusic();
        GameManager_old.instance.ChooseGlobalVolume();
    }
    #endregion
}
