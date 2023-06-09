using System;
using UnityEngine;
[CreateAssetMenu(fileName = "NewRoomGenerationValues", menuName = "RoomGenerationValues")]
[Serializable] public class RoomGenerationValues
{
    public Vector2Int size;
    public Type type;
    public enum Type
    {
        StartingPoint, FightArea, ShopRoom, BossRoom, ExitStairs
    }
}
