using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Generation
{
    [ExecuteInEditMode]
    public class GenPro : MonoBehaviour
    {
        public bool generate;

        #region References
        [Header("--References--")]
        [SerializeField] private Transform levelParent;
        [SerializeField] private GameObject buildingBlock;
        #endregion
        
        #region Values
        [Header("--Generation Values--")]
        [SerializeField] private Vector2Int gridSize;
        [SerializeField] private Vector2Int roomSize;
        [SerializeField] private Vector2Int roomDistance;
        [SerializeField] private Vector2Int fightRoomsAmount;
        [SerializeField] private int highestConsecutiveFights;
        #endregion

        #region InternVariables
        private int[,] _buildingGrid;
        private Vector2Int[] _roomPoses;
        private Vector2Int _nextRoomPos;
        private List<RoomGenerationInfo> _roomGenerationInfo;
        private RoomRepartition _roomRepartition;
        private struct RoomRepartition
        {
            private int _fightRoomsAmount;
            private int _shopRoomsAmount;
        }
        
        private struct RoomGenerationInfo
        {
            private Vector2Int _centerPosition;
            public Vector2Int _minMaxSize;
            public float _chaos;
            private Type _type;
            private enum Type
            {
                StartingPoint,
                FightArea,
                ShopRoom,
                BossRoom,
                ExitStairs
            }
        }
        #endregion
        
        private void Update()
        {
            if (generate)
            {
                generate = false;
                DestroyLevelInstance();
                ProceduralLevelGeneration();
            }
        }

        //builds terrain
        void ProceduralLevelGeneration()
        {
            //goal : create a terrain to navigate onto, that contains several rooms where events will appear
            //experience goal : create a terrain that seems natural, made by a civilisation, and feels good to explore
            //for this we will need several rooms linked by runaways, and rooms to simply enjoy the level art
            //the rooms are always created on top of the last one, so the level always progresses from low to high altitude, and from down to up
            
            //process :
            // - create a grid with ones and zeros (one equals to block to build, zero equals to none), resets generation info
            Initiate();
            //room repartition to know in which amount will come every type of room
            GenerateRoomRepartition();
            //then creates areas for each room, and builds then procedurally (random edges for variety, sizes that varies)
            RoomAreas();
            //then links each room with a bridge
            AddBridges();
            //then builds the level with actual blocks
            BuildLevel();
            //for visualisation purposes
            VisualiseGrid();
            Debug.Log("Created level grid");
        }

        void Initiate()
        {
            //initiates grid
            _buildingGrid = new int[gridSize.x,gridSize.y];
        }
        
        void GenerateRoomRepartition()
        {
            // - select a number of rooms to spawn, knowing that there has to be : 
            // - one room for the start 
            // - several rooms where fights occur
            // - at least one shop per level (depending on the length)
            // - one boss room at the end
            // - one or two rooms for exploration
            // - and one for the end with the stairs
            
            _roomGenerationInfo = new List<RoomGenerationInfo>();
            
            //calculates the number of fight rooms to create for this level
            int fightRoomsCount = Random.Range(fightRoomsAmount.x, fightRoomsAmount.y);
            //then creates sections to distributes them evenly, to avoid encountering too many at once
            int sectionsAmount = Mathf.CeilToInt((float)fightRoomsCount / (float)highestConsecutiveFights);
            int[] sections = new int[sectionsAmount];
            //then distributes them
            int roomsLeft = fightRoomsCount;
            for (int i = 0; i < sectionsAmount; i++)
            {
                if (roomsLeft < highestConsecutiveFights)
                {
                    sections[i] = roomsLeft;
                    break;
                }
                else
                {
                    sections[i] = highestConsecutiveFights;
                    roomsLeft -= highestConsecutiveFights;
                }
            }

            //there will be exactly one shop by section
            int shopRooms = sectionsAmount;
            
            // - one room for the start 
            _roomGenerationInfo.Add(new RoomGenerationInfo());
            
            // - several rooms where fights occur + shops
            //for each section
            for (int i = 0; i < sectionsAmount; i++)
            {
                //adds every fight rooms supposed to appear
                for (int j = 0; j < sections[i]; j++)
                {
                    _roomGenerationInfo.Add(new RoomGenerationInfo());
                }
                //then adds a shop
                _roomGenerationInfo.Add(new RoomGenerationInfo());
            }
            
            // - one boss room at the end
            _roomGenerationInfo.Add(new RoomGenerationInfo());
            // - and one for the end with the stairs
            _roomGenerationInfo.Add(new RoomGenerationInfo());
        }
        
        void RoomAreas()
        {
            //stars with the start room, then builds every other room going from the previous to the next and until stairs room
            for (int i = 0; i < _roomGenerationInfo.Count; i++)
            {
                GenerateRoom(_roomGenerationInfo[i], _nextRoomPos, i);
            }
        }

        void GenerateRoom(RoomGenerationInfo roomInfo, Vector2Int center, int index)
        {
            //--generates room
            
            //creates a round shape with points, and randomised a bit the edges and the center based on the chaos variable
            Vector2Int centerPosition = _nextRoomPos;
            Vector2Int minMaxSize = roomInfo._minMaxSize;
            float chaos = roomInfo._chaos;
            
            //--then sets the point where the next room will be instantiated
            
            //checks if the next room will fit inside of the grid (left and right)
            int maxExtentDist = minMaxSize.y * 2 + roomDistance.y;

            int maxX = _buildingGrid.GetLength(0);
            
            //if this is the last room, we do not need to calculate the position of the next room
            if (index >= _roomGenerationInfo.Count)
            {
                return;
            }
            
            bool canGoUp = centerPosition.x - maxExtentDist >= 0;
            bool canGoRight = centerPosition.x + maxExtentDist <= maxX;
            if (!canGoUp && !canGoRight)
            {
                Debug.LogError("Couldn't find a position for the next room");
                return;
            }
            
            //then chooses direction
            List<int> choices = new List<int>();
            if (canGoUp) choices.Add(-1);
            if (canGoRight) choices.Add(1);
            int dir = choices[Random.Range(0, choices.Count)];

            //if goes up
            if (dir == -1) _nextRoomPos += new Vector2Int(0, Random.Range(roomDistance.x, roomDistance.y));
            //if goes right
            if (dir == 1) _nextRoomPos += new Vector2Int(Random.Range(roomDistance.x, roomDistance.y), 0);
        }

        void AddBridges()
        {
            //links every room center to make sure that the player can actually cross between then

            //bridge initial width
            
            //check extents, then builds it
            
            //add noise to destroy some parts, but make sure that the bridge hasn't got any holes
            
        }

        void BuildLevel()
        {
            //uses grid to build the floor of the level, then adds a procedural collider to match the mesh
        }

        void VisualiseGrid()
        {
            //is used to know which parts of the grid are actually going to be built
            
            //creates a building block for each rendered part of the grid, and places it accordingly to its position on the grid
            for (int i = 0; i < gridSize.x; i++)
            {
                for (int j = 0; j < gridSize.y; j++)
                {
                    //if the block is supposed to be ground, continue operation
                    if (_buildingGrid[i, j] != 1) continue;
                    Vector3 position = new Vector3(i, 0, j);
                    GameObject brick = Instantiate(buildingBlock, position, quaternion.identity);
                    brick.transform.parent = levelParent;
                }
            }
        }

        public void DestroyLevelInstance()
        {
            //destroys grid and level
            foreach (Transform child in levelParent)
            {
                DestroyImmediate(child);
            }
        }
    }
}
