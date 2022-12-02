using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class HaunterIA : MonoBehaviour
{
    //public int enemyType;
    
    private Vector3 playerDir;
    private Vector3 attackDir;
    
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

    private Enemy _enemyTrigger;

    //components
    private NavMeshAgent _agent;
    private GameObject _player;
    private GameObject _attackAnchor;
    private Rigidbody _rb;
    private EnemyType enemyTypeData;
    [SerializeField]private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    public int currentAnimatorState;
    private static readonly int Idle = Animator.StringToHash("Idle");
    private static readonly int Attack = Animator.StringToHash("Attack");
    private static readonly int Death = Animator.StringToHash("Death");
    
    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _player = GameObject.Find("Player");
        _rb = GetComponent<Rigidbody>();
        _attackAnchor = transform.GetChild(1).gameObject;
        _enemyTrigger = GetComponent<Enemy>();
        enemyTypeData = _enemyTrigger.enemyTypeData;
        GetComponent<EnemyDamage>().damage = _enemyTrigger.damage;
    }

    private void Update()
    {
        _agent.speed = _enemyTrigger.speed * _speedFactor  * enemyTypeData.enemySpeed;
        _playerDist = (_player.transform.position - transform.position).magnitude;
        playerDir = (_player.transform.position - transform.position).normalized;

        if (_isHit)
        {
            //resets stun counter
            _stunCounter = enemyTypeData.stunLenght;
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
            _animator.CrossFade(Idle, 0, 0);
            currentAnimatorState = Idle;
        }
        
    }

    void Friction()
    {
        if (_isAttacking)
        {
            _agent.speed = 0;
            _rb.velocity = new Vector3(_rb.velocity.x * 0.95f, _rb.velocity.y, _rb.velocity.z * 0.95f);
        }
    }

    void Haunter()
    {
        //if the enemy is in player range
        if (_playerDist <= enemyTypeData.attackRange)
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
            _animator.CrossFade(Attack, 0, 0);
            currentAnimatorState = Attack;
            float force = enemyTypeData.attackForce;
            //determines attack length, damage, hitbox, force to add
            //stops movement
            //add current damage stat to weapon
            _attackAnchor.transform.GetChild(0).gameObject.GetComponent<ObjectDamage>().damage = _enemyTrigger.damage;
            //warmup
            yield return new WaitForSeconds(enemyTypeData.shootWarmup);
            //determine où l'attaque va se faire
            _attackAnchor.transform.LookAt(_player.transform.position);
            //actives weapon
            _enemyTrigger.canTouchPlayer = true;
            _attackAnchor.transform.GetChild(0).gameObject.SetActive(true);
            _rb.velocity = Vector3.zero;
            Vector3 pushedDir = playerDir;
            _rb.AddForce(pushedDir * force, ForceMode.Impulse);
            //plays animation
            //attack duration : time when the player can actually be hit
            yield return new WaitForSeconds(0.4f);
            _attackAnchor.transform.GetChild(0).gameObject.SetActive(false);
            _enemyTrigger.canTouchPlayer = false;
            //waits cooldown depending on the attack used
            yield return new WaitForSeconds(enemyTypeData.attackCooldown - 0.4f);
            _rb.velocity = Vector3.zero;
            _animator.CrossFade(Idle, 0, 0);
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
