using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Generation.Level;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Generation
{
    public class FightRoom : Room
    {
        //this room locks door when player enters and reopens when has killed every enemy, dropping a chest
        [Header("Variables")]
        [SerializeField] float chestDropRate = 1;
        [SerializeField] float playerDetectionDist = 20;
        [SerializeField] float enemySpawnOffset;
        float _playerDist;
        int _killCount;
        bool _hasBeenEntered;
        bool _isCompleted;

        [Header("References")]
        [SerializeField] GameObject chestPrefab;
        public List<Enemies.Enemy> enemies;
        Transform _playerTransform;
        [HideInInspector] public Animator[] doors;
        [HideInInspector] public Vector2Int[] groundTiles;
        public override void OnGeneration()
        {
            _playerTransform = Player.Controller.instance.transform;
        }

        void Update()
        {
            _playerDist = (_playerTransform.position - transform.position).magnitude;
            
            //if the player hasn't reach out this room until yet, checks it's position to activate when needed
            if (_playerDist < playerDetectionDist && !_hasBeenEntered)
            {
                _hasBeenEntered = true;
                OnPlayerEnter();
            }
            
            //if the player has entered the room but hasn't killed every enemy yet, checks the kill count until the job is done
            if (_hasBeenEntered && _killCount < enemies.Count && !_isCompleted)
            {
                _isCompleted = true;
                OnComplete();
            }
        }

        //spawns enemies and locks doors
        void OnPlayerEnter() => StartCoroutine(EnemySpawning());

        void OnComplete()
        {
            //unlocks doors and spawns a chest
            foreach (var door in doors)
            {
                door.CrossFade("Open", 0, 0);
            }

            if (Random.Range(0f, 1f) < chestDropRate) Instantiate(chestPrefab, transform.position, quaternion.identity);
        }

        IEnumerator EnemySpawning()
        {
            //locks doors
            foreach (var door in doors)
            {
                door.CrossFade("Close", 0, 0);
            }
            //places each enemy on one of the tiles of the room
            List<Vector2Int> groundTilesAvailable = groundTiles.ToList();
            
            //spawn enemies
            foreach (var enemy in enemies)
            {
                Vector2Int tile = groundTilesAvailable[Random.Range(0, groundTilesAvailable.Count)];
                Vector3 spawnPoint = GenProBuilder.instance.TileToWorldPos(tile) + Vector3.up * enemySpawnOffset;
                StartCoroutine(enemy.Spawn());
                yield return new WaitForSeconds(0.5f);
            }
        }
    }
}

