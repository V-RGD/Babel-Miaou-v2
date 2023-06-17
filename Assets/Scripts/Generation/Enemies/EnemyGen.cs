using System;
using System.Collections.Generic;
using Generation.Level;
using Generation.Level.Data;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Generation
{
    public class EnemyGen : MonoBehaviour
    {
        public static EnemyGen instance;
        
        GenerationSettings _settings;
        public EnemySpawnTable enemySpawnTable;

        int _roomGenerated;

        //this needs a great pacing if we want a balanced and interesting game
        void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            _settings = LevelPlanner.instance.generationSettings;
        }

        public void GenerateEnemies(FightRoom fightRoom)
        {
            /*
            
            //selects a batch of enemies
            EnemySpawnTable.FightRoomGenerationSettings room = enemySpawnTable.enemyRepartition[_roomGenerated];
            _roomGenerated++;
            
            //for each type of enemies in it
            foreach (var enemy in room.enemiesAvailable)
            {
                //picks a random amount
                int amount = Random.Range(enemy.amountToSpawn.x, enemy.amountToSpawn.y);
                Enemies.Enemy enemyPrefab = _settings.wanderer;
                switch (enemy.enemy)
                {
                    case EnemySpawnTable.FightRoomGenerationSettings.EnemyAvailable.EnemyType.Wanderer:
                        enemyPrefab = _settings.wanderer;
                        break;
                    case EnemySpawnTable.FightRoomGenerationSettings.EnemyAvailable.EnemyType.Bull:
                        enemyPrefab = _settings.bull;
                        break;
                    case EnemySpawnTable.FightRoomGenerationSettings.EnemyAvailable.EnemyType.Shooter:
                        enemyPrefab = _settings.shooter;
                        break;
                    case EnemySpawnTable.FightRoomGenerationSettings.EnemyAvailable.EnemyType.Mk:
                        enemyPrefab = _settings.mk;
                        break;
                }

                //creates, then adds the enemies to the room
                for (int i = 0; i < amount; i++)
                {
                    Enemies.Enemy newEnemy = Instantiate(enemyPrefab, fightRoom.transform);
                    //sets corresponding values
                    newEnemy.maxHealth = enemy.health;
                    newEnemy.attackValue = enemy.attack;
                    fightRoom.enemies.Add(newEnemy);
                }
            }
            */
        }
    }
}
