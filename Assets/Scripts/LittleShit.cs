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
    private GameObject _player;
    private Rigidbody _rb;
    private GameManager _gameManager;
    private LevelManager _lm;

    public Animator animator;
    private static readonly int Walk = Animator.StringToHash("Walk");
    public readonly int Eat = Animator.StringToHash("Eat");
    private static readonly int Idle = Animator.StringToHash("Idle");
    private static readonly int Burp = Animator.StringToHash("Burp");

    public bool isActive;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }

        instance = this;
    }

    private IEnumerator Start()
    {
        yield return new WaitUntil(()=>DunGen.instance.finishedGeneration);
        animator = transform.GetChild(0).GetComponent<Animator>();
        _player = GameObject.Find("Player");
        agent = GetComponent<NavMeshAgent>();
        _gameManager = GameManager.instance;
        _lm = LevelManager.instance;
        transform.position = new Vector3(_player.transform.position.x, 0, _player.transform.position.z);
        isActive = true;
        ObjectsManager.instance.eyeCollector = gameObject;
    }

    private void Update()
    {
        

        if (stunTimer > 0)
        {
            stunTimer -= Time.deltaTime;
            agent.enabled = false;
        }
        else
        {
            if (isActive && ObjectsManager.instance.eyeCollectorActive)
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
            //si pas d'oeils, va vers le joueur
            agent.SetDestination(_player.transform.position);
            agent.stoppingDistance = 7f;
        }
    }

    void SpawnChest()
    {
        if (eyesInInventory > ObjectsManager.instance.gameVariables.eyeCollectorCollectCeil)
        {
            animator.CrossFade(Burp, 0, 0);
            eyesInInventory -= ObjectsManager.instance.gameVariables.eyeCollectorCollectCeil;
            _lm.chest = Instantiate(_lm.chest, transform.position + Vector3.up, quaternion.identity);
        }
    }

    public void TpToPlayer()
    {
        //teleports to player if too far away
        if ((_player.transform.position - transform.position).magnitude > 20 && _gameManager.eyesInGame.Count > 0)
        {
            transform.position = new Vector3(_player.transform.position.x, 0, _player.transform.position.z);
        }
    }

    void Animation()
    {
        if (!agent.pathPending)
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

    public float bumpForce;
    public float stunTimer;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerAttack"))
        {
            Debug.Log("hit by player");
            Vector3 bumpDir = other.transform.position - transform.position;
            _rb.AddForce(bumpForce * bumpDir, ForceMode.Impulse);
            stunTimer = 1;
        }
    }
}
