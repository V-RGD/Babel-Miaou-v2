using System;
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
        [SerializeField] int highestConsecutiveFights;

        public List<RoomGenerationInfo> roomBuffer;

        public class RoomGenerationInfo
        {
            public RoomType type;

            //these are used to store room info in local coordinates
            public Vector2Int entryPos;
            public Vector2Int exitPos;
            //this is used to store the position where the room is supposed to generate
            public Vector2Int generationPos;
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

        enum RoomExitDir
        {
            Up,
            Right
        }

        public enum RoomEntranceDir
        {
            Down,
            Left
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
                RoomEntranceDir neededDir = RoomEntranceDir.Down;
                switch (_lastRoomExitDir)
                {
                    case RoomExitDir.Up:
                        neededDir = RoomEntranceDir.Down;
                        break;
                    case RoomExitDir.Right:
                        neededDir = RoomEntranceDir.Left;
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
                Vector2Int posToCreate = Vector2Int.zero;
                //the first room is placed at the very beginning
                if (room.type != RoomType.StartingPoint)
                {
                    int distance = Random.Range(roomDistance.x, roomDistance.y);
                    if (_lastRoomExitDir == RoomExitDir.Right)
                    {
                        posToCreate = _lastRoomExit + new Vector2Int(distance, 0);
                    }
                    if (_lastRoomExitDir == RoomExitDir.Up)
                    {
                        posToCreate = _lastRoomExit + new Vector2Int(0, distance);
                    }
                }
                
                //send info to the buffer
                room.entryPos = entryTile;
                room.exitPos = exitTile;
                room.generationPos = posToCreate;
                room.plan = plan;
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