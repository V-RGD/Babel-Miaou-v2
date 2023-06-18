using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
[CreateAssetMenu(fileName = "RoomPlans", menuName = "Generation/RoomPlans")]

public class RoomPlans : SerializedScriptableObject
{
    //make a page for each x element
    [TabGroup("Start", Icon = SdfIconType.PlayFill, TextColor = "green")] public List<Sprite> startRooms = new List<Sprite>();
    [TabGroup("Fights", Icon = SdfIconType.LightningFill, TextColor = "orange")] public List<Sprite> fightRooms = new List<Sprite>();
    [TabGroup("Shops", Icon = SdfIconType.Coin, TextColor = "yellow")] public List<Sprite> shopRooms = new List<Sprite>();
    [TabGroup("Bosses", Icon = SdfIconType.EmojiAngryFill, TextColor = "red")] public List<Sprite> bossRooms = new List<Sprite>();
    [TabGroup("Stairs", Icon = SdfIconType.DoorOpenFill, TextColor = "blue")] public List<Sprite> stairsRooms = new List<Sprite>();
}
