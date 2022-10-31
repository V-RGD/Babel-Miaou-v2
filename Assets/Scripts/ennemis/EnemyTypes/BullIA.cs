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
    private bool _isDashing;
    private bool _isHit;
    private bool _isTouchingWall;
    private bool _isAttacking;
    private bool _isVulnerable;
    private float dashFactor;

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
        playerDir = (_player.transform.position - transform.position);

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
        ForceManagement();
        MaxSpeed();
        WallCheck();
        Friction();

        if (_stunCounter < 0 && !_isDashing && _enemyTrigger.isActive)
        {
            //if is not stunned by player
            //main behaviour
            Bull();
        }
    }
    
    void Friction()
    {
        if (_stunCounter > 0)
        {
            _rb.velocity = new Vector3(_rb.velocity.x * 0.95f, _rb.velocity.y, _rb.velocity.z * 0.95f);
        }
    }

    void Bull()
    {
        //if the enemy is in player range and if player is in view 
        if (_playerDist <= enemyTypeData.attackRange)
        {
            //stops then attacks
            _speedFactor = 0;
            if (_canDash)
            {
                _canDash = false;
                attackDir = playerDir;
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
        Debug.Log("meep");
        //stops movement
        _rb.velocity = Vector3.zero;
        dashFactor = -0.1f;
        //adds force to character
        _isDashing = true;
        //waits for the attack to start
        yield return new WaitForSeconds(enemyTypeData.dashWarmUp);
        dashFactor = 1;
        //fonce jusuqu'a toucher un mur
        yield return new WaitUntil(() => _isTouchingWall);
        _isDashing = false;
        //stops
        _rb.velocity = Vector3.zero;
        //recoil
        _rb.AddForce(-attackDir * enemyTypeData.recoilForce, ForceMode.Impulse);
        //stuns for a bit
        _stunCounter = enemyTypeData.stunLenght;
        //can dash again when not stun
        _canDash = true;
    }

    void StunProcess()
    {
        //when stunned
        if (_stunCounter > 0)
        {
            //is vulnerable
            _isVulnerable = true;
            _speedFactor = 0;
        }
        else
        {
            //can't be touched
            _isVulnerable = false;
        }
    }

    void ForceManagement()
    {
        if (_isDashing)
        {
            _speedFactor = 0;
            _rb.AddForce(dashFactor * attackDir * enemyTypeData.dashForce);
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


}
