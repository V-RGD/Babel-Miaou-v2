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

        [Header("---References---")] [SerializeField]
        Transform roomsParent;

        [Header("---References---")] [SerializeField]
        StartRoom startRoomPrefab;

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
                        EnemyGen.instance.GenerateEnemies(fightRoom);
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
    }
}