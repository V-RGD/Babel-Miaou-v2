using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Random = UnityEngine.Random;

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
    private AudioSource _audioSource;

    private Animator _animator;
    public int currentAnimatorState;
    private static readonly int Walk = Animator.StringToHash("Walk");
    private static readonly int Attack = Animator.StringToHash("Attack");
    private static readonly int Stun = Animator.StringToHash("Stun");

    public ParticleSystem stunFx;

    public ShooterStates shooterState;
    public enum ShooterStates
    {
        Stun,
        Flee,
        Follow,
        Attack
    }
    
    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _player = GameObject.Find("Player");
        _rb = GetComponent<Rigidbody>();
        _enemyTrigger = GetComponent<Enemy>();
        enemyTypeData = _enemyTrigger.enemyTypeData;
        _animator = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();

        wallLayerMask = LayerMask.GetMask("Wall");
        GetComponent<EnemyDamage>().damage = _enemyTrigger.damage;
    }

    void StunSystem()
    {
        if (_enemyTrigger.stunCounter <= 0 && _enemyTrigger.isActive)
        {
            //if is not stunned by player
            //main behaviour
            _enemyTrigger.canFlip = true;
        }
        else
        {
            if (currentAnimatorState != Stun)
            {
                _animator.CrossFade(Stun, 0, 0);
                currentAnimatorState = Stun;
            }
            _agent.speed = 0;
            _speedFactor = 0;
            _rb.velocity = Vector3.zero;
        }
    }

    private void Start()
    {
        SwitchState(ShooterStates.Flee);
    }

    private void Update()
    {
        _agent.speed = _enemyTrigger.speed * _speedFactor * enemyTypeData.enemySpeed;
        var playerPos = _player.transform.position;
        var position = transform.position;
        _playerDist = (playerPos - position).magnitude;
        playerDir = playerPos - position;
        _projectileDir = playerPos - position;
        CheckPlayerState();
    }
    private void FixedUpdate()
    {
        if (_enemyTrigger.isActive)
        {
            Behaviour();
        }
        MaxSpeed();
    }
    void Behaviour()
    {
        switch (shooterState)
        {
            case ShooterStates.Attack : break; //stays still
            case ShooterStates.Follow :
                FollowPlayer();
                AttackManagement();
                break; //gets closer to player
            case ShooterStates.Flee : 
                FleePlayer();
                break; //gets away from it
            case ShooterStates.Stun : 
                StunSystem();
                break; //stays still
        }
    }

    void CheckPlayerState()
    {
        //firs checks if it's stunned
        if (_enemyTrigger.stunCounter > 0)    
        {
            SwitchState(ShooterStates.Stun);
            return;
        }
        
        //then if it's attacking
        if (shooterState == ShooterStates.Attack)
        {
            return;
        }
        
        //and then if it can move and in which direction
        //if the enemy is too far, gets closer
        if (_playerDist > enemyTypeData.attackRange)
        {
            SwitchState(ShooterStates.Follow);
        }
        //if the enemy is too close, walks away
        if (_playerDist < _desiredRange)
        {
            SwitchState(ShooterStates.Flee);
        }
    }
    void FollowPlayer()
    {
        //avance
        _speedFactor = 1;
        _agent.SetDestination(_player.transform.position);

        if (currentAnimatorState != Walk)
        {
            _animator.CrossFade(Walk, 0, 0);
            currentAnimatorState = Walk;
        }
    }
    void FleePlayer()
    {
        //increases cooldown if is normal version
        if (!isBigShooter)
        {
            attackCooldown = attackCooldownWhenRunningAway;
        }
        //recule
        _speedFactor = 0;
        _rb.AddForce(_fleeDir.normalized * (_enemyTrigger.speed * enemyTypeData.enemySpeed), ForceMode.VelocityChange);
        
        //flee dir
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
        
        if (currentAnimatorState != Walk)
        {
            _animator.CrossFade(Walk, 0, 0);
            currentAnimatorState = Walk;
        }
    }
    
    IEnumerator ShootProjectile()
    {
        SwitchState(ShooterStates.Attack);
        _animator.CrossFade(Attack, 0, 0);
        currentAnimatorState = Attack;
        _audioSource.PlayOneShot(GameSounds.instance.shooterRecharge[Random.Range(0, GameSounds.instance.shooterRecharge.Length)]);
        yield return new WaitForSeconds(enemyTypeData.shootWarmup);
        _audioSource.PlayOneShot(GameSounds.instance.shooterProjectile[Random.Range(0, GameSounds.instance.shooterProjectile.Length)]);
        //shoots a projectile depending on the current level
        switch (LevelManager.instance.currentLevel)
        {
            case 0 : 
                GameObject projectile = Instantiate(enemyTypeData.mageProjectile, transform.position, quaternion.identity);
                projectile.GetComponent<Rigidbody>().AddForce(_projectileDir.normalized * enemyTypeData.projectileForce);
                projectile.GetComponent<ProjectileDamage>().damage = enemyTypeData.projectileDamage;
                projectile.transform.GetChild(0).transform.LookAt(transform.position + _projectileDir.normalized * 1000);
                break;
            case 1 :
                GameObject projectileLeft = Instantiate(enemyTypeData.mageProjectile, transform.position, quaternion.identity);
                projectileLeft.GetComponent<Rigidbody>().AddForce(Quaternion.Euler(0, 0, 15) * _projectileDir.normalized * enemyTypeData.projectileForce);
                projectileLeft.GetComponent<ProjectileDamage>().damage = enemyTypeData.projectileDamage;
                projectileLeft.transform.GetChild(0).transform.LookAt(transform.position + _projectileDir.normalized * 1000);
                
                GameObject projectileRight = Instantiate(enemyTypeData.mageProjectile, transform.position, quaternion.identity);
                projectileRight.GetComponent<Rigidbody>().AddForce(Quaternion.Euler(0, 0, -15) * _projectileDir.normalized * enemyTypeData.projectileForce);
                projectileRight.GetComponent<ProjectileDamage>().damage = enemyTypeData.projectileDamage;
                projectileRight.transform.GetChild(0).transform.LookAt(transform.position + _projectileDir.normalized * 1000);
                break;
            case 2 : 
                GameObject projectile1 = Instantiate(enemyTypeData.mageProjectile, transform.position, quaternion.identity);
                projectile1.GetComponent<Rigidbody>().AddForce(Quaternion.Euler(0, 0, 15) * _projectileDir.normalized * enemyTypeData.projectileForce);
                projectile1.GetComponent<ProjectileDamage>().damage = enemyTypeData.projectileDamage;
                
                GameObject projectile2 = Instantiate(enemyTypeData.mageProjectile, transform.position, quaternion.identity);
                projectile2.transform.GetChild(0).transform.LookAt(transform.position + _projectileDir.normalized * 1000);
                projectile2.GetComponent<Rigidbody>().AddForce(Quaternion.Euler(0, 0, -15) * _projectileDir.normalized * enemyTypeData.projectileForce);
                projectile2.GetComponent<ProjectileDamage>().damage = enemyTypeData.projectileDamage;
                
                GameObject projectile3 = Instantiate(enemyTypeData.mageProjectile, transform.position, quaternion.identity);
                projectile3.transform.GetChild(0).transform.LookAt(transform.position + _projectileDir.normalized * 1000);
                projectile3.GetComponent<Rigidbody>().AddForce(_projectileDir.normalized * enemyTypeData.projectileForce);
                projectile3.GetComponent<ProjectileDamage>().damage = enemyTypeData.projectileDamage;
                projectile3.transform.GetChild(0).transform.LookAt(transform.position + _projectileDir.normalized * 1000);
                break;
        }
        
        //waits for cooldown to refresh to shoot again
        SwitchState(ShooterStates.Flee);
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
    void SwitchState(ShooterStates nextState)
    {
        if (nextState != shooterState)
        {
            shooterState = nextState;
            _rb.velocity = Vector3.zero;
        }
    }
    void AttackManagement()
    {
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
    
    private void OnTriggerEnter(Collider other)
    {
        //if player hit
        if (other.CompareTag("PlayerAttack"))
        {
            stunFx.Play();
        }
    }
}
