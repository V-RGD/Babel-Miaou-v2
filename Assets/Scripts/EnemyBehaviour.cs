using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyBehaviour : MonoBehaviour
{
    [Header("Enemy Stats")]
    public int enemyType;
    public float speed;
    public float max_health;
    public float damage;
    
    private float dashWarmUp;
    private int dashForce;
    private int eyesDropped;
    private Vector3 dashDir;
    
    private LayerMask wallLayerMask;
    
    private float projectileDamage;
    private float projectileRate;
    public float attackRange;
    private float stunLenght;
    private float shootWarmup;
    
    [Header("Reserved by enemy types")]

    //values
    private int _projectileForce;
    private float _health;
    private float _stunCounter;
    private float _playerDist;
    private float _speedFactor;
    private Vector3 _projectileDir;
    private bool _canShootProjectile;
    private bool _isStunned;
    private bool _isDashing;
    private bool _canDash;
    private bool _isHit;
    private bool _isTouchingWall;
    private bool _isVulnerable;

    //components
    private NavMeshAgent _agent;
    private GameObject _player;
    private GameManager _gameManager;
    private Rigidbody _rb;

    [Header("*Objects*")]
    public GameObject eyeToken;
    public GameObject healthSlider;
    public GameObject mageProjectile;
    
    private void Start()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _agent = GetComponent<NavMeshAgent>();
        _player = GameObject.Find("Player");
        _rb = GetComponent<Rigidbody>();
        _canDash = true;
        _health = max_health;
        wallLayerMask = LayerMask.GetMask("Wall");
        GetComponent<EnemyDamage>().damage = damage;
        _canShootProjectile = true;
    }

    private void Update()
    {
        _agent.speed = speed * _speedFactor;
        _playerDist = (_player.transform.position - transform.position).magnitude;

        if (!_isStunned && !_isDashing)
        {
            //if is not stunned by player
            Behaviour();
        }

        if (_health <= 0)
        {
            //dies
            Death();
        }

        if (_isHit)
        {
            //resets stun counter
            _stunCounter = stunLenght;
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
        if (!_isStunned)
        {
            switch (enemyType)
            {
                case 0 : Haunter(); break;
                case 1 : Bull(); break;
                case 2 : Mage(); break;
                case 3 : Sniper(); break;
                case 4 : AspiBoum(); break;
            }
        }
    }
    
    IEnumerator Dash()
    {
        _isDashing = true;
        yield return new WaitForSeconds(dashWarmUp);
        //fonce dans une seule direction
        _rb.AddForce(dashDir * dashForce);
        yield return new WaitUntil(() => _isTouchingWall);
        //when touching a wall
        _rb.velocity = Vector3.zero;
        //stuns for a bit
        _stunCounter = stunLenght;
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
        for (int i = 0; i < eyesDropped; i++)
        {
            Instantiate(eyeToken, transform.position, quaternion.identity);
        }
        Debug.Log("dies");
        Destroy(gameObject);
    }

    void SliderUpdate()
    {
        if (_health == max_health)
        {
            healthSlider.SetActive(false);
        }
        else
        {
            healthSlider.SetActive(true);
            healthSlider.GetComponent<Slider>().value = _health / max_health;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerAttack") && _isVulnerable)
        {
            _health -= other.GetComponent<ObjectDamage>().damage;
        }
        
        if (other.CompareTag("Player"))
        {
            _gameManager.health -= damage;
            _player.GetComponent<PlayerController>().invincibleCounter = _player.GetComponent<PlayerController>().invincibleTime;
        }
    }

    void WallCheck()
    {
        if (Physics.Raycast(transform.position, dashDir, 4, wallLayerMask))
        {
            _isTouchingWall = true;
        }
        else
        {
            _isTouchingWall = false;
        }
    }

    void Bull()
    {
        dashDir = _player.transform.position - transform.position;
        
        //if the enemy is in player range
        if (_playerDist <= attackRange)
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
        if (_playerDist >= attackRange)
        {
            //avance
            _speedFactor = 1;
        }
        
        //if the enemy is in player rangetoo close, walks back to position
        if (_playerDist < attackRange)
        {
            //recule
            _speedFactor = 0;
            if (_rb.velocity.magnitude < speed) 
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
        dashDir = _player.transform.position - transform.position;
        
        //if the enemy is in player range
        if (_playerDist <= attackRange)
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
        
    }

    void AspiBoum()
    {
        
    }
    
    IEnumerator ShootProjectile()
    {
        yield return new WaitForSeconds(shootWarmup);
        //shoots a projectile
        GameObject projectile = Instantiate(mageProjectile, transform.position, quaternion.identity);
        //gives it proper force
        projectile.GetComponent<Rigidbody>().AddForce(_projectileDir.normalized * _projectileForce);
        projectile.GetComponent<ProjectileDamage>().damage = projectileDamage;
        //waits for cooldown to refresh to shoot again
        yield return new WaitForSeconds(projectileRate);
        //can shoot again
        _canShootProjectile = true;
    }
}
