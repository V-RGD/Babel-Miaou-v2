using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

public class LittleShit : MonoBehaviour
{
    //si oeil, courts vers l'oeil
    private Animator _animator;
    public int eyesInInventory;
    public NavMeshAgent agent;
    private GameObject _player;
    private Rigidbody _rb;
    private GameManager _gameManager;
    private LevelManager _lm;

    private void Awake()
    {
        _animator = transform.GetChild(0).GetComponent<Animator>();
        _player = GameObject.Find("Player");
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        _gameManager = GameManager.instance;
        _lm = LevelManager.instance;
    }

    private void Update()
    {
        Direction();
        SpawnChest();
        Animation();
        TpToPlayer();
    }

    void Direction()
    {
        //si oeil, va vers l'oeil
        if (_gameManager.eyesInGame.Count > 0 && (_gameManager.eyesInGame[0].position - transform.position).magnitude < 35)
        {
            agent.SetDestination(_gameManager.eyesInGame[0].position);
            agent.stoppingDistance = 0.1f;
        }
        else
        {
            //si pas d'oeils, va vers le joueur
            agent.SetDestination(_player.transform.position);
            agent.stoppingDistance = 7f;
        }
    }

    void SpawnChest()
    {
        if (eyesInInventory > ObjectsManager.instance.gameVariables.eyeCollectorCollectCeil)
        {
            eyesInInventory -= ObjectsManager.instance.gameVariables.eyeCollectorCollectCeil;
            _lm.chest = Instantiate(_lm.chest, transform.position + Vector3.up, quaternion.identity);
        }
    }

    void TpToPlayer()
    {
        //teleports to player if too far away
        if ((_player.transform.position - transform.position).magnitude > 50 && _gameManager.eyesInGame.Count > 0)
        {
            transform.position = new Vector3(_player.transform.position.x, transform.position.y,
                _player.transform.position.z);
        }
    }

    void Animation()
    {
        if (!agent.pathPending)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    _animator.SetBool("Walk", false);
                }
            }
            else
            {
                _animator.SetBool("Walk", true);
            }
        }
    }
}
