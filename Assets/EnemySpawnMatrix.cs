using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnMatrix : MonoBehaviour
{
    private GameManager _gameManager;
    
    //how much enemy will spawn in the room
    public int[,] roomSpawnAmountMatrix = new int[3,3];
    public class EnemyMatrix
    {
        //what probability does this enemy have to spawn
        public int[,] spawnMatrix = new int[3,3];
        //it's stat depending on the stage, and difficulty
        public int[,] healthMatrix = new int[3,3];
        public int[,] damageMatrix = new int[3,3];
        public int[,] eyesSpawnedMatrix = new int[3,3];
        public int[,] speedMatrix = new int[3,3];
    }

    public List<EnemyMatrix> matrices;

    public int stage;
    public int roomNumber;
    public int difficulty;

    void EnemySpawn()
    {
        // //determine etage
        // stage = _gameManager.currentStage;
        // //determine niveau de difficulté
        // if (_gameManager.currentRoom < 3)
        // {
        //     difficulty = 1;
        // }
        // else if (_gameManager.currentRoom < 5)
        // {
        //     difficulty = 2;
        // }
        // else
        // {
        //     //_gameManager.currentRoom > 5 and < 8
        //     difficulty = 3;
        // }
        // //determine le nombre d'ennemis devant spawn
        // int enemyNumber = roomSpawnAmountMatrix[stage, difficulty];
        // //pour chaque ennemi
        // for (int i = 0; i < enemyNumber; i++)
        // {
        //     //determine les pourcentages d'apparition pour chacun
        //     int enemySpawning = Random.Range(0, 100);
        //     if (enemySpawning < matrices[0].spawnMatrix)
        //     {
        //         //spawn enemi
        //         //break
        //     }
        // }
        // //pour chaque ennemi, choisir de façon random ceux qui vont spawn
        // //détermine les stats de chacun (Vie, dégats, vitesse, yeux lootés
        //
        //
        // //pour chaque type d'ennemi
        // for (int i = 0; i < 5; i++)
        // {
        //     
        // }
    }
}
