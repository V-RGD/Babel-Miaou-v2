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

        public struct RoomData
        {
            [HorizontalGroup(Order = 0)] public bool wanderers;
            [HorizontalGroup(Order = 1)] public bool bulls;
            [HorizontalGroup(Order = 2)] public bool shooters;
            [HorizontalGroup(Order = 3)] public bool marksmen;

            [ShowIf("wanderers"), TabGroup("Wanderers", TextColor = "blue")]
            public GenerationValues wandererValues;

            [ShowIf("bulls"), TabGroup("Bulls", TextColor = "red")]
            public GenerationValues bullValues;

            [ShowIf("shooters"), TabGroup("Shooters", TextColor = "orange")]
            public GenerationValues shooterValues;

            [ShowIf("marksmen"), TabGroup("Marksmen", TextColor = "purple")]
            public GenerationValues marksmenValues;
        }

        public List<Sprite> enemySprites;

        public struct GenerationValues
        {
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

        [OnInspectorInit]
        void CreateData()
        {
        }
        // [DictionaryDrawerSettings(DisplayMode = DictionaryDisplayOptions.Foldout)]
        // public Dictionary<SomeEnum, MyCustomType> EnumObjectLookup = new Dictionary<SomeEnum, MyCustomType>()
        // {
        //     { SomeEnum.Third, new MyCustomType() },
        //     { SomeEnum.Fourth, new MyCustomType() },
        // };
    }
}