using System.Collections;
using Unity.AI.Navigation;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public float health;
    public bool startSpawning;
    private bool canInitiateSpawning = true;
    public bool isActive;
    private bool _isTank;
    private float _stunCounter;

    private GameObject _player;
    public GameObject healthSlider;
    private GameObject spawnZone;
    public BoxCollider mainCollider;
    private Rigidbody _rb;

    private GameManager _gameManager;
    private UIManager _uiManager;
    private SpriteRenderer _spriteRenderer;
    public EnemyType enemyTypeData;
    private NavMeshAgent _agent;

    [HideInInspector]public GameObject room;

    void Start()
    {
        _gameManager = GameManager.instance;
        _uiManager = UIManager.instance;
        _rb = GetComponent<Rigidbody>();
        _agent = GetComponent<NavMeshAgent>();
        _player = GameObject.Find("Player");
        spawnZone = transform.GetChild(0).gameObject;
        _spriteRenderer = transform.GetChild(2).GetComponent<SpriteRenderer>();
        spawnZone.SetActive(false);
        health = enemyTypeData.maxHealth;
        _spriteRenderer.enabled = false;
        isActive = false;
        _rb.useGravity = false;
        mainCollider.enabled = false;

        //check if the associated ia is a haunter with tank specs
        if (GetComponent<HaunterIA>())
        {
            HaunterIA ia = GetComponent<HaunterIA>();
            _isTank = ia.isTank;
        }
    }

    void Update()
    {
        if (startSpawning && canInitiateSpawning)
        {
            GameObject.Find("NavMeshSurface").GetComponent<NavMeshSurface>().BuildNavMesh();
            canInitiateSpawning = false;
            StartCoroutine(EnemyApparition());
        }
        
        if (health <= 0)
        {
            //dies
            Death();
        }
        SliderUpdate();
    }

    IEnumerator EnemyApparition()
    {
        _spriteRenderer.enabled = false;
        //spawn zone appears
        spawnZone.SetActive(true);
        //vfx plays
        yield return new WaitForSeconds(2);
        //then enemy spawns
        spawnZone.SetActive(false);
        _agent.enabled = true;
        _rb.useGravity = true;
        mainCollider.enabled = true;
        isActive = true;
        _spriteRenderer.enabled = true;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        //if player hit
        if (other.CompareTag("PlayerAttack"))
        {
            if (_isTank)
            {
                //tanks take a hit before being vulnerable
                _isTank = false;
            }
            else
            {
                //receives damage
                _gameManager.DealDamageToEnemy(other.GetComponent<ObjectDamage>().damage, this);
            }
            _rb.AddForce((_player.transform.position - transform.position) * -20, ForceMode.Impulse);
            _stunCounter = 1;
        }
        
        //deals damage
        if (other.CompareTag("Player") && PlayerController.instance.stunCounter < 0 && !PlayerController.instance._playerAttacks.isAttacking)
        {
            _gameManager.DealDamageToPlayer(enemyTypeData.damage);
        }
    }

    void TakeDamage(float damageTaken)
    {
        //receives damage and applies object effects accordingly
        health -= damageTaken;
        //_stunCounter = enemyTypeData.stunLenght;
    }

    void Death()
    {
        for (int i = 0; i < enemyTypeData.eyesDropped; i++)
        {
            Instantiate(enemyTypeData.eyeToken, transform.position, quaternion.identity);
        }
        Destroy(gameObject);
    }
    
    void SliderUpdate()
    {
        if (health >= enemyTypeData.maxHealth)
        {
            healthSlider.SetActive(false);
        }
        else
        {
            healthSlider.SetActive(true);
            healthSlider.GetComponent<Slider>().value = health / enemyTypeData.maxHealth;
        }
    }
}
