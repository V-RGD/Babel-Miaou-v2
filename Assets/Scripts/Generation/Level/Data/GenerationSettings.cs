using Generation.Rooms;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Generation.Level
{
    [CreateAssetMenu(fileName = "GenerationSettings", menuName = "Generation/GenerationSettings")]
    public class GenerationSettings : ScriptableObject
    {
        [Title("--- Level Settings ---", "Values defining how the generation works", TitleAlignments.Centered)]
        [Title("", "Room Generation")]
        public Vector2Int fightRoomsAmount = new(5, 7);

        public int highestConsecutiveFights = 3;

        [Title("", "Generation Scale")] public float tileSize = 10;
        public float tileSizeRatio = 0.1f;

        [Title("", "Room Positioning")] public Vector2Int roomDistance = new(8, 11);
        public int bridgeWidth = 1;

        [Title("", "Offsets")] public float wallTopOffset = 25;
        public float exteriorWallsOffset = -5;
        public float groundWallOffset = 10;
        public float doorHeightOffset = 5;

        [Title("--- Instantiation ---", "Elements created and used in the game", TitleAlignments.Centered)]
        [Title("", "Tiles")]
        public GameObject groundTile;
        public GameObject topWallTile;
        public GameObject bridgeTile;

        [Title("", "Walls")] public GameObject groundWallTile;
        public GameObject exteriorWallsTile;
        public GameObject bridgeWallTile;

        [Title("", "Rooms")] public StartRoom startRoomPrefab;
        public FightRoom fightRoomPrefab;
        public ShopRoom shopRoomPrefab;
        public BossRoom bossRoomPrefab;
        public StairsRoom stairsRoomPrefab;

        [Title("", "Elements")] public Animator doorPrefab;
        public Animator stairs;

        [Title("", "Enemies")] 
        public Enemies.Enemy wanderer;
        public Enemies.Enemy bull;
        public Enemies.Enemy shooter;
        public Enemies.Enemy mk;
    }
}