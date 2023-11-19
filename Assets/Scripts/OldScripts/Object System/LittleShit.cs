using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

public class LittleShit : MonoBehaviour
{
    public static LittleShit instance;
    
    //si oeil, courts vers l'oeil
    public int eyesInInventory;
    public NavMeshAgent agent;
    public GameObject _player;
    private Rigidbody _rb;
    private GameManager_old _gameManager;
    private LevelManager _lm;
    
    public bool isFlippingSprite;
    private float _flipCounter;
    public GameObject sprite;
    private float _turnSpeed = 10;

    public Animator animator;
    private static readonly int Walk = Animator.StringToHash("Walk");
    public readonly int Eat = Animator.StringToHash("Eat");
    private static readonly int Idle = Animator.StringToHash("Idle");
    private static readonly int Burp = Animator.StringToHash("Burp");
    private static readonly int Hit = Animator.StringToHash("Hit");

    public bool isActive;
    public bool isStun;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        _rb = GetComponent<Rigidbody>();
        _player = GameObject.Find("Player");
    }

    private IEnumerator Start()
    {
        yield return new WaitUntil(()=>DunGen.instance.finishedGeneration);
        animator = transform.GetChild(0).GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        _gameManager = GameManager_old.instance;
        _lm = LevelManager.instance;
        transform.position = new Vector3(_player.transform.position.x, 0, _player.transform.position.z);
        isActive = true;
        ObjectsManager_old.instance.eyeCollector = gameObject;
    }

    private void Update()
    {
        FlipSprite();

        if (isStun)
        {
            agent.enabled = false;
        }
        else
        {
            _rb.velocity = new Vector3(_rb.velocity.x, 0, _rb.velocity.z);
            if (isActive && ObjectsManager_old.instance.eyeCollectorActive)
            {
                agent.enabled = true;
                Direction();
                SpawnChest();
                Animation();
            }
        }
    }

    void Direction()
    {
        //si oeil, va vers l'oeil
        if (_gameManager.eyesInGame.Count > 0 && (_gameManager.eyesInGame[0].position - transform.position).magnitude < 35)
        {
            agent.SetDestination(_gameManager.eyesInGame[0].position);
            agent.stoppingDistance = 0.1f;
        }
        else
        {
            if ((_player.transform.position - transform.position).magnitude > 9)
            {
                //si pas d'oeils, va vers le joueur
                agent.SetDestination(_player.transform.position);
                agent.stoppingDistance = 5f;
            }
        }
    }

    void SpawnChest()
    {
        if (eyesInInventory > ObjectsManager_old.instance.gameVariables.eyeCollectorCollectCeil)
        {
            animator.CrossFade(Burp, 0, 0);
            eyesInInventory -= ObjectsManager_old.instance.gameVariables.eyeCollectorCollectCeil;
            _lm.chest = Instantiate(_lm.chest, transform.position + Vector3.up, quaternion.identity);
        }
    }

    public void TpToPlayer()
    {
        //teleports to player if too far away
        if ((_player.transform.position - transform.position).magnitude > 20)
        {
            //Debug.Log("tped to player");
            agent.enabled = false;
            transform.position = new Vector3(_player.transform.position.x, 0, _player.transform.position.z);
            agent.enabled = true;
        }
    }

    void Animation()
    {
        if (!agent.pathPending && !isStun)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    animator.CrossFade(Idle, 0, 0);
                }
            }
            else
            {
                animator.CrossFade(Walk, 0, 0);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerAttack"))
        {
            isStun = true;
            float force = 0;
            float bumpLength = 0;
            if (PlayerAttacks_old.instance.comboState is PlayerAttacks_old.ComboState.SimpleAttack or PlayerAttacks_old.ComboState.ReverseAttack or PlayerAttacks_old.ComboState.ThirdAttack or PlayerAttacks_old.ComboState.SpinAttack)
            {
                force = 5;
                bumpLength = 0.3f;
            }
            else if (PlayerAttacks_old.instance.smashPowerTimer > 0.3f)
            {
                if (PlayerAttacks_old.instance.smashState == PlayerAttacks_old.SmashState.Blue)
                {
                    force = 7;
                    bumpLength = 0.4f;
                }
                if (PlayerAttacks_old.instance.smashState == PlayerAttacks_old.SmashState.Orange)
                {
                    force = 10;
                    bumpLength = 0.5f;
                }
                if (PlayerAttacks_old.instance.smashState == PlayerAttacks_old.SmashState.Purple)
                {
                    force = 15;
                    bumpLength = 0.7f;
                }
            }
            //Debug.Log("hit by player");
            Vector3 bumpDir = other.transform.position - transform.position;
            _rb.AddForce(force * -bumpDir.normalized, ForceMode.Impulse);
            _rb.AddForce(force/2 * Vector3.up, ForceMode.Impulse);
            StartCoroutine(BumpStun(bumpLength));
            animator.CrossFade(Hit, 0, 0);
        }
    }

    IEnumerator BumpStun(float time)
    {
        yield return new WaitForSeconds(time);
        isStun = false; 
        _rb.velocity = Vector3.zero;
    }
    private void FlipSprite()
    {
        Vector3 playerDir = -_player.transform.position + transform.position;
        if (!isFlippingSprite)
        {
            if (playerDir.x > 0 && _flipCounter < 1)
            {
                _flipCounter += Time.deltaTime * _turnSpeed;
                sprite.transform.localScale = new Vector3(-_flipCounter * 1, 1.5f, 1);
            }
            if (playerDir.x < 0 && _flipCounter > -1)
            {
                _flipCounter -= Time.deltaTime * _turnSpeed;
                sprite.transform.localScale = new Vector3(-_flipCounter * 1, 1.5f, 1);
            }
        }
    }
}
