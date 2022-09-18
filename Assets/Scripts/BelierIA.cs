using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class BelierIA : MonoBehaviour
{
    public float speed;
    public float maxHealth;
    public float damage;
    public float attackRange;
    public float dashWarmUp;
    public int dashForce;
    public float stunLenght;
    public int eyesDropped;
    
    private float stunCounter;
    private float health;
    private float playerDist;
    private float speedFactor;
    private Vector3 dashDir;
    
    private bool isDashing;
    private bool canDash;
    public bool isHit;
    public bool isStunned;
    public bool isTouchingWall;
    public bool isVulnerable;

    private LayerMask wallLayerMask;

    private NavMeshAgent agent;
    private GameObject player;
    private Rigidbody rb;
    private GameManager gameManager;
    public GameObject eyeToken;
    public GameObject healthSlider;
    
    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.Find("Player");
        rb = GetComponent<Rigidbody>();
        canDash = true;
        health = maxHealth;
        wallLayerMask = LayerMask.GetMask("Wall");
    }

    private void Update()
    {
        agent.speed = speed * speedFactor;
        playerDist = (player.transform.position - transform.position).magnitude;

        if (!isStunned && !isDashing)
        {
            //if is not stunned by player
            Behaviour();
        }

        if (health <= 0)
        {
            //dies
            Death();
        }

        if (isHit)
        {
            //resets stun counter
            stunCounter = stunLenght;
        }

        StunProcess();
        SliderUpdate();
        WallCheck();
    }

    private void FixedUpdate()
    {
        stunCounter -= Time.deltaTime;
    }

    

    void Behaviour()
    {
        dashDir = player.transform.position - transform.position;
        
        //if the enemy is in player range
        if (playerDist <= attackRange)
        {
            //stops then attacks
            speedFactor = 0;
            if (canDash)
            {
                canDash = false;
                StartCoroutine(Dash());
            }
        }
        
        //if it's too far from attacking
        else
        {
            speedFactor = 1;
            //follows player
            agent.SetDestination(player.transform.position);
        }
    }
    
    IEnumerator Dash()
    {
        isDashing = true;
        yield return new WaitForSeconds(dashWarmUp);
        //fonce dans une seule direction
        rb.AddForce(dashDir * dashForce);
        yield return new WaitUntil(() => isTouchingWall);
        //when touching a wall
        rb.velocity = Vector3.zero;
        //stuns for a bit
        stunCounter = stunLenght;
        //can dash again when not stun
        isDashing = false;
        canDash = true;
    }

    void StunProcess()
    {
        //when stunned
        if (stunCounter > 0)
        {
            isStunned = true;
            //is vulnerable
            isVulnerable = true;
            speedFactor = 0;
        }
        else
        {
            isStunned = false;
            //can't be touched
            isVulnerable = false;
        }
    }
    
    void Death()
    {
        for (int i = 0; i < eyesDropped; i++)
        {
            Instantiate(eyeToken, transform.position, quaternion.identity);
        }
        Destroy(gameObject);
    }

    void SliderUpdate()
    {
        if (health == maxHealth)
        {
            healthSlider.SetActive(false);
        }
        else
        {
            healthSlider.SetActive(true);
            healthSlider.GetComponent<Slider>().value = health / maxHealth;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerAttack") && isVulnerable)
        {
            health -= other.GetComponent<ObjectDamage>().damage;
        }
        
        if (other.CompareTag("Player"))
        {
            gameManager.health -= damage;
            player.GetComponent<PlayerController>().invincibleCounter = player.GetComponent<PlayerController>().invincibleTime;
        }

        
    }

    void WallCheck()
    {
        if (Physics.Raycast(transform.position, dashDir, 4, wallLayerMask))
        {
            isTouchingWall = true;
        }
        else
        {
            isTouchingWall = false;
        }
    }
}
