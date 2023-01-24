using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.VFX;
using Random = UnityEngine.Random;

public class BullIA : MonoBehaviour
{
    //public int enemyType;
    
    private Vector3 playerDir;
    private Vector3 attackDir;
    private LayerMask wallLayerMask;
    
    //values
    private float _stunCounter;
    private float _playerDist;
    private float _speedFactor;
    private Vector3 _projectileDir;
    private bool _canDash = true;
    private bool _isDashing;
    private bool _isHit;
    private bool _isTouchingWall;
    private bool _isAttacking;
    private float dashFactor;
    public bool isBigBull;
    private bool _wallInSight;
    private float _bigBullDriftLenght = 0.5f;

    //components
    private NavMeshAgent _agent;
    private GameObject _player;
    private Rigidbody _rb;
    private EnemyType enemyTypeData;
    private Enemy _enemyTrigger;
    
    [SerializeField]private Animator _animator;
    public GameObject _sprite;
    public int currentAnimatorState;
    private static readonly int Idle = Animator.StringToHash("Idle");
    private static readonly int Attack = Animator.StringToHash("Attack");
    private static readonly int Stun = Animator.StringToHash("Stun");
    public GameObject dashFx;
    public ParticleSystem stunFx;
    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _agent = GetComponent<NavMeshAgent>();
        _player = GameObject.Find("Player");
        _rb = GetComponent<Rigidbody>();
        _enemyTrigger = GetComponent<Enemy>();
        enemyTypeData = _enemyTrigger.enemyTypeData;

        wallLayerMask = LayerMask.GetMask("Wall", "Pont");
        GetComponent<EnemyDamage>().damage = _enemyTrigger.damage;
        stunFx.gameObject.SetActive(false);
    }

    private void Update()
    {
        _agent.speed = _enemyTrigger.speed * _speedFactor * enemyTypeData.enemySpeed;
        _playerDist = (_player.transform.position - transform.position).magnitude;
        playerDir = (_player.transform.position - transform.position).normalized;

        StunProcess();
        WallCheck();
    }

    private void FixedUpdate()
    {
        _stunCounter -= Time.deltaTime;
        ForceManagement();
        MaxSpeed();
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

        if (_wallInSight && _isDashing)
        {
            _rb.velocity = new Vector3(_rb.velocity.x * 0.8f, _rb.velocity.y, _rb.velocity.z * 0.8f);
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
                if (isBigBull)
                {
                    StartCoroutine(BigBullDrift());
                }
                else
                {
                    StartCoroutine(Dash());
                }
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
        _animator.CrossFade(Attack, 0, 0);
        currentAnimatorState = Attack;
        //stops movement
        _rb.velocity = Vector3.zero;
        dashFactor = -0.1f;
        //adds force to character
        _isDashing = true;
        //waits for the attack to start
        yield return new WaitForSeconds(enemyTypeData.dashWarmUp);
        _audioSource.clip = GameSounds.instance.bullDash[Random.Range(0, GameSounds.instance.bullDash.Length)];
        _audioSource.Play();
        _enemyTrigger.canTouchPlayer = true;
        _sprite.SetActive(false);
        dashFx.gameObject.SetActive(true);
        dashFactor = 1;
        //fonce jusuqu'a toucher un mur
        yield return new WaitUntil(() => _isTouchingWall);
        _audioSource.Stop();
        _audioSource.PlayOneShot(GameSounds.instance.bullHurt[Random.Range(0, GameSounds.instance.bullHurt.Length)]);
        stunFx.gameObject.SetActive(true);
        stunFx.Play();
        _sprite.SetActive(true);
        _animator.CrossFade(Stun, 0, 0);
        currentAnimatorState = Stun;
        _isDashing = false;
        _enemyTrigger.canTouchPlayer = false;
        //stops
        _rb.velocity = Vector3.zero;
        //recoil
        _rb.AddForce(-attackDir * enemyTypeData.recoilForce, ForceMode.Impulse);
        //stuns for a bit
        _stunCounter = enemyTypeData.stunLenght;
        //can dash again when not stun
        dashFx.gameObject.SetActive(false);
        _animator.CrossFade(Idle, 0, 0);
        currentAnimatorState = Idle;
        yield return new WaitForSeconds(enemyTypeData.stunLenght);
        _canDash = true;
    }

    void StunProcess()
    {
        //when stunned
        if (_stunCounter > 0)
        {
            //is vulnerable
            _speedFactor = 0;
        }
    }

    void ForceManagement()
    {
        if (_isDashing)
        {
            _speedFactor = 0;
            _rb.AddForce(attackDir * (dashFactor * 10 * enemyTypeData.dashForce));
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && _player.GetComponent<PlayerController>().stunCounter < 0)
        {
            if (_isDashing)
            {
                //bumps the player
                PlayerController.instance.stunCounter = 1.5f;
                _player.GetComponent<Rigidbody>().AddForce(playerDir * enemyTypeData.bumpForce);
            }
        }
        if (other.CompareTag("PlayerAttack") && _isDashing && PlayerAttacks.instance.smashState != PlayerAttacks.SmashState.None)
        {
            _isTouchingWall = true;
            Debug.Log("stopped by player");
        }
        
        if (other.CompareTag("Wall"))
        {
            if (_isDashing)
            {
                _isTouchingWall = true;
            }
        }
    }

    void WallCheck()
    {
        Debug.DrawRay(transform.position + Vector3.down * 1.5f, (attackDir  ) * 6 ,Color.red);
        if (Physics.Raycast(transform.position + Vector3.down * 1.5f, (attackDir  ), 6, wallLayerMask))
        {
            _isTouchingWall = true;
        }
        else
        {
            _isTouchingWall = false;
        }

        if (isBigBull)
        {
            //drifts if a wall is upfront
            if (Physics.Raycast(transform.position, attackDir, 10, wallLayerMask))
            {
                _wallInSight = true;
            }
            else
            {
                _wallInSight = false;
            }
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

    IEnumerator BigBullDrift()
    {
        //stops movement
        _rb.velocity = Vector3.zero;
        dashFactor = -0.1f;
        //adds force to character
        _isDashing = true;
        //waits for the attack to start
        yield return new WaitForSeconds(enemyTypeData.dashWarmUp);
        
        //fonce jusuqu'a toucher un mur
        dashFactor = 1;
        yield return new WaitUntil(() => _wallInSight);
        
        //drifte - ralentit de fou
        dashFactor = 0;
        
        yield return new WaitForSeconds(_bigBullDriftLenght);
        attackDir = playerDir;
        
        //fonce une nouvelle fois jusuqu'a toucher un mur
        dashFactor = 1;
        yield return new WaitUntil(() => _isTouchingWall);
        
        //stops
        _isDashing = false;
        _rb.velocity = Vector3.zero;
        //recoil
        _rb.AddForce(-attackDir * enemyTypeData.recoilForce, ForceMode.Impulse);
        //stuns for a bit
        _stunCounter = enemyTypeData.stunLenght;
        //can dash again when not stun
        _canDash = true;
    }
}
