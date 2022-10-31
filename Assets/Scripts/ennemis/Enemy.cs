using System.Collections;
using Unity.AI.Navigation;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    private float _health;
    public bool startSpawning;
    private bool canInitiateSpawning = true;
    public bool isActive;
    private bool isTank;

    private GameObject _player;
    public GameObject healthSlider;
    private GameObject spawnZone;
    
    private GameManager _gameManager;
    private SpriteRenderer _spriteRenderer;
    public EnemyType enemyTypeData;
    
    void Start()
    {
        _player = GameObject.Find("Player");
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        spawnZone = transform.GetChild(0).gameObject;
        _spriteRenderer = transform.GetChild(2).GetComponent<SpriteRenderer>();
        isActive = false;
        _spriteRenderer.enabled = false;
        spawnZone.SetActive(true);
        _health = enemyTypeData.maxHealth;
       
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
        
        if (_health <= 0)
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
        GetComponent<NavMeshAgent>().enabled = true;
        isActive = true;
        _spriteRenderer.enabled = true;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        //receives damage
        if (other.CompareTag("PlayerAttack"))
        {
            if (isTank)
            {
                isTank = false;
            }
            else
            {
                TakeDamage(other.GetComponent<ObjectDamage>().damage);
            }
        }
        
        //deals damage
        if (other.CompareTag("Player") && _player.GetComponent<PlayerController>().stunCounter < 0)
        {
            DealDamage(enemyTypeData.damage);
        }
    }

    void TakeDamage(float damageTaken)
    {
        //receives damage and applies object effects accordingly
        _health -= damageTaken;
        //_stunCounter = enemyTypeData.stunLenght;
    }

    void DealDamage(float damageDealt)
    {
        //deals damage to player and applies object affects accordingly
        _gameManager.health -= Mathf.CeilToInt(damageDealt);
        _player.GetComponent<PlayerController>().invincibleCounter = _player.GetComponent<PlayerController>().invincibleTime;
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
