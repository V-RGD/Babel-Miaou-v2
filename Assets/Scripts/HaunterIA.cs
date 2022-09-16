using System;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class HaunterIA : MonoBehaviour
{
    public float speed;
    public float health;
    public float damage;
    public float attackRate;
    private float playerDist;
    public float attackRange;

    private float speedFactor;

    public NavMeshAgent agent;
    public NavMeshSurface navMeshSurface;
    public GameObject player;

    public bool isInPlayerRange;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.Find("Player");
        navMeshSurface.BuildNavMesh();
    }

    private void Update()
    {
        agent.speed = speed * speedFactor;
        
        playerDist = (player.transform.position - transform.position).magnitude;

        //if the enemy is in player range
        if (playerDist <= attackRange)
        {
            isInPlayerRange = true;
            //stops then attacks
            speedFactor = 0;
        }
        
        //if it's too far from attacking
        else
        {
            isInPlayerRange = false;
            speedFactor = 1;
            //follows player
            agent.SetDestination(player.transform.position);
        }

        if (health <= 0)
        {
            //dies
            
        }
    }
    
    
}
