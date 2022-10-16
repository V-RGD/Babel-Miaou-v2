using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using Random = UnityEngine.Random;

public class DunGen : MonoBehaviour
{
    public int goldenPathLength;
    public float branchProba;

    public List<GameObject> rooms;
    public GameObject goldenPathCheck;
    public GameObject branchCheck;

    private readonly int[,] _map = new int[100, 100];
    private int[,] _roomNumberMap = new int[100, 100];
    private int _roomToSpawnNumber = 1;
    public int dungeonSize;

    //components
    private NavMeshSurface _navMeshSurface;
    private LevelManager _lm;

    private void Start()
    {
        _navMeshSurface = GameObject.Find("NavMeshSurface").GetComponent<NavMeshSurface>();
        _lm = GetComponent<LevelManager>();
        StartCoroutine(GenPro());
    }

    #region Genpro
    public IEnumerator GenPro()
    {
        //set un offset
        float offset = 100;
        //choisit une diagonale
        int randDiagonale = Random.Range(0, 4);
        int dirX = 0;
        int dirY = 0;
        switch (randDiagonale)
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
            yield return new WaitForSeconds(0.01f);
            //GameObject roomChecker = Instantiate(goldenPathCheck, new Vector3(tblX * offset, 0, tblY * offset) - new Vector3(500, 0, 500), Quaternion.Euler(90, 0, 0));
            //roomChecker.SetActive(true);
        }
        dungeonSize -= 1;

        
        #region Branches
        //adds multiple branches
        for (int a = 0; a < 100; a++)
        {
            for (int b = 0; b < 100; b++)
            {
                //checks if the location has a room prepared
                if (_map[a, b] == 1)
                {
                    int randBranch = Random.Range(0, 100);
                    if (randBranch > branchProba)
                    {
                        //for each exit, check if a room is prepared
                        List<int> potentialDirections = new List<int>();
                        if (_map[a + 1, b] == 0)
                        {
                            potentialDirections.Add(0);
                        }
        
                        if (_map[a, b + 1] == 0)
                        {
                            potentialDirections.Add(1);
                        }
        
                        if (_map[a - 1, b] == 0)
                        {
                            potentialDirections.Add(2);
                        }
        
                        if (_map[a, b - 1] == 0)
                        {
                            potentialDirections.Add(3);
                        }
                        
                        int branchOffsetX = 0;
                        int branchOffsetY = 0;
        
                        //adds a random one where available
                        int randDir = potentialDirections[Random.Range(0, potentialDirections.Count)];
                        switch (randDir)
                        {
                            case 0:
                                _map[a + 1, b] = 1;
                                _roomNumberMap[a + 1, b] = 999; 
                                branchOffsetX = 1;
                                break;
                            case 1:
                                _map[a, b + 1] = 1;
                                branchOffsetY = 1;
                                _roomNumberMap[a, b + 1] = 999;
                                break;
                            case 2:
                                _map[a - 1, b] = 1;
                                branchOffsetX = -1;
                                _roomNumberMap[a - 1, b] = 999; 
                                break;
                            case 3:
                                _map[a, b - 1] = 1;
                                branchOffsetY = -1;
                                _roomNumberMap[a, b - 1] = 999;
                                break;
                        }
        
                        yield return new WaitForSeconds(0.01f);
                        //GameObject roomChecker = Instantiate(branchCheck, new Vector3((a + branchOffsetX) * offset, 0, (b + branchOffsetY) * offset) - new Vector3(5000, 0, 5000), Quaternion.Euler(90, 0, 0));
                        //roomChecker.SetActive(true);
                    }
                }
            }
        }
        #endregion
        
        
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

                    //pour chaque salle définie, instancier les salles correspondantes pour relier le donjon
                    foreach (GameObject room in rooms)
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

                    //just checking if the room isn't completely surrounded by others
                    if (potentialRooms.Count != 0)
                    {
                        GameObject roomPrepared = potentialRooms[Random.Range(0, potentialRooms.Count)];
                        GameObject roomSpawning = Instantiate(roomPrepared, new Vector3(i * offset, 0, j * offset) - new Vector3(5000, 0, 5000), Quaternion.Euler(90, 0, 0));
                        //giving it a proper name
                        roomSpawning.name = "Room" + _roomNumberMap[i, j];
                        roomSpawning.GetComponent<Room>().currentRoom = _roomNumberMap[i, j];
                        //adding room to the list
                        _lm.roomList.Add(roomSpawning);
                        roomSpawning.SetActive(true);
                    }
                    yield return new WaitForSeconds(0.01f);
                }
            }
        }
        #endregion
        _navMeshSurface.BuildNavMesh();
    }
    #endregion
}
