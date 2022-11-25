using System.Collections;
using Unity.AI.Navigation;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.VFX;

public class Enemy : MonoBehaviour
{
    public float health;
    public bool isActive;
    private bool _isTank;
    private float _stunCounter;

    private GameObject _player;
    public GameObject healthSlider;
    public BoxCollider mainCollider;
    public BoxCollider supportCollider;
    private Rigidbody _rb;

    private GameManager _gameManager;
    private UIManager _uiManager;
    public GameObject sprite;
    public VisualEffect spawnVfx;
    public EnemyType enemyTypeData;
    private NavMeshAgent _agent;
    public ParticleSystem splashFX;

    [HideInInspector]public GameObject room;

    void Start()
    {
        _gameManager = GameManager.instance;
        _uiManager = UIManager.instance;
        _rb = GetComponent<Rigidbody>();
        _agent = GetComponent<NavMeshAgent>();
        _player = GameObject.Find("Player");
        health = enemyTypeData.maxHealth;
        sprite.SetActive(false);
        healthSlider.SetActive(false);
        isActive = false;
        _rb.useGravity = false;
        mainCollider.enabled = false;
        supportCollider.enabled = false;

        //check if the associated ia is a haunter with tank specs
        if (GetComponent<HaunterIA>())
        {
            HaunterIA ia = GetComponent<HaunterIA>();
            _isTank = ia.isTank;
        }
    }

    public IEnumerator EnemyApparition()
    {
        sprite.SetActive(false);
        //vfx plays
        spawnVfx.Play();
        yield return new WaitForSeconds(1);
        sprite.SetActive(true);
        yield return new WaitForSeconds(2);
        //then enemy spawns
        _agent.enabled = true;
        _rb.useGravity = true;
        mainCollider.enabled = true;
        supportCollider.enabled = true;
        isActive = true;
        
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

    public void Death()
    {
        for (int i = 0; i < enemyTypeData.eyesDropped; i++)
        {
            Instantiate(enemyTypeData.eyeToken, transform.position, quaternion.identity);
        }
        Destroy(gameObject);
    }
    
   public void SliderUpdate()
    {
        healthSlider.SetActive(true);
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
