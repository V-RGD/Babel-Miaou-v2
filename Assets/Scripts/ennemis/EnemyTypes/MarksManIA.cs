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
    private Vector3 _laserPos;
    private LayerMask _playerLayerMask;

    //values
    private bool _isDashing;
    private bool _isHit;
    private float _laserTimer;
    private float rotationSpeed = 5f;
    private bool _isCharging;
    private bool _canShootLaser = true;
    private bool _canLaserTouchPlayer;
    private float _laserLength = 0.5f;
    
    public Gradient laserGradient;
    public Material laserMaterial;
    private WaitForSeconds _waitInstance;
    private float _playerPosDelay = 0.0f;

    //components
    private GameObject _player;
    private GameManager _gameManager;
    private EnemyType enemyTypeData;
    private Enemy _enemyTrigger;
    [SerializeField]private LineRenderer _lineRenderer;
    public ParticleSystem laserFx;
    
    [SerializeField]private Animator _animator;
    public int currentAnimatorState;
    private static readonly int Idle = Animator.StringToHash("Idle");
    private static readonly int Attack = Animator.StringToHash("Attack");
    private static readonly int Stun = Animator.StringToHash("Stun");

    private void Awake()
    {
        laserFx.Stop();
        _gameManager = GameManager.instance;
        _player = GameObject.Find("Player");
        _enemyTrigger = GetComponent<Enemy>();
        laserMaterial = _lineRenderer.material;
        enemyTypeData = _enemyTrigger.enemyTypeData;

        _playerLayerMask = LayerMask.GetMask("Player");
        GetComponent<EnemyDamage>().damage = _enemyTrigger.damage;
    }

    private void Start()
    {
        _waitInstance = new WaitForSeconds(_playerPosDelay);
        _lineRenderer.enabled = false;
    }

    private void Update()
    {
    }

    private void FixedUpdate()
    {
        
        if (_enemyTrigger.isActive)
        {
            //if is not stunned by player
            //main behaviour
            MarksMan();
        }
        LaserCollision();

        if (_isCharging)
        {
            StartCoroutine(DelayPlayerPos());
        }
    }

    void MarksMan()
    {
        //calculates the distance between object and player

        if (_canShootLaser)
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
        if (Physics.Raycast(transform.position, _attackDir, out hit, 1000, _playerLayerMask))
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
        _animator.CrossFade(Attack, 0, 0);
        currentAnimatorState = Attack;
        //while charging, laser is in direction of player, and color is updated depending on the current charge
        _lineRenderer.enabled = true;
        _isCharging = true;

        yield return new WaitForSeconds(enemyTypeData.shootWarmup);
        
        _isCharging = false;
        _laserTimer = 0;
        laserMaterial.color = Color.cyan;
        //waits a bit for the player to avoid the laser
        _lineRenderer.enabled = false;
        laserFx.transform.parent.LookAt(_player.transform);
        laserFx.Play();
        yield return new WaitForSeconds(0.5f);
        
        //shoots laser
        _canLaserTouchPlayer = true;
        
        //laser set inactive
        yield return new WaitForSeconds(_laserLength);
        laserFx.Stop();
        _canLaserTouchPlayer = false;
        
        //can shoot again
        yield return new WaitForSeconds(enemyTypeData.attackCooldown);
        _animator.CrossFade(Idle, 0, 0);
        currentAnimatorState = Idle;
        _lineRenderer.enabled = false;
        _canShootLaser = true;
    }

    IEnumerator DelayPlayerPos()
    {
        //delays player pos
        Vector3 playerPos = _player.transform.position;

        yield return _waitInstance;
        _playerDir = playerPos - transform.position;
        _laserPos = playerPos;
    }
    
    void LaserCollision()
    {
        if (_canLaserTouchPlayer)
        {
            //shoots laser
            laserMaterial.color = Color.magenta;
            RaycastHit hit;
            //check if player touches laser
            if (Physics.Raycast(transform.position, _attackDir, 1000, _playerLayerMask))
            {
                Debug.Log("hit player");
                //deals damage
                _gameManager.DealDamageToPlayer(_enemyTrigger.damage);
                //can touch laser twice
            }
        }
    }
}
