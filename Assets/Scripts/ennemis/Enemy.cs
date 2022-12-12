using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.VFX;

public class Enemy : MonoBehaviour
{
    public float health;
    public float maxHealth;
    public float speed;
    public float eyesLooted;
    public float damage;
    public bool isActive;
    private bool _isTank;
    public float stunCounter;
    private float _poisonCounter;
    private float _flipCounter;
    private float _turnSpeed = 10;
    private bool _canTakePoisonDamage = true;

    private GameObject _player;
    public GameObject healthSlider;
    public BoxCollider mainCollider;
    private Rigidbody _rb;

    private GameManager _gameManager;
    public GameObject sprite;
    public VisualEffect spawnVfx;
    public EnemyType enemyTypeData;
    private NavMeshAgent _agent;
    public ParticleSystem splashFX;
    public ParticleSystem hitFX;
    public bool canTouchPlayer;
    public bool isFlippingSprite;

    [HideInInspector]public GameObject room;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _agent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        _gameManager = GameManager.instance;
        
        _agent.speed = speed;
        maxHealth = health;
        _player = GameObject.Find("Player");
        sprite.SetActive(false);
        healthSlider.SetActive(false);
        isActive = false;
        _rb.useGravity = false;
        mainCollider.enabled = false;
        hitFX.gameObject.SetActive(false);

        //check if the associated ia is a haunter with tank specs
        if (GetComponent<HaunterIA>())
        {
            HaunterIA ia = GetComponent<HaunterIA>();
            _isTank = ia.isTank;
        }
    }

    private void Update()
    {
        stunCounter -= Time.deltaTime;
        if (_poisonCounter > 0 && health > 0)
        {
            _poisonCounter -= Time.deltaTime;
            if (_canTakePoisonDamage)
            {
                StartCoroutine(ResetPoisonCounter());
            }
        }

        if (canTouchPlayer)
        {
            FlipSprite();
        }
        else
        {
            if (_flipCounter is < 1 and > 0)
            {
                _flipCounter = 1;
                sprite.transform.localScale = new Vector3(-_flipCounter, 1, 1);
            }
            if (_flipCounter is > -1 and < 0)
            {
                _flipCounter = -1;
                sprite.transform.localScale = new Vector3(-_flipCounter, 1, 1);
            }
        }
    }

    IEnumerator ResetPoisonCounter()
    {
        _canTakePoisonDamage = false;
        _gameManager.DealDamageToEnemy(ObjectsManager.instance.gameVariables.poisonDamage, this, false);
        yield return new WaitForSeconds(ObjectsManager.instance.gameVariables.poisonCooldown);
        _canTakePoisonDamage = true;
    }

    public IEnumerator EnemyApparition()
    {
        sprite.SetActive(false);
        //vfx plays
        spawnVfx.Play();
        yield return new WaitForSeconds(1);
        sprite.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        //then enemy spawns
        _agent.enabled = true;
        _rb.useGravity = true;
        mainCollider.enabled = true;
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

            if (!hitFX.gameObject.activeInHierarchy)
            {
                hitFX.gameObject.SetActive(true);
            }
            _rb.AddForce((_player.transform.position - transform.position).normalized * -PlayerAttacks.instance.bumpForce, ForceMode.Impulse);
            stunCounter = 1;
        }
        
        //deals damage
        if (other.CompareTag("Player") && canTouchPlayer)
        {
            _gameManager.DealDamageToPlayer(damage);
        }

        if (other.CompareTag("Poison"))
        {
            //gets poisoned
            _poisonCounter = ObjectsManager.instance.gameVariables.poisonLenght;
        }
    }

    public void FlipSprite()
    {
        Vector3 playerDir = _player.transform.position - transform.position;
        if (!isFlippingSprite)
        {
            if (playerDir.x > 0 && _flipCounter < 1)
            {
                _flipCounter += Time.deltaTime * _turnSpeed;
                sprite.transform.localScale = new Vector3(-_flipCounter, 1, 1);
            }
            if (playerDir.x < 0 && _flipCounter > -1)
            {
                _flipCounter -= Time.deltaTime * _turnSpeed;
                sprite.transform.localScale = new Vector3(-_flipCounter, 1, 1);
            }
        }
    }

    public void Death()
    {
        for (int i = 0; i < eyesLooted; i++)
        {
            Instantiate(enemyTypeData.eyeToken, new Vector3(transform.position.x, _player.transform.position.y, transform.position.z), Quaternion.identity);
        }
        Destroy(gameObject);
    }
    
   public void SliderUpdate()
    {
        if (health >= maxHealth)
        {
            healthSlider.SetActive(false);
        }
        else
        {
            if (!healthSlider.activeInHierarchy)
            {
                healthSlider.SetActive(true);
            }
            healthSlider.GetComponent<Slider>().value = health / maxHealth;
        }
    }
}
