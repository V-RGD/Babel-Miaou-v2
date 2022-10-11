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
    private bool _canMove;
    private bool _isDashing;
    private bool _isAttacking;
    private bool _isHit;
    private bool _isTouchingWall;
    private bool _isVulnerable;

    private Enemy _enemyTrigger;

    //components
    private NavMeshAgent _agent;
    private GameObject _player;
    private GameObject _attackAnchor;
    private GameManager _gameManager;
    private Rigidbody _rb;
    public EnemyType enemyTypeData;
    
    [Header("*Objects*")]
    public GameObject healthSlider;
    
    private void Awake()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _agent = GetComponent<NavMeshAgent>();
        _player = GameObject.Find("Player");
        _rb = GetComponent<Rigidbody>();
        _attackAnchor = transform.GetChild(1).gameObject;
        _enemyTrigger = GetComponent<Enemy>();

        _health = enemyTypeData.maxHealth;
        wallLayerMask = LayerMask.GetMask("Wall");
        GetComponent<EnemyDamage>().damage = enemyTypeData.damage;
    }

    private void Update()
    {
        _agent.speed = enemyTypeData.speed * _speedFactor;
        _playerDist = (_player.transform.position - transform.position).magnitude;
        playerDir = (_player.transform.position - transform.position).normalized;

        if (!_isStunned && _enemyTrigger.isActive)
        {
            //if is not stunned by player
            //main behaviour
            Haunter();
        }

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

        SliderUpdate();
    }

    private void FixedUpdate()
    {
        _stunCounter -= Time.deltaTime;
        Friction();
        StunProcess();
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
            float force = enemyTypeData.attackForce;
            //determines attack length, damage, hitbox, force to add
            Vector3 lookAtDir = _player.transform.position;

            //determine ou l'attaque va se faire
            _attackAnchor.transform.LookAt(lookAtDir);
            //stops movement
            _canMove = false;
            //add current damage stat to weapon
            _attackAnchor.transform.GetChild(0).gameObject.GetComponent<ObjectDamage>().damage = enemyTypeData.damage;
            //warmup
            yield return new WaitForSeconds(enemyTypeData.shootWarmup);
            //actives weapon
            _attackAnchor.transform.GetChild(0).gameObject.SetActive(true);
            _rb.velocity = Vector3.zero;
            Vector3 pushedDir = playerDir;
            _rb.AddForce(pushedDir * force, ForceMode.Impulse);
            //plays animation
            
            //attack duration : time when the player can actually be hit
            yield return new WaitForSeconds(0.4f);
            _attackAnchor.transform.GetChild(0).gameObject.SetActive(false);
            //waits cooldown depending on the attack used
            yield return new WaitForSeconds(enemyTypeData.attackCooldown - 0.4f);
            _rb.velocity = Vector3.zero;
            //restores speed
            _canMove = true;
            //disables hitbox
            _attackAnchor.transform.GetChild(0).gameObject.SetActive(false);
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

    void Death()
    {
        for (int i = 0; i < enemyTypeData.eyesDropped; i++)
        {
            Instantiate(enemyTypeData.eyeToken, transform.position, quaternion.identity);
        }
        Debug.Log("dies");
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerAttack"))
        {
            _health -= other.GetComponent<ObjectDamage>().damage;
        }
        
        if (other.CompareTag("Player") && _player.GetComponent<PlayerController>().stunCounter < 0)
        {
            _gameManager.health -= enemyTypeData.damage;
            _player.GetComponent<PlayerController>().invincibleCounter = _player.GetComponent<PlayerController>().invincibleTime;
        }
    }
}
