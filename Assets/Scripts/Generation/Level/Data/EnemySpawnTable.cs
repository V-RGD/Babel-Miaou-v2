using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Generation.Level.Data
{
    [CreateAssetMenu(fileName = "EnemySpawnTable", menuName = "Generation/EnemySpawnTable")]
    public class EnemySpawnTable : SerializedScriptableObject
    {
        public List<RoomData> data = new List<RoomData>();
        public class RoomData
        {
            [HorizontalGroup] public bool wanderers;
            [HorizontalGroup] public bool bulls;
            [HorizontalGroup] public bool shooters;
            [HorizontalGroup] public bool marksmen;

            [ShowIf("wanderers"), TabGroup("Wanderers", TextColor = "blue")]
            public GenerationValues wandererValues;
            
            [ShowIf("bulls"), TabGroup("Bulls", TextColor = "red")]
            public GenerationValues bullValues;

            [ShowIf("shooters"), TabGroup("Shooters", TextColor = "orange")]
            public GenerationValues shooterValues;

            [ShowIf("marksmen"), TabGroup("Marksmen", TextColor = "purple")]
            public GenerationValues marksmenValues;
        }
        public struct GenerationValues
        {
            [PropertySpace(SpaceAfter = 10)]
            public Vector2Int amountToSpawn;
            //values applied to the enemy when spawning
            public int health;
            public int attack;
        }

        public enum EnemyType
        {
            Wanderer,
            Bull,
            Shooter,
            Mk
        }
    }
}