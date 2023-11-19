using System;
using UnityEngine;

public class EnemyAttacks : AttackSystem //inherited behaviour state machine
{
    Vector2 _currentPos;
    [HideInInspector] protected Vector2 playerDir;
    [HideInInspector] public Transform player;

    public virtual void Start()
    {
        player = PlayerController.Instance.transform;
    }

    public virtual void Update()
    {
        _currentPos = transform.position;
        //updates player direction
        playerDir = ((Vector2)player.position - _currentPos).normalized;
    }
}