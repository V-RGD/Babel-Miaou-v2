using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PullIA : MonoBehaviour
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
    private Enemy _enemyTrigger;

    [Header("*Objects*")]
    public GameObject healthSlider;
    
    private void Start()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _player = GameObject.Find("Player");
        _canShootProjectile = true;
        _health = enemyTypeData.maxHealth;
        _enemyTrigger = GetComponent<Enemy>();
    }

    private void Update()
    {
        if (_health <= 0)
        {
            //dies
            Death();
        }

        StunProcess();
        SliderUpdate();
    }

    private void FixedUpdate()
    {
        _stunCounter -= Time.deltaTime;
        
        if (!_isStunned && _enemyTrigger.isActive)
        {
            //if is not stunned by player
            Behaviour();
        }
    }

    

    void Behaviour()
    {
        if (_canShootProjectile)
        {
            _canShootProjectile = false;
            StartCoroutine(ShootProjectile());
        }
    }
    
    IEnumerator ShootProjectile()
    {
        //shoots a projectile
        GameObject projN = Instantiate(enemyTypeData.mageProjectile, transform.position + Vector3.forward * 5, quaternion.identity);
        GameObject projS = Instantiate(enemyTypeData.mageProjectile, transform.position + Vector3.back * 5, quaternion.identity);        
        GameObject projE = Instantiate(enemyTypeData.mageProjectile, transform.position + Vector3.left * 5, quaternion.identity);
        GameObject projW = Instantiate(enemyTypeData.mageProjectile, transform.position + Vector3.right * 5, quaternion.identity);
        yield return new WaitForSeconds(enemyTypeData.shootWarmup);
        //gives it proper force
        projN.GetComponent<Rigidbody>().AddForce(new Vector3(20, 0, 20) * enemyTypeData.projectileForce);
        projN.GetComponent<ProjectileDamage>().damage = enemyTypeData.projectileDamage;
        
        projS.GetComponent<Rigidbody>().AddForce(new Vector3(-20, 0, -20) * enemyTypeData.projectileForce);
        projS.GetComponent<ProjectileDamage>().damage = enemyTypeData.projectileDamage;
        
        projE.GetComponent<Rigidbody>().AddForce(new Vector3(-20, 0, 20) * enemyTypeData.projectileForce);
        projE.GetComponent<ProjectileDamage>().damage = enemyTypeData.projectileDamage;
        
        projW.GetComponent<Rigidbody>().AddForce(new Vector3(20, 0, -20) * enemyTypeData.projectileForce);
        projW.GetComponent<ProjectileDamage>().damage = enemyTypeData.projectileDamage;
        
        //se tp 
        yield return new WaitForSeconds(1);
        //calculates a random position where the enemy will spawn
        int randPosX = Random.Range(-20, 21);
        int randPosY = Random.Range(-20, 21);
        Vector3 tpPoint = transform.position + new Vector3(randPosX, _player.transform.position.y + 5, randPosY);
        transform.position = tpPoint;
        
        
        //les rappelle
        if (projN != null)
        {        
            projN.GetComponent<Rigidbody>().velocity = Vector3.zero;
            projN.GetComponent<Rigidbody>().AddForce((transform.position - projN.transform.position) * enemyTypeData.projectileForce);
        }

        if (projS != null)
        {
            projS.GetComponent<Rigidbody>().velocity = Vector3.zero;
            projS.GetComponent<Rigidbody>().AddForce((transform.position - projS.transform.position) * enemyTypeData.projectileForce);
        }

        if (projE != null)
        {
            projE.GetComponent<Rigidbody>().velocity = Vector3.zero;
            projE.GetComponent<Rigidbody>().AddForce((transform.position - projE.transform.position) * enemyTypeData.projectileForce);
        }

        if (projW != null)
        {
            projW.GetComponent<Rigidbody>().velocity = Vector3.zero;
            projW.GetComponent<Rigidbody>().AddForce((transform.position - projW.transform.position) * enemyTypeData.projectileForce);
        }

        //waits for cooldown to refresh to shoot again
        yield return new WaitForSeconds(enemyTypeData.attackCooldown);
        //can shoot again
        _canShootProjectile = true;
    }

    void StunProcess()
    {
        //when stunned
        if (_stunCounter > 0)
        {
            _isStunned = true;
            _speedFactor = 0;
        }
        else
        {
            _isStunned = false;
        }
    }
    
    void Death()
    {
        Instantiate(enemyTypeData.eyeToken, transform.position, quaternion.identity);
        Destroy(gameObject);
    }

    void SliderUpdate()
    {
        if (_health == enemyTypeData.maxHealth)
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
            _stunCounter = enemyTypeData.stunLenght;
        }
        
        if (other.CompareTag("Player"))
        {
            _gameManager.health -= enemyTypeData.damage;
            _player.GetComponent<PlayerController>().invincibleCounter = _player.GetComponent<PlayerController>().invincibleTime;
        }
    }
}
