using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class ShooterIA : MonoBehaviour
{
    //public int enemyType;
    
    private Vector3 playerDir;
    private Vector3 attackDir;
    private LayerMask wallLayerMask;
    
    //values
    private float _playerDist;
    private float _speedFactor;
    private Vector3 _projectileDir;
    private bool _canShootProjectile = true;
    private bool _canDash = true;
    private bool _isDashing;
    private bool _isHit;
    private bool _isTouchingWall;
    private bool _isVulnerable;
    private float _desiredRange = 13f;
    private float attackCooldownWhenRunningAway = 4;
    private bool isRunningAway;
    private float attackCooldown;
    private Vector3 _fleeDir;
    public bool isBigShooter;

    //components
    private NavMeshAgent _agent;
    private GameObject _player;
    public GameObject bigShooterProjo;
    private Rigidbody _rb;
    private EnemyType enemyTypeData;
    private Enemy _enemyTrigger;

    private Animator _animator;
    public int currentAnimatorState;
    private static readonly int Idle = Animator.StringToHash("Idle");
    private static readonly int Attack = Animator.StringToHash("Attack");
    private static readonly int Stun = Animator.StringToHash("Stun");

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _player = GameObject.Find("Player");
        _rb = GetComponent<Rigidbody>();
        _enemyTrigger = GetComponent<Enemy>();
        enemyTypeData = _enemyTrigger.enemyTypeData;
        _animator = GetComponent<Animator>();

        wallLayerMask = LayerMask.GetMask("Wall");
        GetComponent<EnemyDamage>().damage = _enemyTrigger.damage;
    }

    private void Update()
    {
        _agent.speed = _enemyTrigger.speed * _speedFactor * enemyTypeData.enemySpeed;
        _playerDist = (_player.transform.position - transform.position).magnitude;
        playerDir = _player.transform.position - transform.position;
        FleeDir();
    }

    private void FixedUpdate()
    {
        if (_enemyTrigger.stunCounter <= 0 && _enemyTrigger.isActive)
        {
            //if is not stunned by player
            //main behaviour
            Shooter();
        }
        else
        {
            _agent.speed = 0;
            _speedFactor = 0;
            _rb.velocity = Vector3.zero;
        }
        
        MaxSpeed();
    }

    void Shooter()
    {
        //calculates the distance between object and player
        var position = transform.position;
        _projectileDir = _player.transform.position - position;

        //if the enemy is too close, walks away
        if (_playerDist < _desiredRange)
        {
            //increases cooldown if is normal version
            if (!isBigShooter)
            {
                attackCooldown = attackCooldownWhenRunningAway;
            }
            //recule
            _speedFactor = 0;
            _rb.AddForce(_fleeDir.normalized * (_enemyTrigger.speed * enemyTypeData.enemySpeed), ForceMode.VelocityChange);
        }
        //if the enemy is in range, and not too far
        if (_playerDist > _desiredRange && _playerDist < enemyTypeData.attackRange)
        {
            //attaque et agit normalement
            _speedFactor = 0;
            attackCooldown = enemyTypeData.attackCooldown;
            _agent.SetDestination(transform.position);
        }
        //if the enemy is too far, gets closer
        if (_playerDist > enemyTypeData.attackRange)
        {
            //avance
            _speedFactor = 1;
            _agent.SetDestination(_player.transform.position);
        }
        if (_canShootProjectile && _playerDist < enemyTypeData.attackRange)
        {
            _canShootProjectile = false;
            if (isBigShooter)
            {
                StartCoroutine(ShootBigShooterProjectile());
            }
            else
            {
                StartCoroutine(ShootProjectile());
            }
        }
    }

    IEnumerator ShootProjectile()
    {
        _animator.CrossFade(Attack, 0, 0);
        currentAnimatorState = Attack;
        yield return new WaitForSeconds(enemyTypeData.shootWarmup);
        //shoots a projectile
        GameObject projectile = Instantiate(enemyTypeData.mageProjectile, transform.position, quaternion.identity);
        //gives it proper force
        projectile.GetComponent<Rigidbody>().AddForce(_projectileDir.normalized * enemyTypeData.projectileForce);
        projectile.GetComponent<ProjectileDamage>().damage = enemyTypeData.projectileDamage;
        //waits for cooldown to refresh to shoot again
        yield return new WaitForSeconds(attackCooldown);
        _animator.CrossFade(Attack, 0, 0);
        currentAnimatorState = Attack;
        //can shoot again
        _canShootProjectile = true;
    }
    
    IEnumerator ShootBigShooterProjectile()
    {
        float maxAngle = 100; //degrees
        float projectilesNumber = 7;
        float baseAngle = -maxAngle/2;
        float angleOffset = maxAngle / (projectilesNumber - 1);
        float projectionAngle = baseAngle;
        
        yield return new WaitForSeconds(enemyTypeData.shootWarmup);
        //shoots multiples projectiles

        for (int i = 0; i < projectilesNumber - 1; i++)
        {
            //activates projecile
            GameObject projo = Instantiate(enemyTypeData.mageProjectile);
            projo.SetActive(true);
            //places it correctly
            projo.transform.position = transform.position + playerDir.normalized;
            //gives it force, in desired angle
            projo.GetComponent<Rigidbody>().AddForce(Quaternion.Euler(0, projectionAngle, 0) * _projectileDir.normalized  * enemyTypeData.projectileForce);
            //sets damage
            projo.GetComponent<ProjectileDamage>().damage = enemyTypeData.projectileDamage;
            //adds angle offset
            projectionAngle += angleOffset;
        }
        
        //then adds big projectile
        
        GameObject bigProjectile = Instantiate(bigShooterProjo, transform.position, quaternion.identity);
        bigProjectile.GetComponent<BigShooterProjectile>().enemyTypeData = enemyTypeData;
        //activates projecile
        bigProjectile.SetActive(true);
        //places it correctly
        bigProjectile.transform.position = transform.position + playerDir.normalized;
        //gives it force, in desired angle
        bigProjectile.GetComponent<Rigidbody>().AddForce(_projectileDir.normalized  * enemyTypeData.projectileForce);
        //sets damage
        bigProjectile.GetComponent<ProjectileDamage>().damage = enemyTypeData.projectileDamage;
        
        //waits for cooldown to refresh to shoot again
        yield return new WaitForSeconds(attackCooldown);
        //can shoot again
        _canShootProjectile = true;
    }

    void MaxSpeed()
    {
        //cap x speed
        if(_rb.velocity.x > enemyTypeData.maxSpeed)
        {
            _rb.velocity = new Vector3(enemyTypeData.maxSpeed, _rb.velocity.y, _rb.velocity.z);
        }
        if(_rb.velocity.x < -enemyTypeData.maxSpeed)
        {
            _rb.velocity = new Vector3(-enemyTypeData.maxSpeed, _rb.velocity.y, _rb.velocity.z);
        }
            
        //cap x speed
        if(_rb.velocity.z > enemyTypeData.maxSpeed)
        {
            _rb.velocity = new Vector3(_rb.velocity.x, _rb.velocity.y, enemyTypeData.maxSpeed);
        }
        if(_rb.velocity.z < -enemyTypeData.maxSpeed)
        {
            _rb.velocity = new Vector3(_rb.velocity.x, _rb.velocity.y, -enemyTypeData.maxSpeed);
        }
    }

    void FleeDir()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -playerDir.normalized, out hit, 20, wallLayerMask))
        {
            //else, turns a bit
            Vector3 wallDir = transform.position - hit.point;
            _fleeDir = wallDir;
        }
        else
        {
            //if there isn't a wall in flee direction, simply run away from player
            _fleeDir = -playerDir;
        }
    }
}
