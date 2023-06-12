using System.Collections;
using System.Collections.Generic;
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
        float _playerDist;
        int _killCount;
        bool _hasBeenEntered;
        bool _isCompleted;

        [Header("References")]
        [SerializeField] EnemyGenProperties genProperties;
        [SerializeField] GameObject chestPrefab;
        public List<Enemies.Enemy> enemies;
        Animator _doorsAnimator;
        Transform _playerTransform; 
        EnemyGenerationInfo _genInfos;

        public override void OnGeneration()
        {
            //generates enemies when created
            EnemyGeneration();
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
            _doorsAnimator.CrossFade("Open", 0, 0);
            if (Random.Range(0f, 1f) < chestDropRate) Instantiate(chestPrefab, transform.position, quaternion.identity);
        }

        void EnemyGeneration()
        {
            //for each enemy in it
            // foreach (var VARIABLE in COLLECTION)
            // {
            //     
            // }
            
            //instantiates enemies as asked, then applies values depending on the presets chosen for each
            foreach (var info in _genInfos.spawnInfos)
            {
                //instantiates enemy
                Enemies.Enemy newEnemy = Instantiate(info.prefab, transform);
                //gives it corresponding info
                newEnemy.attackValue = info.attack;
                newEnemy.maxHealth = info.health;
                //adds it to the room list
                enemies.Add(newEnemy);
                //disables enemy until entering room
                newEnemy.gameObject.SetActive(false);
            }
        }

        IEnumerator EnemySpawning()
        {
            //locks doors
            _doorsAnimator.CrossFade("Close", 0, 0);
            
            //spawn enemies
            foreach (var enemy in enemies)
            {
                StartCoroutine(enemy.Spawn());
                yield return new WaitForSeconds(0.5f);
            }
        }
    }
}

