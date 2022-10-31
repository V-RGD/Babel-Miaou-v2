using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class TurretIA : MonoBehaviour
{
    //public int enemyType;
    
    private Vector3 playerDir;
    private Vector3 attackDir;
    private LayerMask wallLayerMask;
    
    //values
    private float _health;
    private float _stunCounter;
    private float _playerDist;
    private float _speedFactor;
    private Vector3 _projectileDir;
    private bool _canShootProjectile = true;
    private bool _canDash = true;
    private bool _isStunned;
    private bool _isDashing;
    private bool _isHit;
    private bool _isTouchingWall;
    private bool _isVulnerable;
    private float _desiredRange = 13f;
    private float attackCooldownWhenRunningAway = 4;
    private bool isRunningAway;
    private float attackCooldown;
    private Vector3 _fleeDir;

    //components
    private NavMeshAgent _agent;
    private GameObject _player;
    private GameManager _gameManager;
    private Rigidbody _rb;
    public EnemyType enemyTypeData;
    private Enemy _enemyTrigger;

    [Header("*Objects*")]
    public GameObject healthSlider;
    
    private void Awake()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _agent = GetComponent<NavMeshAgent>();
        _player = GameObject.Find("Player");
        _rb = GetComponent<Rigidbody>();
        _enemyTrigger = GetComponent<Enemy>();

        _health = enemyTypeData.maxHealth;
        wallLayerMask = LayerMask.GetMask("Wall");
        GetComponent<EnemyDamage>().damage = enemyTypeData.damage;
    }

    private void Update()
    {
        _agent.speed = enemyTypeData.speed * _speedFactor;
        _playerDist = (_player.transform.position - transform.position).magnitude;
        playerDir = _player.transform.position - transform.position;

        if (_health <= 0)
        {
            //dies
            Death();
        }

        if (_isHit)
        {
            //resets stun counter
            _stunCounter = enemyTypeData.stunLenght;
        }

        StunProcess();
        SliderUpdate();
    }

    private void FixedUpdate()
    {
        _stunCounter -= Time.deltaTime;
        
        if (!_isStunned && _enemyTrigger.isActive)
        {
            //if is not stunned by player
            //main behaviour
            Shooter();
        }
        
        MaxSpeed();
        FleeDir();
    }

    void Shooter()
    {
        //calculates the distance between object and player
        _projectileDir = _player.transform.position - transform.position;

        //if the enemy is too close, walks away
        if (_playerDist < _desiredRange)
        {
            //increases cooldown
            attackCooldown = attackCooldownWhenRunningAway;
            //recule
            _speedFactor = 0;
            _rb.AddForce(_fleeDir.normalized * 10, ForceMode.Acceleration);
        }
        //if the enemy is in range, and not too far
        if (_playerDist > _desiredRange && _playerDist < enemyTypeData.attackRange)
        {
            //recule
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
            StartCoroutine(ShootProjectile());
        }
    }

    IEnumerator ShootProjectile()
    {
        yield return new WaitForSeconds(enemyTypeData.shootWarmup);
        //shoots a projectile
        GameObject projectile = Instantiate(enemyTypeData.mageProjectile, transform.position, quaternion.identity);
        //gives it proper force
        projectile.GetComponent<Rigidbody>().AddForce(_projectileDir.normalized * enemyTypeData.projectileForce);
        projectile.GetComponent<ProjectileDamage>().damage = enemyTypeData.projectileDamage;
        //waits for cooldown to refresh to shoot again
        yield return new WaitForSeconds(attackCooldown);
        //can shoot again
        _canShootProjectile = true;
    }

    void StunProcess()
    {
        //when stunned
        if (_stunCounter > 0)
        {
            _isStunned = true;
            //is vulnerable
            _isVulnerable = true;
            _speedFactor = 0;
        }
        else
        {
            _isStunned = false;
            //can't be touched
            _isVulnerable = false;
        }
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

    void Death()
    {
        for (int i = 0; i < enemyTypeData.eyesDropped; i++)
        {
            Instantiate(enemyTypeData.eyeToken, transform.position, quaternion.identity);
        }
        Destroy(gameObject);
    }

    void SliderUpdate()
    {
        if (_health >= enemyTypeData.maxHealth)
        {
            healthSlider.SetActive(false);
        }
        else
        {
            healthSlider.SetActive(true);
            healthSlider.GetComponent<Slider>().value = _health / enemyTypeData.maxHealth;
        }
    }
    
    void FleeDir()
    {
        if (Physics.Raycast(transform.position, -playerDir, 4, wallLayerMask))
        {
            Debug.Log("hit wall");
            //else, turns a bit
            _fleeDir = Quaternion.Euler(0, 45, 0) * -playerDir;
        }
        else
        {
            //if there isn't a wall in flee direction, simply run away from player
            _fleeDir = -playerDir;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerAttack"))
        {
            _health -= other.GetComponent<ObjectDamage>().damage;
        }
    }
}
