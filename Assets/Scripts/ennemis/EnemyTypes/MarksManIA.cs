using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class MarksManIA : MonoBehaviour
{
    private Vector3 _playerDir;
    private Vector3 _attackDir;
    private Vector3 _fleeDir;
    private Vector3 _laserPos;
    private LayerMask _wallLayerMask;
    private LayerMask _playerLayerMask;

    //values
    private float _stunCounter;
    private float _playerDist;
    private float _speedFactor;
    private bool _canShootProjectile = true;
    private bool _canDash = true;
    private bool _isStunned;
    private bool _isDashing;
    private bool _isHit;
    private bool _isTouchingWall;
    private bool _isVulnerable;
    private float _desiredRange = 20f;
    private bool isRunningAway;
    private float _laserTimer;
    private float rotationSpeed = 5f;
    private bool _isCharging;
    private bool _canShootLaser = true;
    private bool _canLaserTouchPlayer;
    private float _laserLength = 1;
    
    public Gradient laserGradient;
    public Material laserMaterial;
    private WaitForSeconds _waitInstance;
    private float _playerPosDelay = 0.0f;

    //components
    private NavMeshAgent _agent;
    private GameObject _player;
    private GameManager _gameManager;
    private Rigidbody _rb;
    private EnemyType enemyTypeData;
    private Enemy _enemyTrigger;
    private LineRenderer _lineRenderer;

    private void Awake()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _agent = GetComponent<NavMeshAgent>();
        _player = GameObject.Find("Player");
        _rb = GetComponent<Rigidbody>();
        _enemyTrigger = GetComponent<Enemy>();
        _lineRenderer = GetComponent<LineRenderer>();
        laserMaterial = _lineRenderer.material;
        enemyTypeData = _enemyTrigger.enemyTypeData;

        _wallLayerMask = LayerMask.GetMask("Wall");
        _playerLayerMask = LayerMask.GetMask("Player");
        GetComponent<EnemyDamage>().damage = enemyTypeData.damage;
    }

    private void Start()
    {
        _waitInstance = new WaitForSeconds(_playerPosDelay);
        _lineRenderer.enabled = false;
    }

    private void Update()
    {
        _agent.speed = enemyTypeData.speed * _speedFactor;
        _playerDist = (_player.transform.position - transform.position).magnitude;
        _playerDir = _player.transform.position - transform.position;

        if (_isHit)
        {
            //resets stun counter
            _stunCounter = enemyTypeData.stunLenght;
        }

        StunProcess();
    }

    private void FixedUpdate()
    {
        _stunCounter -= Time.deltaTime;
        
        if (!_isStunned && _enemyTrigger.isActive)
        {
            //if is not stunned by player
            //main behaviour
            MarksMan();
        }
        
        MaxSpeed();
        FleeDir();
        LaserCollision();

        if (_isCharging)
        {
            StartCoroutine(DelayPlayerPos());
        }
    }

    void MarksMan()
    {
        //calculates the distance between object and player
        _playerDist = (_player.transform.position - transform.position).magnitude;

        //move
        if (_canShootLaser)
        {
            //if the enemy is too close, walks away
            if (_playerDist < _desiredRange)
            {
                //recule
                _speedFactor = 0;
                _rb.AddForce(_fleeDir.normalized * 10, ForceMode.Acceleration);
            }

            //if the enemy is in range, and not too far
            if (_playerDist > _desiredRange && _playerDist < enemyTypeData.attackRange)
            {
                //stays immobile
                _speedFactor = 0;
                _agent.SetDestination(transform.position);
            }

            //if the enemy is too far, gets closer
            if (_playerDist > enemyTypeData.attackRange)
            {
                //avance
                _speedFactor = 1;
                _agent.SetDestination(_player.transform.position);
            }
        }
        else
        {
            _rb.velocity = Vector3.zero;
        }


        if (_canShootLaser && _playerDist < enemyTypeData.attackRange)
        {
            _canShootLaser = false;
            StartCoroutine(LaserAttack());
        }

        if (_isCharging)
        {
            LaserVisuals();
        }
    }

    void LaserVisuals()
    {
        //updates laser color
        if (_laserTimer < enemyTypeData.shootWarmup)
        {
            _laserTimer += Time.deltaTime;
        }
        else
        {
            _laserTimer = enemyTypeData.shootWarmup;
        }
        laserMaterial.color = laserGradient.Evaluate(_laserTimer / enemyTypeData.shootWarmup);

        //updates laser position
        _attackDir = (_laserPos - transform.position).normalized;
        Vector3 hitPoint;
        RaycastHit hit;
            
        //check if a wall is in between laser
        if (Physics.Raycast(transform.position, _attackDir, out hit, 1000, _wallLayerMask))
        {
            hitPoint = hit.point;
        }
        else
        {
            hitPoint = transform.position + _attackDir.normalized * 1000;
        }
            
        _lineRenderer.SetPosition(0, transform.position);
        _lineRenderer.SetPosition(1, hitPoint);
    }

    IEnumerator LaserAttack()
    {
        //while charging, laser is in direction of player, and color is updated depending on the current charge
        _lineRenderer.enabled = true;
        _isCharging = true;

        yield return new WaitForSeconds(enemyTypeData.shootWarmup);
        
        _isCharging = false;
        _laserTimer = 0;
        laserMaterial.color = Color.cyan;
        //waits a bit for the player to avoid the laser
        yield return new WaitForSeconds(0.5f);
        
        //shoots laser
        _canLaserTouchPlayer = true;
        
        //laser set inactive
        yield return new WaitForSeconds(_laserLength);
        _canLaserTouchPlayer = false;
        _lineRenderer.enabled = false;
        
        //can shoot again
        yield return new WaitForSeconds(enemyTypeData.attackCooldown);
        _lineRenderer.enabled = false;
        _canShootLaser = true;
    }

    IEnumerator DelayPlayerPos()
    {
        //delays player pos
        Vector3 playerPos = _player.transform.position;

        yield return _waitInstance;

        _laserPos = playerPos;
    }
    
    void LaserCollision()
    {
        if (_canLaserTouchPlayer)
        {
            //shoots laser
            laserMaterial.color = Color.magenta;

            //check if player touches laser
            if (Physics.Raycast(transform.position, _attackDir, 4, _playerLayerMask))
            {
                Debug.Log("hit player");
                //deals damage
                _gameManager.health -= enemyTypeData.projectileDamage;
                //can touch laser twice
            }
        }
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

    void FleeDir()
    {
        if (Physics.Raycast(transform.position, -_playerDir, 4, _wallLayerMask))
        {
            Debug.Log("hit wall");
            //else, turns a bit
            _fleeDir = Quaternion.Euler(0, 45, 0) * -_playerDir;
        }
        else
        {
            //if there isn't a wall in flee direction, simply run away from player
            _fleeDir = -_playerDir;
        }
    }
}
