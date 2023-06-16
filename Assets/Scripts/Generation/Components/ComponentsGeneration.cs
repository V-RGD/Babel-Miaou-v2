using System;
using System.Collections.Generic;
using Generation.Level;
using Generation.Rooms;
using UnityEngine;

namespace Generation.Components
{
    public class ComponentsGeneration : MonoBehaviour
    {
        public static ComponentsGeneration instance;
        
        [Header("---Values---")] 
        [SerializeField] float doorHeightOffset = 1;
        [Header("---References---")] 
        [SerializeField] Transform roomsParent;
        [SerializeField] StartRoom startRoomPrefab;
        [SerializeField] FightRoom fightRoomPrefab;
        [SerializeField] ShopRoom shopRoomPrefab;
        [SerializeField] BossRoom bossRoomPrefab;
        [SerializeField] StairsRoom stairsRoomPrefab;
        
        void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
        }

        public void CreateRoomComponents()
        {
            List<GenProPlanner.RoomGenerationInfo> buffer = GenProPlanner.instance.roomBuffer;

            int fightRoomsIndex = 0;
            int shopRoomsIndex = 0;

            foreach (var room in buffer)
            {
                switch (room.type)
                {
                    case GenProPlanner.RoomType.StartingPoint:
                        StartRoom startRoom = Instantiate(startRoomPrefab, roomsParent);
                        startRoom.OnGeneration();
                        break;
                    case GenProPlanner.RoomType.FightArea:
                        FightRoom fightRoom = Instantiate(fightRoomPrefab, roomsParent);
                        fightRoomsIndex++;
                        fightRoom.name = "FightRoom N° " + fightRoomsIndex;
                        //generates enemies for the room
                        EnemyGen.instance.GenerateEnemies(fightRoom);
                        //creates and assigns doors depending on orientation
                        fightRoom.doors = CreateDoors(room);
                        fightRoom.OnGeneration();
                        break;
                    case GenProPlanner.RoomType.ShopRoom:
                        ShopRoom shopRoom = Instantiate(shopRoomPrefab, roomsParent);
                        shopRoom.name = "ShopRoom N° " + shopRoomsIndex;
                        shopRoom.OnGeneration();
                        break;
                    case GenProPlanner.RoomType.BossRoom:
                        BossRoom bossRoom = Instantiate(bossRoomPrefab, roomsParent);
                        bossRoom.OnGeneration();
                        break;
                    case GenProPlanner.RoomType.ExitStairs:
                        StairsRoom exitRoom = Instantiate(stairsRoomPrefab, roomsParent);
                        exitRoom.OnGeneration();
                        break;
                }
            }
        }

        [SerializeField] Animator doorPrefab;
        Animator[] CreateDoors(GenProPlanner.RoomGenerationInfo room)
        {
            //creates and assigns doors depending on orientation
            Animator[] doors = new Animator[2];
            for (int i = 0; i < 2; i++)
            {
                //creates door
                Animator door = Instantiate(doorPrefab, GenProBuilder.instance.TileToWorldPos(room.doorTiles[i] + room.instantiationTile) + Vector3.up * doorHeightOffset, Quaternion.identity);
                doors[i] = door;
            }

            return doors;
        }
    }
}