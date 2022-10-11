using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class BullIA : MonoBehaviour
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
    private bool _canDash = true;
    private bool _isStunned;
    private bool _isDashing;
    private bool _isHit;
    private bool _isTouchingWall;
    private bool _isAttacking;
    private bool _isVulnerable;
    private float _dashDir;

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
        playerDir = (_player.transform.position - transform.position).normalized;

        if (!_isStunned && !_isDashing && _enemyTrigger.isActive)
        {
            //if is not stunned by player
            //main behaviour
            Bull();
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

        StunProcess();
        SliderUpdate();
        WallCheck();
    }

    private void FixedUpdate()
    {
        _stunCounter -= Time.deltaTime;
        Mathf.Clamp(_rb.velocity.magnitude, 10f, 20f);
        Friction();
        ForceManagement();
        MaxSpeed();
    }
    
    void Friction()
    {
        if (!_isDashing)
        {
            _agent.speed = 0;
            _rb.velocity = new Vector3(_rb.velocity.x * 0.95f, _rb.velocity.y, _rb.velocity.z * 0.95f);
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

    void Bull()
    {
        
        //if the enemy is in player range
        if (_playerDist <= enemyTypeData.attackRange)
        {
            //stops then attacks
            _speedFactor = 0;
            if (_canDash)
            {
                _canDash = false;
                StartCoroutine(Dash());
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

    IEnumerator Dash()
    {
        _isDashing = true;
        attackDir = playerDir;
        _dashDir = -0.15f;
        yield return new WaitForSeconds(enemyTypeData.dashWarmUp);
        //fonce dans une seule direction
        _dashDir = 1;
        yield return new WaitUntil(() => _isTouchingWall);
        //when touching a wall
        _dashDir = 0;
        _rb.velocity = Vector3.zero;
        _rb.AddForce(-attackDir * enemyTypeData.recoilForce);
        //stuns for a bit
        _stunCounter = enemyTypeData.stunLenght;
        _rb.velocity = Vector3.zero;
        //can dash again when not stun
        _isDashing = false;
        _canDash = true;
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

    void ForceManagement()
    {
        if (_dashDir != 0)
        {
            _rb.AddForce(_dashDir * attackDir * enemyTypeData.dashForce);
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
            //deals damage
            _gameManager.health -= enemyTypeData.damage;
            _player.GetComponent<PlayerController>().invincibleCounter = _player.GetComponent<PlayerController>().invincibleTime;

            if (_isDashing)
            {
                //bumps the player
                _player.GetComponent<PlayerController>().stunCounter = 1.5f;
                _player.GetComponent<Rigidbody>().AddForce(playerDir * enemyTypeData.bumpForce);
            }
        }
    }

    void WallCheck()
    {
        if (Physics.Raycast(transform.position, attackDir, 4, wallLayerMask))
        {
            _isTouchingWall = true;
        }
        else
        {
            _isTouchingWall = false;
        }
    }


}
