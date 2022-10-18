using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LittleShit : MonoBehaviour
{
    //si oeil, courts vers l'oeil
    public List<Transform> eyesInGame;
    private Animator _animator;
    public int eyesInInventory;
    public NavMeshAgent agent;
    private GameObject _player;
    private Rigidbody _rb;

    private void Awake()
    {
        _animator = transform.GetChild(0).GetComponent<Animator>();
        _player = GameObject.Find("Player");
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        //si oeil, va vers l'oeil
        if (eyesInGame.Count > 0)
        {
            agent.SetDestination(eyesInGame[0].position);
            agent.stoppingDistance = 0.1f;
        }
        else
        {
            //si pas d'oeils, va vers le joueur
            agent.SetDestination(_player.transform.position);
            agent.stoppingDistance = 7f;
        }

        if ((_player.transform.position - transform.position).magnitude > 60 && eyesInGame.Count > 0)
        {
            transform.position = new Vector3(_player.transform.position.x, transform.position.y,
                _player.transform.position.z);
        }
        
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
