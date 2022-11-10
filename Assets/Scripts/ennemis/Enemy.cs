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
    private bool isTank;

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
        _player = GameObject.Find("Player");
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        spawnZone = transform.GetChild(0).gameObject;
        _spriteRenderer = transform.GetChild(2).GetComponent<SpriteRenderer>();
        isActive = false;
        _spriteRenderer.enabled = false;
        spawnZone.SetActive(false);
        health = enemyTypeData.maxHealth;
        _uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();
        _rb = GetComponent<Rigidbody>();
        _rb.useGravity = false;
        mainCollider.enabled = false;
        _agent = GetComponent<NavMeshAgent>();

        //check if the associated ia is a haunter with tank specs
        if (GetComponent<HaunterIA>())
        {
            HaunterIA ia = GetComponent<HaunterIA>();
            isTank = ia.isTank;
        }
    }

    void Update()
    {
        if ((startSpawning && canInitiateSpawning ) || Input.GetKeyDown(KeyCode.T))
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
            if (isTank)
            {
                //tanks take a hit before being vulnerable
                isTank = false;
            }
            else
            {
                //receives damage
                _gameManager.DealDamageToEnemy(other.GetComponent<ObjectDamage>().damage, this);
            }
        }
        
        //deals damage
        if (other.CompareTag("Player") && _player.GetComponent<PlayerController>().stunCounter < 0)
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
