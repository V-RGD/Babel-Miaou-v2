using System;
using System.Collections.Generic;
using UnityEngine;
namespace Generation
{
    [CreateAssetMenu(fileName = "NewEnemyGenerationProperties", menuName = "EnemyGenerationProperties")]
    //This is a compilation of every enemy type in the game, including presets for each
    //Think this as an encyclopedia of the stats of the bestiary including every variants like the bokoblins and moblins in botw
    public class EnemyGenProperties : ScriptableObject
    {
        //for each enemy, manages health, damage, and spawn rates
        [Serializable] public class EnemyType
        {
            public string name;
            public Enemies.Enemy prefab;
            //used to easily create variants of enemies
            public List<Preset> presets;
            [Serializable] public class Preset
            {
                public string name;
                public int health;
                public int attack;
                public float spawnRate;
            }
        }

        public List<Enemy> enemies;
    }

    public class EnemyGenerationInfo
    {
        //this is sent to each room to determine what enemies are gonna spawn, with the corresponding stats
        [Serializable] public class EnemySpawnInfo
        {
            public Enemies.Enemy prefab;
            public int health;
            public int attack;
        }
        public List<EnemySpawnInfo> spawnInfos;
    }
}
