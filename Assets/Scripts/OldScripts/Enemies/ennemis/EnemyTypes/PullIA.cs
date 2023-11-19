using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class PullIA : MonoBehaviour
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
    private Rigidbody _rb;
    private EnemyType enemyTypeData;
    private Enemy_old _enemyOldTrigger;
    private SpriteRenderer _spriteRenderer;
    public BoxCollider collider;
    private List<GameObject> _projeciles;
    [HideInInspector]public Transform roomPosition;

    private void Start()
    {
        _player = GameObject.Find("Player");
        _canShootProjectile = true;
        _enemyOldTrigger = GetComponent<Enemy_old>();
        _spriteRenderer = transform.GetChild(2).GetComponent<SpriteRenderer>();
        enemyTypeData = _enemyOldTrigger.enemyTypeData;

        //creates a list of projectiles for further use
        _projeciles = new List<GameObject>();
        for (int i = 0; i < 4; i++)
        {
            GameObject projo = Instantiate(enemyTypeData.mageProjectile);
            _projeciles.Add(projo);
            projo.GetComponent<AspireurProjectile>().damage = enemyTypeData.projectileDamage;
            projo.SetActive(false);
        }
    }

    private void Update()
    {
        StunProcess();
    }

    private void FixedUpdate()
    {
        _stunCounter -= Time.deltaTime;
        
        if (!_isStunned && _enemyOldTrigger.isActive)
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
        //Debug.Log("active");
        //actives projo
        _projeciles[0].SetActive(true);
        _projeciles[1].SetActive(true);
        _projeciles[2].SetActive(true);
        _projeciles[3].SetActive(true);
        //stoppe eventuels mouvements
        _projeciles[0].GetComponent<Rigidbody>().velocity = Vector3.zero;
        _projeciles[1].GetComponent<Rigidbody>().velocity = Vector3.zero;
        _projeciles[2].GetComponent<Rigidbody>().velocity = Vector3.zero;
        _projeciles[3].GetComponent<Rigidbody>().velocity = Vector3.zero;
        //shoots projectile
        _projeciles[0].transform.position =  transform.position + Vector3.forward * 5;
        _projeciles[1].transform.position =  transform.position + Vector3.back * 5;
        _projeciles[2].transform.position =  transform.position + Vector3.left * 5;
        _projeciles[3].transform.position =  transform.position + Vector3.right * 5;
        yield return new WaitForSeconds(enemyTypeData.shootWarmup);
        //Debug.Log("shoots");
        //gives it proper force
        _projeciles[0].GetComponent<Rigidbody>().AddForce(new Vector3(20, 0, 20) * enemyTypeData.projectileForce);
        _projeciles[1].GetComponent<Rigidbody>().AddForce(new Vector3(-20, 0, -20) * enemyTypeData.projectileForce);
        _projeciles[2].GetComponent<Rigidbody>().AddForce(new Vector3(-20, 0, 20) * enemyTypeData.projectileForce);
        _projeciles[3].GetComponent<Rigidbody>().AddForce(new Vector3(20, 0, -20) * enemyTypeData.projectileForce);

        yield return new WaitForSeconds(1f);
        // //les stoppe
        // _projeciles[0].GetComponent<Rigidbody>().velocity = Vector3.zero;
        // _projeciles[1].GetComponent<Rigidbody>().velocity = Vector3.zero;
        // _projeciles[2].GetComponent<Rigidbody>().velocity = Vector3.zero;
        // _projeciles[3].GetComponent<Rigidbody>().velocity = Vector3.zero;
        //se tp 
        _spriteRenderer.enabled = false;
        collider.enabled = false;

        yield return new WaitForSeconds(1);

        _spriteRenderer.enabled = true;
        collider.enabled = true;
        //calculates a random position where the enemy will spawn
        int randPosX = Random.Range(-20, 21);
        int randPosY = Random.Range(-20, 21);
        
        Vector3 tpPoint = transform.position + new Vector3(randPosX, _player.transform.position.y + 5, randPosY);
        transform.position = tpPoint;

        //Debug.Log("rappelle");
        //rappelle ses projectiles
        _projeciles[0].GetComponent<Rigidbody>().AddForce((transform.position - _projeciles[0].transform.position) * enemyTypeData.projectileForce);
        _projeciles[1].GetComponent<Rigidbody>().AddForce((transform.position - _projeciles[1].transform.position) * enemyTypeData.projectileForce);
        _projeciles[2].GetComponent<Rigidbody>().AddForce((transform.position - _projeciles[2].transform.position) * enemyTypeData.projectileForce);
        _projeciles[3].GetComponent<Rigidbody>().AddForce((transform.position - _projeciles[3].transform.position) * enemyTypeData.projectileForce);

        yield return new WaitForSeconds(1.5f);
        //Debug.Log("desactive");
        //sets projectiles inactive
        _projeciles[0].SetActive(false);
        _projeciles[1].SetActive(false);
        _projeciles[2].SetActive(false);
        _projeciles[3].SetActive(false);
        
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
}
