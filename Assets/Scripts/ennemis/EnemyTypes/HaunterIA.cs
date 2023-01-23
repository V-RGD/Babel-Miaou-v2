using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.VFX;

public class HaunterIA : MonoBehaviour
{
    //public int enemyType;
    private Vector3 _playerDir;
    private Vector3 _attackDir;
    
    //values
    private float _stunCounter;
    private float _playerDist;
    private float _speedFactor;
    private Vector3 _projectileDir;
    private bool _canShootProjectile = true;
    private bool _canDash = true;
    private bool _isStunned;
    private bool _isDashing;
    private bool _isAttacking;
    private bool _isHit;
    private bool _isTouchingWall;
    private bool _isVulnerable;
    public bool isTank;

    //components
    private Enemy _enemyTrigger;
    private NavMeshAgent _agent;
    private GameObject _player;
    private GameObject _attackAnchor;
    private Rigidbody _rb;
    private ObjectDamage _jabDamage;
    private EnemyType _enemyTypeData;
    [SerializeField]private Animator animator;
    private SpriteRenderer _spriteRenderer;
    public int currentAnimatorState;
    private static readonly int Idle = Animator.StringToHash("Idle");
    private static readonly int Attack = Animator.StringToHash("Attack");
    [SerializeField] private VisualEffect clawFx;
    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _player = GameObject.Find("Player");
        _rb = GetComponent<Rigidbody>();
        _attackAnchor = transform.GetChild(1).gameObject;
        _enemyTrigger = GetComponent<Enemy>();
        _enemyTypeData = _enemyTrigger.enemyTypeData;
        GetComponent<EnemyDamage>().damage = _enemyTrigger.damage;
        _jabDamage = _attackAnchor.transform.GetChild(0).gameObject.GetComponent<ObjectDamage>();
    }

    private void Start()
    {
        clawFx.Stop();
    }

    private void Update()
    {
        Vector3 playerPos = _player.transform.position;
        Vector3 position = transform.position;
        _agent.speed = _enemyTrigger.speed * _speedFactor  * _enemyTypeData.enemySpeed;
        _playerDist = (playerPos - position).magnitude;
        _playerDir = (playerPos - position).normalized;

        if (_isHit)
        {
            //resets stun counter
            _stunCounter = _enemyTypeData.stunLenght;
        }
        GetAnimation();
    }
    private void FixedUpdate()
    {
        _stunCounter -= Time.deltaTime;
        Friction();
        StunProcess();
        
        if (!_isStunned && _enemyTrigger.isActive)
        {
            //if is not stunned by player
            //main behaviour
            Haunter();
        }
    }
    void GetAnimation()
    {
        if (_isAttacking)
        {
            return;
        }
        else
        {
            if (Idle == currentAnimatorState) return;
            animator.CrossFade(Idle, 0, 0);
            currentAnimatorState = Idle;
        }
        
    }
    void Friction()
    {
        if (_isAttacking)
        {
            _agent.speed = 0;
            Vector3 velocity = _rb.velocity;
            _rb.velocity = new Vector3(velocity.x * 0.95f, velocity.y, velocity.z * 0.95f);
        }
    }
    void Haunter()
    {
        //if the enemy is in player range
        if (_playerDist <= _enemyTypeData.attackRange)
        {
            //stops then attacks
            _speedFactor = 0;
            if (!_isAttacking)
            {
                //stops moving
                _isAttacking = true;
                StartCoroutine(AttackCooldown());
            }
        }
        
        //if it's too far from attacking
        else
        {
            _speedFactor = 1;
            //follows player
            _agent.SetDestination(_player.transform.position);
        }
    }
    IEnumerator AttackCooldown()
    {
            animator.CrossFade(Attack, 0, 0);
            currentAnimatorState = Attack;
            float force = _enemyTypeData.attackForce;
            //determines attack length, damage, hitbox, force to add
            //stops movement
            //add current damage stat to weapon
            _jabDamage.damage = _enemyTrigger.damage;
            //warmup
            yield return new WaitForSeconds(_enemyTypeData.shootWarmup - 0.2f);
            Vector3 attackLocation = new Vector3(_player.transform.position.x, _attackAnchor.transform.position.y, _player.transform.position.z);
            yield return new WaitForSeconds(0.2f);
            //determine où l'attaque va se faire
            _attackAnchor.transform.LookAt(attackLocation);
            //actives weapon
            _enemyTrigger.canTouchPlayer = true;
            _attackAnchor.transform.GetChild(0).gameObject.SetActive(true);
            clawFx.Play();
            _rb.velocity = Vector3.zero;
            Vector3 pushedDir = (attackLocation - transform.position).normalized;
            _rb.AddForce(pushedDir * force, ForceMode.Impulse);
            //plays animation
            //attack duration : time when the player can actually be hit
            yield return new WaitForSeconds(0.1f);
            yield return new WaitForSeconds(0.3f);
            _attackAnchor.transform.GetChild(0).gameObject.SetActive(false);
            _enemyTrigger.canTouchPlayer = false;
            //waits cooldown depending on the attack used
            yield return new WaitForSeconds(_enemyTypeData.attackCooldown - 0.4f);
            _rb.velocity = Vector3.zero;
            animator.CrossFade(Idle, 0, 0);
            currentAnimatorState = Idle;
            //disables hitbox
            _isAttacking = false;
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
}
