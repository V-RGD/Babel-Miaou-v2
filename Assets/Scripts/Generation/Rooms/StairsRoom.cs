using Unity.Mathematics;
using UnityEngine;

namespace Generation.Rooms
{
    public class StairsRoom : Room
    {
        [SerializeField] GameObject stairs;
        [SerializeField] Vector3 stairsOffset;
        public override void OnGeneration()
        {
            //spawns stairs / end of the level
            Instantiate(stairs, roomCenter + stairsOffset, quaternion.identity);
        }
    }
}
