using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Aspirateur : MonoBehaviour
{
    public float speed;
    public float maxHealth;
    public float damage;
    public float projectileDamage;
    public float projectileRate;
    public float attackRange;
    public int projectileForce;
    public float stunLenght;
    public float shootWarmup;
    
    private float stunCounter;
    private float health;
    private float playerDist;
    private float speedFactor;
    private Vector3 projectileDir;
    
    private bool canShootProjectile;
    public bool isStunned;
    
    private NavMeshAgent agent;
    private GameObject player;
    private GameManager gameManager;
    public GameObject eyeToken;
    public GameObject healthSlider;
    public GameObject mageProjectile;
    private Rigidbody rb;
    
    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.Find("Player");
        canShootProjectile = true;
        health = maxHealth;
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        agent.speed = speed * speedFactor;
        playerDist = (player.transform.position - transform.position).magnitude;

        if (!isStunned)
        {
            //if is not stunned by player
            Behaviour();
        }

        if (health <= 0)
        {
            //dies
            Death();
        }

        StunProcess();
        SliderUpdate();
    }

    private void FixedUpdate()
    {
        stunCounter -= Time.deltaTime;
    }

    

    void Behaviour()
    {
        //follows player
        agent.SetDestination(player.transform.position);
        
        //calculates the distance between object and player
        projectileDir = player.transform.position - transform.position;

        //if the enemy is too far away, gets closer
        if (playerDist >= attackRange)
        {
            //avance
            speedFactor = 1;
        }
        
        //if the enemy is in player rangetoo close, walks back to position
        if (playerDist < attackRange)
        {
            //recule
            speedFactor = 0;
            if (rb.velocity.magnitude < speed) 
            { 
                rb.AddForce(-projectileDir, ForceMode.Impulse);
            }
        }
            
        if (canShootProjectile)
        {
            canShootProjectile = false;
            StartCoroutine(ShootProjectile());
        }
    }
    
    IEnumerator ShootProjectile()
    {
        yield return new WaitForSeconds(shootWarmup);
        //shoots a projectile
        GameObject projectile = Instantiate(mageProjectile, transform.position, quaternion.identity);
        //gives it proper force
        projectile.GetComponent<Rigidbody>().AddForce(projectileDir.normalized * projectileForce);
        projectile.GetComponent<ProjectileDamage>().damage = projectileDamage;
        //waits for cooldown to refresh to shoot again
        yield return new WaitForSeconds(projectileRate);
        //can shoot again
        canShootProjectile = true;
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
    
    void Death()
    {
        Instantiate(eyeToken, transform.position, quaternion.identity);
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
        if (other.CompareTag("PlayerAttack"))
        {
            health -= other.GetComponent<ObjectDamage>().damage;
            stunCounter = stunLenght;
        }
        
        if (other.CompareTag("Player"))
        {
            gameManager.health -= damage;
            player.GetComponent<PlayerController>().invincibleCounter = player.GetComponent<PlayerController>().invincibleTime;
        }

        
    }
}
