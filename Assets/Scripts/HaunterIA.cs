using System;
using System.Collections;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class HaunterIA : MonoBehaviour
{
    public float speed;
    public float health;
    public float maxHealth;
    public float damage;
    public float attackRate;
    private float playerDist;
    public float attackRange;

    private float speedFactor;

    private bool isDashing;
    private bool canDash;
    public float dashWarmUp;
    public float dashCooldown;
    public int dashForce;
    public float stunLenght;
    public bool isHit;
    public bool isStunned;
    public float dashLenght;

    public NavMeshAgent agent;
    public NavMeshSurface navMeshSurface;
    public GameObject player;
    public Rigidbody rb;

    public GameObject eyeToken;

    public float stunCounter;

    public bool isInPlayerRange;

    public GameObject healthSlider;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.Find("Player");
        rb = GetComponent<Rigidbody>();
        //navMeshSurface.BuildNavMesh();
        canDash = true;
        health = maxHealth;
        GetComponent<EnemyDamage>().damage = damage;
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
    }

    private void FixedUpdate()
    {
        stunCounter -= Time.deltaTime;
    }

    IEnumerator Dash()
    {
        isDashing = true;
        yield return new WaitForSeconds(dashWarmUp);
        rb.AddForce((player.transform.position - transform.position).normalized * dashForce);
        yield return new WaitForSeconds(dashLenght);
        rb.velocity = Vector3.zero;
        yield return new WaitForSeconds(dashCooldown);
        isDashing = false;
        canDash = true;
    }

    void Behaviour()
    {
        //if the enemy is in player range
        if (playerDist <= attackRange)
        {
            isInPlayerRange = true;
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
            isInPlayerRange = false;
            speedFactor = 1;
            //follows player
            agent.SetDestination(player.transform.position);
        }
    }

    void Death()
    {
        Instantiate(eyeToken);
        Destroy(gameObject);
    }

    void StunProcess()
    {
        //when stunned
        if (stunCounter > 0)
        {
            isStunned = true;
            speedFactor = 0;
        }
        else
        {
            isStunned = false;
        }
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
        if (other.CompareTag("PlayerAttack"))
        {
            health -= other.GetComponent<ObjectDamage>().damage;
        }
    }
}
