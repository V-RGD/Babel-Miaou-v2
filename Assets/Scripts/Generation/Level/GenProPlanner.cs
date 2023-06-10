using System;
using System.Collections.Generic;
using Player;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Generation
{
    //this script is about deciding which rooms will be made, and where
    public class GenProPlanner : MonoBehaviour
    {
        public static GenProPlanner instance;
        private GenProShaper _shaper;
        private GenProBuilder _builder;
        
        #region References
        [Header("--RoomValues--")]
        [SerializeField] private RoomGenerationValues startingRoomValues;
        [SerializeField] private RoomGenerationValues fightingRoomValues;
        [SerializeField] private RoomGenerationValues shopRoomValues;
        [SerializeField] private RoomGenerationValues bossRoomValues;
        [SerializeField] private RoomGenerationValues stairsRoomValues;
        #endregion
        
        #region Values
        [Header("--Generation Values--")]
        [SerializeField] private Vector2Int roomDistance;
        [SerializeField] private Vector2Int fightRoomsAmount;
        [SerializeField] private int highestConsecutiveFights;
        #endregion

        #region InternVariables
        public List<RoomGenerationInfo> roomBuffer;
        public class RoomGenerationInfo
        {
            public RoomGenerationValues generationValues;
            public Vector2Int centerPosition;
            public float chaos;
        }
        
        private Vector2Int _nextRoomPos;
        #endregion
        
        private void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            _shaper = GetComponent<GenProShaper>();
            _builder = GetComponent<GenProBuilder>();
        }

        private void Start()
        {
            ProceduralLevelGeneration();
        }

        //builds terrain
        void ProceduralLevelGeneration()
        {
            //Step 1 - create a grid with ones and zeros (one equals to block to build, zero equals to none), resets generation info
            Initiate();
            //Step 2 : calculates the amount of rooms about to be instantiated, then adds them in a buffer
            BuildRoomBuffer();
            //Step 3 : place each one of them on the grid in their respective positions
            PlaceRooms();
            //Step 4 : Send info to the Shaper
            _shaper.ShapeLevel();
        }

        void Initiate()
        {
            //destroys level instance if a level is already built
            _builder.DestroyLevelInstance();
            //sets the max grid size at the theoritical max distance that can be reached
            int maxGridSize = 0;
            maxGridSize += startingRoomValues.size.y;
            maxGridSize += fightingRoomValues.size.y * fightRoomsAmount.y;
            maxGridSize += shopRoomValues.size.y * ((fightRoomsAmount.y / highestConsecutiveFights) + 1);
            maxGridSize += bossRoomValues.size.y;
            maxGridSize += stairsRoomValues.size.y;
            int bridgesDist = roomDistance.y * (1 + fightRoomsAmount.y + (fightRoomsAmount.y / highestConsecutiveFights) + 1 + 1);
            maxGridSize += bridgesDist;
            _builder.buildingGrid = new int[maxGridSize, maxGridSize];
        }
        
        void BuildRoomBuffer()
        {
            //resets buffer
            roomBuffer = new List<RoomGenerationInfo>();
            
            //calculates the number of fight rooms to create for this level
            int fightRoomsCount = Random.Range(fightRoomsAmount.x, fightRoomsAmount.y);
            
            //then creates sections to distributes them evenly, to avoid encountering too many at once
            int sectionsAmount = Mathf.CeilToInt((float)fightRoomsCount / (float)highestConsecutiveFights);
            int[] sections = new int[sectionsAmount];
            //then distributes them
            int roomsLeft = fightRoomsCount;
            for (int i = 0; i < sectionsAmount; i++)
            {
                //if not enough rooms for the section, just adds the ones left
                if (roomsLeft < highestConsecutiveFights)
                {
                    sections[i] = roomsLeft;
                    break;
                }
                
                //if enough rooms left for the section, adds them
                sections[i] = highestConsecutiveFights;
                roomsLeft -= highestConsecutiveFights;
            }
            
            //-----------------------------Repartition-----------------------------------------
            
            //- one room for the start 
            AddNewRoom(startingRoomValues);
            
            //- several rooms where fights occur + shops
            //for each section
            for (int i = 0; i < sectionsAmount; i++)
            {
                //adds every fight rooms supposed to appear
                for (int j = 0; j < sections[i]; j++)
                {
                    AddNewRoom(fightingRoomValues);
                }
                //then adds a shop
                AddNewRoom(shopRoomValues);
            }
            
            //- one boss room at the end
            AddNewRoom(bossRoomValues);
            
            //- and one for the end with the stairs
            AddNewRoom(stairsRoomValues);
        }

        void PlaceRooms()
        {
            int baseOffset = Random.Range(roomDistance.x, roomDistance.y) + roomBuffer[0].generationValues.size.y;
            _nextRoomPos = Vector2Int.one * baseOffset;
            
            //sets the center position of each
            for (int i = 0; i < roomBuffer.Count; i++)
            {
                //Step 1 : chooses between up or right to place the next room
                int dir = Random.Range(0, 2);
                //Step 2 : updates the position depending on the direction taken
                //if goes up
                int distanceToPlace = Random.Range(roomDistance.x, roomDistance.y) + roomBuffer[i].generationValues.size.y;
                if (dir == 0) _nextRoomPos += new Vector2Int(0, distanceToPlace);
                //if goes right
                if (dir == 1) _nextRoomPos += new Vector2Int(distanceToPlace, 0);
                //then sets the position
                roomBuffer[i].centerPosition = _nextRoomPos;
            }
        }

        public void AddNewRoom(RoomGenerationValues roomValues)
        {
            RoomGenerationInfo newInfo = new RoomGenerationInfo();
            newInfo.generationValues = roomValues;
            roomBuffer.Add(newInfo);
        }
    }
}
