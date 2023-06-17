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
        
        GenerationSettings _settings;
        void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            _settings = LevelPlanner.instance.generationSettings;
        }

        public void CreateRoomComponents()
        {
            List<LevelPlanner.RoomGenerationInfo> buffer = LevelPlanner.instance.roomBuffer;

            int fightRoomsIndex = 0;
            int shopRoomsIndex = 0;

            foreach (var room in buffer)
            {
                switch (room.type)
                {
                    case LevelPlanner.RoomType.StartingPoint:
                        StartRoom startRoom = InitiateRoom(_settings.startRoomPrefab, room);
                        break;
                    case LevelPlanner.RoomType.FightArea:
                        fightRoomsIndex++;
                        InitiateFightRoom(fightRoomsIndex, room);
                        break;
                    case LevelPlanner.RoomType.ShopRoom:
                        shopRoomsIndex++;
                        InitiateShop(shopRoomsIndex, room);
                        break;
                    case LevelPlanner.RoomType.BossRoom:
                        BossRoom bossRoom = InitiateRoom(_settings.bossRoomPrefab, room);
                        break;
                    case LevelPlanner.RoomType.ExitStairs:
                        StairsRoom exitRoom = InitiateRoom(_settings.stairsRoomPrefab, room);
                        break;
                }
            }
        }

        //creates a room and assigns useful info to the script
        T InitiateRoom<T>(T prefab, LevelPlanner.RoomGenerationInfo roomInfo) where T : Room
        {
            T room = Instantiate(prefab, LevelBuilder.instance.roomsParent);
            room.transform.parent = LevelBuilder.instance.roomsParent;
            room.roomCenter = GridUtilities.TileToWorldPos(roomInfo.centerTile);
            room.OnGeneration();
            return room;
        }

        //creates a shop and renames it
        void InitiateShop(int index, LevelPlanner.RoomGenerationInfo roomInfo)
        {
            ShopRoom shopRoom = InitiateRoom(_settings.shopRoomPrefab, roomInfo);
            shopRoom.name = "ShopRoom " + index;
        }

        //creates a fight room, renames it, then generates enemies and door
        void InitiateFightRoom(int index, LevelPlanner.RoomGenerationInfo roomInfo)
        {
            FightRoom fightRoom = InitiateRoom(_settings.fightRoomPrefab, roomInfo);
            fightRoom.name = "FightRoom " + index;
            //generates enemies for the room
            EnemyGen.instance.GenerateEnemies(fightRoom);
            //creates and assigns doors depending on orientation
            fightRoom.doors = CreateDoors(roomInfo);
        }

        Animator[] CreateDoors(LevelPlanner.RoomGenerationInfo room)
        {
            //creates and assigns doors depending on orientation
            Animator[] doors = new Animator[2];
            for (int i = 0; i < 2; i++)
            {
                //creates door
                Animator door = Instantiate(_settings.doorPrefab, GridUtilities.TileToWorldPos(room.doorTiles[i] + room.instantiationTile) + Vector3.up * _settings.doorHeightOffset, Quaternion.identity);
                door.transform.parent = LevelBuilder.instance.doorParent;
                doors[i] = door;
            }

            return doors;
        }
    }
}