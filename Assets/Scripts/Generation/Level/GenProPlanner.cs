using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Generation.Level
{
    //this script is about deciding which rooms will be made, and where
    public class GenProPlanner : MonoBehaviour
    {
        public static GenProPlanner instance;

        [Header("--RoomValues--")] [SerializeField]
        List<Sprite> startRoomPlans;

        [SerializeField] List<Sprite> fightRoomPlans;
        [SerializeField] List<Sprite> shopRoomPlans;
        [SerializeField] List<Sprite> bossRoomPlans;
        [SerializeField] List<Sprite> stairsRoomPlans;

        [Header("--Generation Values--")] [SerializeField]
        Vector2Int roomDistance;

        [SerializeField] Vector2Int fightRoomsAmount;
        [SerializeField] int highestConsecutiveFights = 3;

        public List<RoomGenerationInfo> roomBuffer;

        public class RoomGenerationInfo
        {
            public RoomType type;

            //these are used to store room info in world grid coordinates
            public Vector2Int entryTile;
            public Vector2Int exitTile;
            //this is used to store the position where the room is supposed to generate
            public Vector2Int instantiationTile;
            public Vector2Int centerTile;
            public Sprite plan;
        }

        public enum RoomType
        {
            StartingPoint,
            FightArea,
            ShopRoom,
            BossRoom,
            ExitStairs
        }

        public enum RoomExitDir
        {
            Up,
            Right,
            Error
        }

        public enum RoomEntranceDir
        {
            Down,
            Left,
            Error
        }

        RoomExitDir _lastRoomExitDir;
        Vector2Int _nextRoomPos;
        Vector2Int _lastRoomExit;

        void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
        }

        void Start()
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
            GenProShaper.instance.ShapeLevel();
        }

        void Initiate()
        {
            //destroys level instance if a level is already built
            GenProBuilder.instance.DestroyLevelInstance();
            //sets the max grid size at the max distance that can be reached
            GenProBuilder.instance.buildingGrid = new int[400, 400];
            GenProBuilder.instance.heightMap = new int[400, 400];
        }

        void BuildRoomBuffer()
        {
            //resets buffer
            roomBuffer = new List<RoomGenerationInfo>();

            //calculates the number of fight rooms to create for this level
            int fightRoomsCount = Random.Range(fightRoomsAmount.x, fightRoomsAmount.y);

            //then creates sections to distributes them evenly, to avoid encountering too many at once
            int sectionsAmount = Mathf.CeilToInt((float) fightRoomsCount / (float) highestConsecutiveFights);
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
            AddNewRoom(RoomType.StartingPoint);

            //- several rooms where fights occur + shops
            //for each section
            for (int i = 0; i < sectionsAmount; i++)
            {
                //adds every fight rooms supposed to appear
                for (int j = 0; j < sections[i]; j++)
                {
                    AddNewRoom(RoomType.FightArea);
                }

                //then adds a shop
                AddNewRoom(RoomType.ShopRoom);
            }

            //- one boss room at the end
            AddNewRoom(RoomType.BossRoom);

            //- and one for the end with the stairs
            AddNewRoom(RoomType.ExitStairs);
        }

        void PlaceRooms()
        {
            //takes the room type
            foreach (var room in roomBuffer)
            {
                Sprite plan = startRoomPlans[0];
                //checks the last room orientation
                RoomEntranceDir neededDir = RoomEntranceDir.Error;
                switch (_lastRoomExitDir)
                {
                    case RoomExitDir.Up:
                        neededDir = RoomEntranceDir.Down;
                        break;
                    case RoomExitDir.Right:
                        neededDir = RoomEntranceDir.Left;
                        break;
                    case RoomExitDir.Error :
                        Debug.LogError("Error initializing last room exit dir");
                        break;
                }

                //picks a random plan between those available of the corresponding type
                switch (room.type)
                {
                    case RoomType.StartingPoint:
                        plan = startRoomPlans[Random.Range(0, startRoomPlans.Count)];
                        break;
                    case RoomType.FightArea:
                        plan = GridUtilities.FindSpriteOfEntranceType(fightRoomPlans, neededDir);
                        break;
                    case RoomType.ShopRoom:
                        plan = GridUtilities.FindSpriteOfEntranceType(shopRoomPlans, neededDir);
                        break;
                    case RoomType.BossRoom:
                        plan = GridUtilities.FindSpriteOfEntranceType(bossRoomPlans, neededDir);
                        break;
                    case RoomType.ExitStairs:
                        plan = GridUtilities.FindSpriteOfEntranceType(stairsRoomPlans, neededDir);
                        break;
                }

                //scans the plan to find the entry and exit tiles
                int[,] grid = MaskConverter.MaskToGrid(plan.texture);
                Vector2Int entryTile = Vector2Int.zero;
                Vector2Int exitTile = Vector2Int.zero;
                if (room.type != RoomType.StartingPoint)
                {
                    entryTile = GridUtilities.GetTilesOfIndex(grid, 2)[0];
                }

                if (room.type != RoomType.ExitStairs)
                {
                    exitTile = GridUtilities.GetTilesOfIndex(grid, 3)[0];
                }
                
                //selects a tile at a reasonable distance from the last exit)
                Vector2Int roomEntrance = Vector2Int.zero;
                //the first room is placed at the very beginning
                if (room.type != RoomType.StartingPoint)
                {
                    int distance = Random.Range(roomDistance.x, roomDistance.y);
                    if (_lastRoomExitDir == RoomExitDir.Right)
                    {
                        roomEntrance = _lastRoomExit + new Vector2Int(distance, 0);
                    }
                    if (_lastRoomExitDir == RoomExitDir.Up)
                    {
                        roomEntrance = _lastRoomExit + new Vector2Int(0, distance);
                    }
                }
                
                Vector2Int instantiationPosition = roomEntrance - entryTile;
                Vector2Int centerTile = GridUtilities.GetTilesOfIndex(grid, 4)[0];
                Vector2Int roomCenter = instantiationPosition + centerTile;
                Vector2Int roomExit = instantiationPosition + exitTile;

                //send info to the buffer
                
                //the position where the bridge is supposed to end at : entry pos
                room.entryTile = roomEntrance;
                //the position where the room will be instantiated : position where the bridge stops - the offset of the entrance
                room.instantiationTile = instantiationPosition;
                //the position where the exit of the room is located : instantiation pos + exit offset
                room.exitTile = roomExit;
                //the tile at the center of the room
                room.centerTile = roomCenter;
                
                room.plan = plan;
                //updates planning info to create the next room at the right position
                _lastRoomExit = roomExit;
                if (room.type != RoomType.ExitStairs) _lastRoomExitDir = GridUtilities.CheckExitType(room.plan);
            }
        }

        void AddNewRoom(RoomType type)
        {
            RoomGenerationInfo newInfo = new RoomGenerationInfo();
            newInfo.type = type;
            roomBuffer.Add(newInfo);
        }
    }
}