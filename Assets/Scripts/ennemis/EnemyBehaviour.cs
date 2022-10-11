using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyBehaviour : MonoBehaviour
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

    //components
    private NavMeshAgent _agent;
    private GameObject _player;
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

        _health = enemyTypeData.maxHealth;
        wallLayerMask = LayerMask.GetMask("Wall");
        GetComponent<EnemyDamage>().damage = enemyTypeData.damage;
    }

    private void Update()
    {
        _agent.speed = enemyTypeData.speed * _speedFactor;
        _playerDist = (_player.transform.position - transform.position).magnitude;
        playerDir = _player.transform.position - transform.position;

        if (!_isStunned && !_isDashing)
        {
            //if is not stunned by player
            //main behaviour
            /*switch (enemyType)
            {
                case 0 : Haunter(); break;
                case 1 : Bull(); break;
                case 2 : Mage(); break;
                case 3 : Sniper(); break;
                case 4 : AspiBoum(); break;
            }*/
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
    }
    
    void Behaviour()
    {
        
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

    void Mage()
    {
        //follows player
        //agent.SetDestination(player.transform.position);
        
        //calculates the distance between object and player
        _projectileDir = _player.transform.position - transform.position;

        //if the enemy is too far away, gets closer
        if (_playerDist >= enemyTypeData.attackRange)
        {
            //avance
            _speedFactor = 1;
        }
        
        //if the enemy is in player rangetoo close, walks back to position
        if (_playerDist < enemyTypeData.attackRange)
        {
            //recule
            _speedFactor = 0;
            if (_rb.velocity.magnitude < enemyTypeData.speed) 
            { 
                _rb.AddForce(-_projectileDir, ForceMode.Impulse);
            }
        }
            
        if (_canShootProjectile)
        {
            _canShootProjectile = false;
            StartCoroutine(ShootProjectile());
        }
    }
    
    void Haunter()
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

    void Sniper()
    {
        //shots to target every 1/attack rate
    }

    void AspiBoum()
    {
        //flem d'expliquer
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
        yield return new WaitForSeconds(enemyTypeData.attackCooldown);
        //can shoot again
        _canShootProjectile = true;
    }
    
    
    IEnumerator Dash()
    {
        _isDashing = true;
        yield return new WaitForSeconds(enemyTypeData.dashWarmUp);
        //fonce dans une seule direction
        _rb.AddForce(playerDir * enemyTypeData.dashForce);
        yield return new WaitUntil(() => _isTouchingWall);
        //when touching a wall
        _rb.velocity = Vector3.zero;
        //stuns for a bit
        _stunCounter = enemyTypeData.stunLenght;
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
        
        if (other.CompareTag("Player"))
        {
            _gameManager.health -= enemyTypeData.damage;
            _player.GetComponent<PlayerController>().invincibleCounter = _player.GetComponent<PlayerController>().invincibleTime;
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
