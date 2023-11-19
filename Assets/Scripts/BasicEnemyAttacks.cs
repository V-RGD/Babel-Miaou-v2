using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class BasicEnemyAttacks : EnemyAttacks
{
    [SerializeField] float timeUntilCanAttack = 2;
    float _timeUntilCanAttackTimer;
    public Attack defaultAttack;
    bool _detectsPlayer;

    public override void Start()
    {
        base.Start();

        defaultAttack.onAttack.AddListener(UseDefaultAttack);

        _timeUntilCanAttackTimer = timeUntilCanAttack;
    }

    void UseDefaultAttack() => StartCoroutine(AttackRoutine());

    public override void Update()
    {
        base.Update();
        if (_timeUntilCanAttackTimer > 0)
        {
            _timeUntilCanAttackTimer -= Time.deltaTime;
            return;
        }

        Behaviour();
    }

    void Behaviour()
    {
        //if enemy is not already attacking
        if (isAttacking) return;

        if (!_detectsPlayer) return;
        StartAttack();
    }

    void StartAttack()
    {
        isAttacking = true;
        //chooses either the default attack or the variant
        onAttack?.Invoke();
        StartCoroutine(AttackRoutine());
    }

    protected virtual IEnumerator AttackRoutine()
    {
        yield return null;
    }
}