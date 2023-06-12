using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Generation
{
    public class EnemyGen : MonoBehaviour
    {
        public static EnemyGen instance;

        int _roomGenerated;

        [SerializeField] Enemies.Enemy wanderer;
        [SerializeField] Enemies.Enemy bull;
        [SerializeField] Enemies.Enemy shooter;
        [SerializeField] Enemies.Enemy mk;
        
        //this needs a great pacing if we want a balanced and interesting game
        public enum EnemyType
        {
            Wanderer,
            Bull,
            Shooter,
            Mk
        }
        [Serializable] public class EnemyToGenerate
        {
            public EnemyType enemy;
            public Vector2Int amountToSpawn;
        }
        
        [Serializable] public class EnemyBatch  
        {
            public List<EnemyToGenerate> enemiesAvailable;
        }

        public List<EnemyBatch> generationVariables;
        void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
        }

        public void GenerateEnemies(FightRoom fightRoom)
        {
            //selects a batch of enemies
            EnemyBatch batch = generationVariables[_roomGenerated];
            _roomGenerated++;
            
            //for each type of enemies in it
            foreach (var enemy in batch.enemiesAvailable)
            {
                //picks a random amount
                int amount = Random.Range(enemy.amountToSpawn.x, enemy.amountToSpawn.y);
                Enemies.Enemy enemyPrefab = wanderer;
                switch (enemy.enemy)
                {
                    case EnemyType.Wanderer:
                        enemyPrefab = wanderer;
                        break;
                    case EnemyType.Bull:
                        enemyPrefab = bull;
                        break;
                    case EnemyType.Shooter:
                        enemyPrefab = shooter;
                        break;
                    case EnemyType.Mk:
                        enemyPrefab = mk;
                        break;
                }

                //creates, then adds the enemies to the room
                for (int i = 0; i < amount; i++)
                {
                    Enemies.Enemy newEnemy = Instantiate(enemyPrefab, fightRoom.transform);
                    fightRoom.enemies.Add(newEnemy);
                }
            }
        }
    }
}
