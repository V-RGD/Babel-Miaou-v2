using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class AspirateurIA : MonoBehaviour
{
    public float maxHealth;
    public float damage;
    public float projectileDamage;
    public float projectileCooldown = 2;
    public float stunLenght;
    public float shootWarmup;
    
    private float stunCounter;
    private float health;
    private float speedFactor;
    public float shootForce;
    public float retreatForce;
    
    private bool canShootProjectile;
    public bool isStunned;
    
    private GameObject player;
    private GameManager gameManager;
    public GameObject eyeToken;
    public GameObject healthSlider;
    public GameObject mageProjectile;
    
    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        player = GameObject.Find("Player");
        canShootProjectile = true;
        health = maxHealth;
    }

    private void Update()
    {
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
        
        if (!isStunned)
        {
            //if is not stunned by player
            Behaviour();
        }
    }

    

    void Behaviour()
    {
        if (canShootProjectile)
        {
            canShootProjectile = false;
            StartCoroutine(ShootProjectile());
        }
    }
    
    IEnumerator ShootProjectile()
    {
        //shoots a projectile
        GameObject projN = Instantiate(mageProjectile, transform.position + Vector3.forward * 5, quaternion.identity);
        GameObject projS = Instantiate(mageProjectile, transform.position + Vector3.back * 5, quaternion.identity);        
        GameObject projE = Instantiate(mageProjectile, transform.position + Vector3.left * 5, quaternion.identity);
        GameObject projW = Instantiate(mageProjectile, transform.position + Vector3.right * 5, quaternion.identity);
        yield return new WaitForSeconds(shootWarmup);
        //gives it proper force
        projN.GetComponent<Rigidbody>().AddForce(new Vector3(20, 0, 20) * shootForce);
        projN.GetComponent<ProjectileDamage>().damage = projectileDamage;
        
        projS.GetComponent<Rigidbody>().AddForce(new Vector3(-20, 0, -20) * shootForce);
        projS.GetComponent<ProjectileDamage>().damage = projectileDamage;
        
        projE.GetComponent<Rigidbody>().AddForce(new Vector3(-20, 0, 20) * shootForce);
        projE.GetComponent<ProjectileDamage>().damage = projectileDamage;
        
        projW.GetComponent<Rigidbody>().AddForce(new Vector3(20, 0, -20) * shootForce);
        projW.GetComponent<ProjectileDamage>().damage = projectileDamage;
        
        //se tp 
        yield return new WaitForSeconds(1);
        //calculates a random position where the enemy will spawn
        int randPosX = Random.Range(-20, 21);
        int randPosY = Random.Range(-20, 21);
        Vector3 tpPoint = transform.position + new Vector3(randPosX, player.transform.position.y + 5, randPosY);
        transform.position = tpPoint;
        
        
        //les rappelle
        if (projN != null)
        {        
            projN.GetComponent<Rigidbody>().velocity = Vector3.zero;
            projN.GetComponent<Rigidbody>().AddForce((transform.position - projN.transform.position) * retreatForce);
        }

        if (projS != null)
        {
            projS.GetComponent<Rigidbody>().velocity = Vector3.zero;
            projS.GetComponent<Rigidbody>().AddForce((transform.position - projS.transform.position) * retreatForce);
        }

        if (projE != null)
        {
            projE.GetComponent<Rigidbody>().velocity = Vector3.zero;
            projE.GetComponent<Rigidbody>().AddForce((transform.position - projE.transform.position) * retreatForce);
        }

        if (projW != null)
        {
            projW.GetComponent<Rigidbody>().velocity = Vector3.zero;
            projW.GetComponent<Rigidbody>().AddForce((transform.position - projW.transform.position) * retreatForce);
        }

        //waits for cooldown to refresh to shoot again
        yield return new WaitForSeconds(projectileCooldown);
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
