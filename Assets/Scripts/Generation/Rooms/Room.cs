using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Generation
{
    public class Room : MonoBehaviour
    {
        public Vector3 roomCenter;
        //manages enemy spawn, loot drop, door closing, boss spawning, and stairs appearing, shop as well
        public virtual void OnGeneration()
        {
            
        }
    }
}

