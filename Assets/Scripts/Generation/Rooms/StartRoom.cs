using UnityEngine;

namespace Generation.Rooms
{
    public class StartRoom : Room
    {
        [SerializeField] Vector3 spawnOffset;
        public override void OnGeneration()
        {
            //places player at the center of the start room
            Player.Controller.instance.transform.position = roomCenter + spawnOffset;
            //when the generation ends, gives control to the player
            Player.Controller.instance.canMove = true;
        }
    }
}
