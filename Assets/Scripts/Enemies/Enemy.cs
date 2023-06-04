using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemies
{
    public class Enemy : MonoBehaviour
    {
        //basic properties that every enemy shares
        public float attackValue;
        public float maxHealth;

        //this is called whenever the player enters the room where the enemy is located
        public virtual void Spawn()
        {
            
        }
    }
}

