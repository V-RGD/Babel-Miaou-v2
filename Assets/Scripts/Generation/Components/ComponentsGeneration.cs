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
        
        [Header("---Parents---")] 
        [SerializeField] Transform roomsParent;
        [SerializeField] Transform doorParent;
        [SerializeField] public Transform shopParent;
        [SerializeField] public Transform interactionsParent;
        
        
        [Header("---Components---")]
        [SerializeField] StartRoom startRoomPrefab;
        [SerializeField] FightRoom fightRoomPrefab;
        [SerializeField] ShopRoom shopRoomPrefab;
        [SerializeField] BossRoom bossRoomPrefab;
        [SerializeField] StairsRoom stairsRoomPrefab;
        [SerializeField] Animator doorPrefab;

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
                        StartRoom startRoom = InitiateRoom(startRoomPrefab, room);
                        break;
                    case GenProPlanner.RoomType.FightArea:
                        fightRoomsIndex++;
                        InitiateFightRoom(fightRoomsIndex, room);
                        break;
                    case GenProPlanner.RoomType.ShopRoom:
                        shopRoomsIndex++;
                        InitiateShop(shopRoomsIndex, room);
                        break;
                    case GenProPlanner.RoomType.BossRoom:
                        BossRoom bossRoom = InitiateRoom(bossRoomPrefab, room);
                        break;
                    case GenProPlanner.RoomType.ExitStairs:
                        StairsRoom exitRoom = InitiateRoom(stairsRoomPrefab, room);
                        break;
                }
            }
        }

        //creates a room and assigns useful info to the script
        T InitiateRoom<T>(T prefab, GenProPlanner.RoomGenerationInfo roomInfo) where T : Room
        {
            T room = Instantiate(prefab, roomsParent);
            room.transform.parent = roomsParent;
            room.roomCenter = GridUtilities.TileToWorldPos(roomInfo.centerTile);
            room.OnGeneration();
            return room;
        }

        //creates a shop and renames it
        void InitiateShop(int index, GenProPlanner.RoomGenerationInfo roomInfo)
        {
            ShopRoom shopRoom = InitiateRoom(shopRoomPrefab, roomInfo);
            shopRoom.name = "ShopRoom " + index;
        }

        //creates a fight room, renames it, then generates enemies and door
        void InitiateFightRoom(int index, GenProPlanner.RoomGenerationInfo roomInfo)
        {
            FightRoom fightRoom = InitiateRoom(fightRoomPrefab, roomInfo);
            fightRoom.name = "FightRoom " + index;
            //generates enemies for the room
            EnemyGen.instance.GenerateEnemies(fightRoom);
            //creates and assigns doors depending on orientation
            fightRoom.doors = CreateDoors(roomInfo);
        }

        Animator[] CreateDoors(GenProPlanner.RoomGenerationInfo room)
        {
            //creates and assigns doors depending on orientation
            Animator[] doors = new Animator[2];
            for (int i = 0; i < 2; i++)
            {
                //creates door
                Animator door = Instantiate(doorPrefab, GridUtilities.TileToWorldPos(room.doorTiles[i] + room.instantiationTile) + Vector3.up * doorHeightOffset, Quaternion.identity);
                door.transform.parent = doorParent;
                doors[i] = door;
            }

            return doors;
        }
    }
}